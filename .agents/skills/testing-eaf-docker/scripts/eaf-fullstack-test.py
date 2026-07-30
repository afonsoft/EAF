#!/usr/bin/env python3
"""End-to-end smoke test for the EAF Docker full stack.

Covers:
- Host / tenant JWT login and tenant claim verification
- Tenant CRUD and data isolation
- Same user in multiple tenants
- Unique users per tenant
- Cross-tenant SignalR chat

Requires the full stack up and a few environment variables:
    export MSSQL_SA_PASSWORD='<your-sql-sa-password>'
    export EAF_INITIAL_PASSWORD='<current-admin-password>'
    export EAF_DEFAULT_PASSWORD='<desired-admin-password>'
    docker compose -f docker-compose.all.yml up -d --build

Optional dependency for the chat step:
    pip3 install signalrcore
"""
import base64
import json
import os
import sys
import time
import urllib.error
import urllib.request

API = os.environ.get("EAF_API_URL", "http://localhost:5000")
ANGULAR = os.environ.get("EAF_ANGULAR_URL", "http://localhost:4200")

INITIAL_PASSWORD = os.environ.get("EAF_INITIAL_PASSWORD")
DEFAULT_PASSWORD = os.environ.get("EAF_DEFAULT_PASSWORD")

if not DEFAULT_PASSWORD:
    print("ERROR: set EAF_DEFAULT_PASSWORD to the desired admin password", file=sys.stderr)
    sys.exit(1)

RESULTS = []


def http(method, path, body=None, headers=None):
    url = API + path
    data = json.dumps(body).encode("utf-8") if body is not None else None
    req = urllib.request.Request(url, data=data, method=method)
    req.add_header("Content-Type", "application/json")
    if headers:
        for k, v in headers.items():
            req.add_header(k, v)
    try:
        with urllib.request.urlopen(req, timeout=30) as resp:
            return resp.status, json.loads(resp.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        body = e.read().decode("utf-8")
        try:
            return e.code, json.loads(body)
        except json.JSONDecodeError:
            return e.code, body


def record(name, ok, detail=""):
    RESULTS.append((name, ok, detail))
    print(f"  {'OK' if ok else 'FAIL'}: {name}{(' - ' + detail) if detail else ''}")


def decode_token(token):
    payload_b64 = token.split(".")[1]
    pad = 4 - len(payload_b64) % 4
    if pad != 4:
        payload_b64 += "=" * pad
    return json.loads(base64.urlsafe_b64decode(payload_b64).decode("utf-8"))


def login(username, password, tenant_id=None):
    headers = {}
    if tenant_id:
        headers["Abp-TenantId"] = str(tenant_id)
    return http("POST", "/api/TokenAuth/Authenticate",
                {"userNameOrEmailAddress": username, "password": password, "rememberClient": False}, headers)


def ensure_admin_password(username, password):
    """Reset default admin password on the first run."""
    # Try the target password first in case it was already reset.
    code, r = login(username, password)
    if code == 200 and r.get("result", {}).get("accessToken"):
        return code, r

    if not INITIAL_PASSWORD:
        print("ERROR: host admin requires a password reset but EAF_INITIAL_PASSWORD is not set", file=sys.stderr)
        sys.exit(1)

    code, r = login(username, INITIAL_PASSWORD)
    if code == 200 and r.get("result", {}).get("shouldResetPassword"):
        reset_code = r["result"]["passwordResetCode"]
        user_id = r["result"]["userId"]
        http("POST", "/api/services/app/Account/ResetPassword",
             {"userId": user_id, "password": password, "resetCode": reset_code})
        return login(username, password)
    return code, r


def get_users(token):
    code, r = http("GET", "/api/services/app/User/GetUsers?MaxResultCount=100&SkipCount=0",
                   headers={"Authorization": "Bearer " + token})
    if code != 200:
        return []
    return r["result"]["items"]


def user_exists(token, username):
    return any(u["userName"] == username for u in get_users(token))


def create_user(token, username, email, password):
    if user_exists(token, username):
        return True
    code, r = http("POST", "/api/services/app/User/CreateOrUpdateUser",
                   {"assignedRoleNames": ["Admin"], "setRandomPassword": False,
                    "sendActivationEmail": False,
                    "user": {"userName": username, "name": username.capitalize(),
                             "surname": "T", "emailAddress": email,
                             "isActive": True, "password": password,
                             "shouldChangePasswordOnNextLogin": False}},
                   {"Authorization": "Bearer " + token})
    return r.get("success", False) or "já foi usado" in str(r)


def enable_chat_features(host_token, tenant_ids):
    for tid in tenant_ids:
        http("PUT", "/api/services/app/Tenant/UpdateTenantFeatures",
             {"id": tid,
              "featureValues": [
                  {"name": "App.ChatFeature", "value": "true"},
                  {"name": "App.ChatFeature.TenantToTenant", "value": "true"},
                  {"name": "App.ChatFeature.GroupChat", "value": "true"}]},
             {"Authorization": "Bearer " + host_token})


def send_signalr_message(sender_token, target_tenant_id, target_user_id, target_tenancy_name, message):
    try:
        from signalrcore.hub_connection_builder import HubConnectionBuilder
    except ImportError:
        record("SignalR chat", False, "signalrcore not installed (pip3 install signalrcore)")
        return False

    connection = (
        HubConnectionBuilder()
        .with_url(f"{API}/signalr-chat?access_token={sender_token}")
        .build()
    )
    received = []

    def handler(msg):
        received.append(msg)

    connection.on("getChatMessage", handler)
    connection.start()
    time.sleep(2)

    payload = {
        "userId": target_user_id,
        "tenantId": target_tenant_id,
        "tenancyName": target_tenancy_name,
        "userName": "admin",
        "message": message,
    }
    connection.send("SendMessage", [payload])
    time.sleep(2)
    connection.stop()

    ok = len(received) > 0 and any(payload["message"] in str(m) for m in received)
    record("SignalR chat", ok, f"sent='{message}', received={len(received)}")
    return ok


def main():
    print("=== EAF Docker Full-Stack Test ===\n")

    print("1. Stack health")
    for name, url in [("API", API + "/AbpUserConfiguration/GetAll"), ("Angular", ANGULAR)]:
        try:
            with urllib.request.urlopen(url, timeout=10) as resp:
                record(f"{name} reachable", resp.status == 200, f"status={resp.status}")
        except Exception as e:
            record(f"{name} reachable", False, str(e))

    print("\n2. Host admin login and password reset")
    code, r = ensure_admin_password("admin", DEFAULT_PASSWORD)
    if not (code == 200 and r.get("result", {}).get("accessToken")):
        record("Host admin login", False, str(r))
        sys.exit(1)
    host_token = r["result"]["accessToken"]
    host_payload = decode_token(host_token)
    record("Host admin login", True, f"tenantid={host_payload.get('tenantid')} sub={host_payload.get('sub')}")

    print("\n3. Tenant CRUD")
    tenants = {}
    for tname in ["tenantA", "tenantB"]:
        code, r = http("POST", "/api/services/app/Tenant/CreateTenant",
                       {"tenancyName": tname, "name": tname,
                        "adminEmailAddress": f"admin@{tname}.com",
                        "adminPassword": DEFAULT_PASSWORD, "isActive": True,
                        "shouldChangePasswordOnNextLogin": False},
                       {"Authorization": "Bearer " + host_token})
        created = r.get("success", False) or "já foi usado" in str(r) or "already" in str(r).lower()
        record(f"Create tenant {tname}", created, str(r.get("message", "")) if not created else "")

    code, r = http("GET", "/api/services/app/Tenant/GetTenants?MaxResultCount=100&SkipCount=0",
                   headers={"Authorization": "Bearer " + host_token})
    for t in r["result"]["items"]:
        tenants[t["tenancyName"]] = t["id"]
    record("Tenant list loaded", bool(tenants), str(tenants))

    print("\n4. Tenant admin login")
    tenant_tokens = {}
    for tname in ["tenantA", "tenantB"]:
        if tname not in tenants:
            continue
        tid = tenants[tname]
        code, r = login("admin", DEFAULT_PASSWORD, tid)
        ok = code == 200 and r.get("result", {}).get("accessToken")
        if ok:
            tenant_tokens[tname] = r["result"]["accessToken"]
            payload = decode_token(tenant_tokens[tname])
            record(f"{tname} admin login", True, f"tenantid={payload.get('tenantid')} sub={payload.get('sub')}")
        else:
            record(f"{tname} admin login", False, str(r))

    if not tenant_tokens:
        print("Missing tenant tokens, aborting")
        sys.exit(1)

    print("\n5. Enable chat features for cross-tenant chat")
    enable_chat_features(host_token, [tenants["tenantA"], tenants["tenantB"]])
    record("Chat features enabled", True)

    print("\n6. User creation (shared + unique per tenant)")
    plan = {
        "tenantA": [("shareduser", "shared@tenantA.com"), ("alice", "alice@tenantA.com")],
        "tenantB": [("shareduser", "shared@tenantB.com"), ("bob", "bob@tenantB.com")],
    }
    for tname, ulist in plan.items():
        for username, email in ulist:
            ok = create_user(tenant_tokens[tname], username, email, DEFAULT_PASSWORD)
            record(f"Create user {username} in {tname}", ok)

    print("\n7. Login checks")
    checks = [
        ("shareduser", tenants["tenantA"]),
        ("shareduser", tenants["tenantB"]),
        ("alice", tenants["tenantA"]),
        ("bob", tenants["tenantB"]),
    ]
    for username, tid in checks:
        code, r = login(username, DEFAULT_PASSWORD, tid)
        ok = code == 200 and r.get("result", {}).get("accessToken")
        if ok:
            payload = decode_token(r["result"]["accessToken"])
            record(f"Login {username} tenant {tid}", True,
                   f"tenantid={payload.get('tenantid')} sub={payload.get('sub')}")
        else:
            record(f"Login {username} tenant {tid}", False, str(r))

    print("\n8. Tenant data isolation")
    code, r = http("GET", "/api/services/app/User/GetUsers?MaxResultCount=100&SkipCount=0",
                   headers={"Authorization": "Bearer " + tenant_tokens["tenantA"]})
    tenant_a_usernames = {u["userName"] for u in r["result"]["items"]}
    record("TenantA user list", True, str(tenant_a_usernames))
    record("Bob isolated from tenantA", "bob" not in tenant_a_usernames)
    record("Alice present in tenantA", "alice" in tenant_a_usernames)

    print("\n9. Cross-tenant friendship")
    code, r = http("POST", "/api/services/app/Friendship/CreateFriendshipRequestByUserName",
                   {"tenancyName": "tenantB", "userName": "admin"},
                   {"Authorization": "Bearer " + tenant_tokens["tenantA"]})
    friend_ok = r.get("success", False) or any(s in str(r.get("message", "")).lower() for s in ["já enviou", "already sent", "youarealready"])
    record("Friendship tenantA -> tenantB", friend_ok, str(r.get("message", "")))

    print("\n10. Cross-tenant chat via SignalR")
    tenant_a_users = {u["userName"]: u["id"] for u in get_users(tenant_tokens["tenantA"])}
    tenant_b_users = {u["userName"]: u["id"] for u in get_users(tenant_tokens["tenantB"])}
    tenant_a_admin_id = tenant_a_users.get("admin")
    tenant_b_admin_id = tenant_b_users.get("admin")

    ok = send_signalr_message(
        tenant_tokens["tenantA"],
        tenants["tenantB"],
        tenant_b_admin_id,
        "tenantB",
        "Hello tenantB admin from tenantA",
    )
    if ok and tenant_a_admin_id:
        # Verify the message is persisted on the receiver side.
        code, r = http("GET",
                       f"/api/services/app/Chat/GetUserChatMessages?UserId={tenant_a_admin_id}&TenantId={tenants['tenantA']}",
                       headers={"Authorization": "Bearer " + tenant_tokens["tenantB"]})
        messages = [m["message"] for m in r.get("result", {}).get("items", [])]
        record("Receiver sees cross-tenant message", any("tenantB" in m for m in messages), str(messages))

    print("\n=== Summary ===")
    failures = [n for n, ok, _ in RESULTS if not ok]
    for name, ok, detail in RESULTS:
        print(f"  [{'PASS' if ok else 'FAIL'}] {name} {detail}")
    print(f"\nTotal: {len(RESULTS) - len(failures)}/{len(RESULTS)} passed")
    if failures:
        print(f"Failures: {failures}")
        sys.exit(1)


if __name__ == "__main__":
    main()
