#!/usr/bin/env python3
"""Playwright visual verification of the EAF Angular admin UI."""
import json
import os
import sys
from playwright.sync_api import sync_playwright

BASE = os.environ.get("EAF_ANGULAR_URL", "http://localhost:4200")
USER = os.environ.get("EAF_ADMIN_USER", "admin")
PASS = os.environ.get("EAF_ADMIN_PASSWORD", "P@ssw0rd123!")
SCREENSHOT_DIR = os.environ.get("EAF_SCREENSHOT_DIR", "/home/ubuntu/repos/EAF/screenshots")

os.makedirs(SCREENSHOT_DIR, exist_ok=True)

console_errors = []
page_errors = []
failed_responses = []

def wait_for_text_regex(page, regex, timeout=30000):
    """Wait until the rendered body matches the supplied JS regex."""
    page.wait_for_function(
        f"new RegExp({json.dumps(regex)}).test(document.body.innerText)",
        timeout=timeout,
    )

def safe_inner_text(page, selector):
    try:
        return page.locator(selector).first.inner_text(timeout=5000)
    except Exception:
        return ""

def main():
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True, args=["--no-sandbox", "--disable-gpu"])
        context = browser.new_context(viewport={"width": 1920, "height": 1080})
        page = context.new_page()

        page.on("console", lambda msg: console_errors.append(f"{msg.type}: {msg.text}") if msg.type in ("error", "severe") else None)
        page.on("pageerror", lambda err: page_errors.append(str(err)))
        page.on("response", lambda resp: failed_responses.append(f"{resp.status}: {resp.url}") if resp.status >= 500 else None)

        results = []

        # 1. Login page
        page.goto(f"{BASE}/account/login", wait_until="networkidle")
        page.screenshot(path=os.path.join(SCREENSHOT_DIR, "01-login-page.png"), full_page=True)
        body_text = page.locator("body").inner_text()
        login_rendered = bool(
            any(t in body_text for t in ["LogIn", "Log In", "Login", "Sign in", "Entrar"])
        )
        results.append(("Login page renders", login_rendered, body_text[:200]))

        # Switch to normal login form if a social/external view is active
        if not page.locator("#Password").is_visible():
            for sel in ["a:has-text('LoginSistem')", "a:has-text('Back')"]:
                link = page.locator(sel).first
                if link.is_visible():
                    link.click()
                    page.wait_for_timeout(500)
                    break

        # Fill credentials and submit
        page.fill("#userNameOrEmailAddress", USER)
        page.fill("#Password", PASS)
        page.click("button[type='submit']")

        # Wait for post-login dashboard route and content
        page.wait_for_url("**/app/main/dashboard", timeout=30000)
        wait_for_text_regex(page, r"Dashboard|In[íi]cio|Painel", timeout=30000)
        # Wait for the dashboard data area to contain either tiles, empty state, or a spinner
        page.wait_for_function(
            "!!document.querySelector('#TenantDashboard .m-content .m-widget24__title, #TenantDashboard .m-content app-empty-state, #TenantDashboard .m-content .fa-spinner')",
            timeout=30000,
        )
        page.wait_for_timeout(1500)
        page.screenshot(path=os.path.join(SCREENSHOT_DIR, "02-dashboard.png"), full_page=True)
        dashboard_text = safe_inner_text(page, "#TenantDashboard")
        results.append(("Dashboard renders", bool(dashboard_text and "Dashboard" in dashboard_text), dashboard_text[:200]))

        # 2. Gateway selection (public account route)
        page.goto(f"{BASE}/account/gateway-selection", wait_until="networkidle")
        wait_for_text_regex(page, r"Gateway [Ss]election|Selecionar gateway", timeout=30000)
        page.wait_for_selector("#GatewayName, #GatewayEdition", timeout=30000)
        page.screenshot(path=os.path.join(SCREENSHOT_DIR, "03-gateway-selection.png"), full_page=True)
        gateway_text = page.locator("body").inner_text()
        results.append(("Gateway selection renders", bool("Gateway" in gateway_text), gateway_text[:200]))

        # 3. Subscriptions (admin route)
        page.goto(f"{BASE}/app/admin/subscriptions", wait_until="networkidle")
        wait_for_text_regex(page, r"Subscriptions|Assinaturas", timeout=30000)
        page.wait_for_selector("#SubscriptionsFilterText, p-table, app-empty-state", timeout=30000)
        page.screenshot(path=os.path.join(SCREENSHOT_DIR, "04-subscriptions.png"), full_page=True)
        subscriptions_text = page.locator("body").inner_text()
        results.append(("Subscriptions renders", bool("Subscriptions" in subscriptions_text or "Assinaturas" in subscriptions_text), subscriptions_text[:200]))

        browser.close()

        print("=== Visual verification ===")
        for name, ok, detail in results:
            print(f"  [{'PASS' if ok else 'FAIL'}] {name}")
            if detail:
                print(f"      detail: {detail!r}")

        print(f"\nConsole errors: {len(console_errors)}")
        for e in console_errors[:20]:
            print("  ", e)
        print(f"Page errors: {len(page_errors)}")
        for e in page_errors[:20]:
            print("  ", e)
        print(f"5xx responses: {len(failed_responses)}")
        for r in failed_responses[:20]:
            print("  ", r)

        if any(not ok for _, ok, _ in results) or console_errors or page_errors or failed_responses:
            print("\nFAIL: visual verification found issues")
            sys.exit(1)
        print("\nPASS: all visual checks passed with no console errors")

if __name__ == "__main__":
    main()
