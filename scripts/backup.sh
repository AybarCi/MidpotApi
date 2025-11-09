#!/bin/bash
# PostgreSQL Backup Script for Production

set -e

# Configuration
BACKUP_DIR="/backups"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/backup_${POSTGRES_DB}_${DATE}.sql.gz"
RETENTION_DAYS=${BACKUP_RETENTION:-7}

# Create backup directory if it doesn't exist
mkdir -p ${BACKUP_DIR}

echo "Starting PostgreSQL backup for database: ${POSTGRES_DB}"

# Perform backup
PGPASSWORD=${POSTGRES_PASSWORD} pg_dump \
  -h postgres \
  -U ${POSTGRES_USER} \
  -d ${POSTGRES_DB} \
  --clean \
  --create \
  --verbose \
  --format=plain \
  | gzip > ${BACKUP_FILE}

# Check if backup was successful
if [ $? -eq 0 ]; then
  echo "Backup completed successfully: ${BACKUP_FILE}"
  
  # Get backup size
  BACKUP_SIZE=$(du -h ${BACKUP_FILE} | cut -f1)
  echo "Backup size: ${BACKUP_SIZE}"
  
  # Remove old backups
  echo "Removing backups older than ${RETENTION_DAYS} days"
  find ${BACKUP_DIR} -name "backup_*.sql.gz" -type f -mtime +${RETENTION_DAYS} -delete
  
  # List remaining backups
  echo "Remaining backups:"
  ls -lh ${BACKUP_DIR}/backup_*.sql.gz 2>/dev/null || echo "No backups found"
  
else
  echo "Backup failed!"
  exit 1
fi