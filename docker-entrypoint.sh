#!/bin/sh
set -e

# Wait for database to be ready
echo "Waiting for database connection..."
while ! curl -f http://$DB_HOST:$DB_PORT/health > /dev/null 2>&1; do
  echo "Waiting for database..."
  sleep 2
done

# Wait for Redis to be ready
if [ ! -z "$REDIS_HOST" ]; then
  echo "Waiting for Redis connection..."
  while ! timeout 1 bash -c "echo > /dev/tcp/$REDIS_HOST/$REDIS_PORT" 2>/dev/null; do
    echo "Waiting for Redis..."
    sleep 2
  done
fi

# Apply database migrations (optional - uncomment if needed)
# echo "Applying database migrations..."
# dotnet ef database update --project /app/DatingWeb.csproj --startup-project /app

echo "Starting application..."
exec "$@"