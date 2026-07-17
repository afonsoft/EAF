#!/bin/bash
set -euo pipefail

COMPOSE_FILE="${COMPOSE_FILE:-docker-compose.all.yml}"
API_PORT="${API_PORT:-5000}"
ANGULAR_PORT="${ANGULAR_PORT:-4200}"
MAX_WAIT="${MAX_WAIT:-300}"
API_URL="http://localhost:${API_PORT}/swagger/v1/swagger.json"
ANGULAR_URL="http://localhost:${ANGULAR_PORT}/"

cleanup() {
    echo ""
    echo "Cleaning up Docker Compose stack ($COMPOSE_FILE)..."
    docker compose -f "$COMPOSE_FILE" down --volumes --remove-orphans || true
}
trap cleanup EXIT

wait_for_url() {
    local url="$1"
    local label="$2"
    local elapsed=0

    echo "Waiting for $label ($url)..."
    while [ "$elapsed" -lt "$MAX_WAIT" ]; do
        if curl -fsS "$url" > /dev/null 2>&1; then
            echo "  $label is ready."
            return 0
        fi
        sleep 5
        elapsed=$((elapsed + 5))
    done

    echo "ERROR: $label did not become ready within ${MAX_WAIT}s."
    return 1
}

echo "Validating compose file: $COMPOSE_FILE"
docker compose -f "$COMPOSE_FILE" config > /dev/null

echo "Starting stack: docker compose -f $COMPOSE_FILE up --build -d"
docker compose -f "$COMPOSE_FILE" up --build -d

wait_for_url "$API_URL" "API Swagger JSON"
wait_for_url "$ANGULAR_URL" "Angular frontend"

if docker compose -f "$COMPOSE_FILE" config --services | grep -q '^eaf-migrator$'; then
    echo "Checking migrator completion..."
    migrator_status=$(docker compose -f "$COMPOSE_FILE" ps -a eaf-migrator --format '{{.Status}}' 2>/dev/null || true)
    if echo "$migrator_status" | grep -q 'Exited (0)'; then
        echo "  Migrator completed successfully."
    else
        echo "  Migrator status: $migrator_status"
    fi
fi

if docker compose -f "$COMPOSE_FILE" config --services | grep -q '^eaf-worker$'; then
    echo "Checking worker logs for critical errors..."
    worker_logs=$(docker logs eaf-worker 2>&1 || true)
    if echo "$worker_logs" | grep -qiE 'FATAL|Unhandled|Critical'; then
        echo "ERROR: Worker log contains fatal/critical messages."
        exit 1
    fi
    echo "  Worker logs look clean."
fi

echo ""
echo "Validation passed for $COMPOSE_FILE"
echo "  - API Swagger JSON: $API_URL"
echo "  - Angular frontend:  $ANGULAR_URL"
