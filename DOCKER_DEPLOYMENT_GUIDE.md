# Midpot API Docker Production Deployment Guide

## 🚀 Production Docker Setup for Portainer

This guide explains how to deploy the Midpot API in a production environment using Docker and Portainer.

## 📋 Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+
- Portainer CE/Business
- Domain name (e.g., api.midpot.net)
- SSL certificates (for HTTPS)

## 📁 File Structure

```
MidpotApi/
├── Dockerfile.production      # Optimized production Dockerfile
├── docker-compose.yml        # Local development compose
├── portainer-stack.yml       # Production Portainer stack
├── docker-entrypoint.sh      # Container startup script
├── redis.conf               # Redis configuration
├── nginx.conf               # Nginx reverse proxy config
├── .env.example             # Environment variables template
├── appsettings.Production.json # Production settings
├── scripts/
│   └── backup.sh            # Database backup script
└── ssl/                     # SSL certificates (create this folder)
    ├── cert.pem
    └── key.pem
```

## 🔧 Environment Configuration

### 1. Create Environment File

```bash
cp .env.example .env
# Edit .env with your production values
```

### 2. Required Environment Variables

Update these variables in your `.env` file:

```bash
# Database
POSTGRES_DB=socialdb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password

# JWT
JWT_KEY=your_super_secure_jwt_key_here_at_least_64_characters
JWT_ISSUER=https://www.midpot.net
JWT_AUDIENCE=https://www.midpot.net

# External Services (Optional)
AZURE_BLOB_STORAGE=your_azure_connection_string
FCM_SENDER_ID=your_fcm_sender_id
FCM_SERVER_KEY=your_fcm_server_key
SMS_SERVICE_URL=https://api.vatansms.net/api/v1/1toN
SMS_USER_ID=your_sms_user_id
SMS_PASSWORD=your_sms_password
SMS_SENDER=YourName
```

### 3. SSL Certificates

Place your SSL certificates in the `ssl/` folder:

```bash
mkdir ssl
cp your_certificate.pem ssl/cert.pem
cp your_private_key.pem ssl/key.pem
chmod 600 ssl/key.pem
```

## 🐳 Docker Build & Deploy

### Option 1: Local Docker Compose (Development)

```bash
# Build and start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Option 2: Portainer Stack (Production)

#### 1. Build Docker Image

```bash
# Build production image
docker build -f Dockerfile.production -t your-registry/midpot-api:latest .

# Push to registry (if using private registry)
docker push your-registry/midpot-api:latest
```

#### 2. Deploy in Portainer

1. Login to Portainer
2. Go to **Stacks** → **Add Stack**
3. Name: `midpot-production`
4. Build method: **Web Editor**
5. Copy content from `portainer-stack.yml`
6. Add environment variables in Portainer
7. Deploy the stack

#### 3. Environment Variables in Portainer

Add these as environment variables in Portainer:

```bash
# Required
POSTGRES_PASSWORD=your_secure_password
JWT_KEY=your_jwt_key

# Optional (update as needed)
API_DOMAIN=api.midpot.net
GITHUB_REPOSITORY=AybarCi/MidpotApi
API_VERSION=latest
```

## 🔍 Health Checks

The application includes health checks:

- **Application Health**: `http://localhost/health`
- **Database Health**: Included in `/health` endpoint
- **Redis Health**: Included in `/health` endpoint

## 📊 Monitoring

### Container Logs

```bash
# View all service logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f midpot-api

# Portainer logs are available in the UI
```

### Performance Monitoring

- **Database**: Monitor PostgreSQL performance
- **Redis**: Monitor cache hit rates and memory usage
- **Application**: Monitor response times and error rates

## 🔄 Updates & Rollbacks

### Updating the Application

1. Build new image:
```bash
docker build -f Dockerfile.production -t your-registry/midpot-api:v2.0 .
docker push your-registry/midpot-api:v2.0
```

2. Update Portainer stack with new version
3. Redeploy the stack

### Rollback

In Portainer:
1. Go to the stack
2. Click **Rollback**
3. Select previous version

## 🗄️ Database Management

### Backups

Automatic backups are configured to run daily at 2 AM.

```bash
# Manual backup
docker exec midpot-postgres-backup /backup.sh

# View backups
ls -la backups/
```

### Restore

```bash
# Restore from backup
gunzip < backups/backup_socialdb_YYYYMMDD_HHMMSS.sql.gz | docker exec -i midpot-postgres psql -U postgres -d socialdb
```

## 🛡️ Security

### Security Features

- Non-root container user
- Health checks
- Rate limiting (via Nginx)
- SSL/TLS encryption
- Security headers
- Input validation

### Security Best Practices

1. **Change default passwords**
2. **Use strong JWT keys**
3. **Enable firewall rules**
4. **Regular security updates**
5. **Monitor logs for suspicious activity**

## 🔧 Troubleshooting

### Common Issues

#### Container Won't Start
```bash
# Check logs
docker logs midpot-api

# Check health
curl http://localhost/health
```

#### Database Connection Issues
```bash
# Check PostgreSQL logs
docker logs midpot-postgres

# Test connection
docker exec midpot-api nc -zv postgres 5432
```

#### Redis Connection Issues
```bash
# Check Redis logs
docker logs midpot-redis

# Test connection
docker exec midpot-api nc -zv redis 6379
```

### Performance Issues

1. **Check resource limits** in Portainer
2. **Monitor database queries**
3. **Check Redis memory usage**
4. **Review application logs**

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Portainer Documentation](https://docs.portainer.io/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
- [Redis Docker Image](https://hub.docker.com/_/redis)

## 🆘 Support

For issues:
1. Check container logs
2. Review health check status
3. Check resource usage
4. Verify environment variables
5. Contact your system administrator