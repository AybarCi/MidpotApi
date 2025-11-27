#!/bin/sh
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "${GREEN}Starting Midpot API...${NC}"

# Function to wait for PostgreSQL
wait_for_postgres() {
    echo "${YELLOW}Waiting for PostgreSQL to be ready...${NC}"
    
    POSTGRES_HOST="${DB_HOST:-172.17.0.9}"
    POSTGRES_PORT="${DB_PORT:-5432}"
    MAX_RETRIES=30
    RETRY_COUNT=0
    
    until nc -z "$POSTGRES_HOST" "$POSTGRES_PORT" 2>/dev/null; do
        RETRY_COUNT=$((RETRY_COUNT + 1))
        if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
            echo "${RED}PostgreSQL is unavailable after $MAX_RETRIES attempts - exiting${NC}"
            exit 1
        fi
        echo "PostgreSQL is unavailable - sleeping (attempt $RETRY_COUNT/$MAX_RETRIES)"
        sleep 2
    done
    
    echo "${GREEN}PostgreSQL is up and ready!${NC}"
}

# Function to wait for Redis
wait_for_redis() {
    echo "${YELLOW}Waiting for Redis to be ready...${NC}"
    
    REDIS_HOST="${REDIS_HOST:-redis}"
    REDIS_PORT="${REDIS_PORT:-6379}"
    MAX_RETRIES=30
    RETRY_COUNT=0
    
    until nc -z "$REDIS_HOST" "$REDIS_PORT" 2>/dev/null; do
        RETRY_COUNT=$((RETRY_COUNT + 1))
        if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
            echo "${RED}Redis is unavailable after $MAX_RETRIES attempts - exiting${NC}"
            exit 1
        fi
        echo "Redis is unavailable - sleeping (attempt $RETRY_COUNT/$MAX_RETRIES)"
        sleep 2
    done
    
    echo "${GREEN}Redis is up and ready!${NC}"
}

# Function to run database migrations (optional)
run_migrations() {
    if [ "$RUN_MIGRATIONS" = "true" ]; then
        echo "${YELLOW}Running database migrations...${NC}"
        dotnet ef database update --no-build || {
            echo "${RED}Migration failed!${NC}"
            exit 1
        }
        echo "${GREEN}Migrations completed successfully!${NC}"
    else
        echo "${YELLOW}Skipping migrations (RUN_MIGRATIONS not set to 'true')${NC}"
    fi
}

# Graceful shutdown handler
shutdown_handler() {
    echo "${YELLOW}Received shutdown signal, gracefully shutting down...${NC}"
    # Give the application time to finish current requests
    sleep 5
    exit 0
}

# Trap SIGTERM and SIGINT for graceful shutdown
trap shutdown_handler SIGTERM SIGINT

# Main execution
echo "Environment: ${ASPNETCORE_ENVIRONMENT:-Production}"
echo "Database Host: ${DB_HOST:-postgres}"
echo "Redis Host: ${REDIS_HOST:-redis}"

# Wait for dependencies
wait_for_postgres
wait_for_redis

# Run migrations if enabled
run_migrations

# Print startup message
echo "${GREEN}All dependencies are ready. Starting application...${NC}"
echo "${GREEN}======================================${NC}"

# Execute the main application
exec "$@"