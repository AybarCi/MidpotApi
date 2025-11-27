# GitHub Container Registry (GHCR) Deployment Guide

## Overview

MidpotApi uses GitHub Actions to automatically build and push Docker images to GitHub Container Registry (GHCR). This allows you to pull pre-built images instead of building locally.

## Image Registry

- **Registry**: GitHub Container Registry (ghcr.io)
- **Repository**: `ghcr.io/aybarci/midpotapi`
- **Available Tags**:
  - `latest` - Latest production build from main branch
  - `develop` - Latest staging build from develop branch
  - `develop-<sha>` - Specific commit from develop branch
  - `main-<sha>` - Specific commit from main branch

## Deployment Options

### Option 1: Production Deployment (Recommended)

Use the production docker-compose file that pulls from GHCR:

```bash
# 1. Copy environment template
cp .env.example .env

# 2. Edit .env with your production values
nano .env

# 3. Pull latest image and start services
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d

# 4. Check logs
docker-compose -f docker-compose.prod.yml logs -f midpot-api
```

### Option 2: Using Default docker-compose.yml

The default `docker-compose.yml` now supports image pull mode:

```bash
# Default: Use GHCR image (production)
docker-compose up -d

# Use specific version
DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:develop docker-compose up -d

# For local development (build from source)
# 1. Edit docker-compose.yml
# 2. Comment out 'image:' line
# 3. Uncomment 'build:' section
docker-compose up -d --build
```

### Option 3: Portainer Stack Deployment

For Portainer, use the production compose file:

1. Login to Portainer
2. Go to **Stacks** → **Add Stack**
3. Name: `midpot-production`
4. Upload `docker-compose.prod.yml`
5. Add environment variables
6. Deploy

## Environment Variables

Required in `.env` file:

```bash
# Docker Image (use GHCR by default)
DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:latest

# Database
POSTGRES_PASSWORD=your_secure_password

# JWT & Security
JWT_KEY=your_jwt_key
SECURITY_KEY=your_security_key

# Optional services
AZURE_BLOB_STORAGE=...
FCM_SENDER_ID=...
FCM_SERVER_KEY=...
```

## Pulling Images Manually

### Public Registry (if configured)

```bash
# Pull latest
docker pull ghcr.io/aybarci/midpotapi:latest

# Pull specific version
docker pull ghcr.io/aybarci/midpotapi:develop
```

### Private Registry (requires authentication)

If the repository is private, you need to authenticate:

```bash
# 1. Create a Personal Access Token (PAT) on GitHub
# Go to: Settings → Developer settings → Personal access tokens
# Scopes: read:packages

# 2. Login to GHCR
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# 3. Pull image
docker pull ghcr.io/aybarci/midpotapi:latest
```

## CI/CD Workflow

The GitHub Actions workflow automatically:

1. **On push to `develop` branch**:
   - Builds Docker image
   - Pushes to `ghcr.io/aybarci/midpotapi:develop`
   - Pushes to `ghcr.io/aybarci/midpotapi:develop-<sha>`

2. **On push to `main` branch**:
   - Builds Docker image
   - Pushes to `ghcr.io/aybarci/midpotapi:latest`
   - Pushes to `ghcr.io/aybarci/midpotapi:main-<sha>`

3. **On Docker Hub** (if configured):
   - Also pushes to Docker Hub registry

## Updating to Latest Version

```bash
# Pull latest image
docker-compose -f docker-compose.prod.yml pull

# Recreate container with new image
docker-compose -f docker-compose.prod.yml up -d

# Or use one command
docker-compose -f docker-compose.prod.yml pull && docker-compose -f docker-compose.prod.yml up -d
```

## Rollback to Previous Version

```bash
# Use specific commit SHA
DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:main-abc1234 docker-compose up -d

# Or edit .env file
# DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:main-abc1234
docker-compose up -d
```

## Health Check

After deployment, verify the application is running:

```bash
# Check container status
docker-compose ps

# Check health endpoint
curl http://localhost:5000/health

# View logs
docker-compose logs -f midpot-api
```

## Troubleshooting

### Image Pull Failed

**Error**: `Error response from daemon: pull access denied`

**Solution**:
1. Ensure the repository is public, OR
2. Login to GHCR:
   ```bash
   echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
   ```

### Wrong Image Version

**Error**: Container is running old version

**Solution**:
```bash
# Force pull and recreate
docker-compose pull
docker-compose up -d --force-recreate
```

### Local Build Instead of Pull

If you want to build locally for testing:

```bash
# Edit docker-compose.yml
# Comment: image: ${DOCKER_IMAGE:-ghcr.io/aybarci/midpotapi:latest}
# Uncomment: build section

docker-compose up -d --build
```

## Best Practices

1. **Production**: Always use tagged versions (not `latest`)
   ```bash
   DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:main-abc1234
   ```

2. **Staging**: Use `develop` tag
   ```bash
   DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:develop
   ```

3. **Testing**: Use commit-specific tags
   ```bash
   DOCKER_IMAGE=ghcr.io/aybarci/midpotapi:develop-abc1234
   ```

4. **Local Development**: Build from source
   - Comment out `image:` 
   - Uncomment `build:` in docker-compose.yml

## Automated Deployments

### Webhook Integration

You can set up webhooks to auto-deploy when new images are pushed:

```bash
# Portainer webhook example
curl -X POST https://portainer.example.com/api/webhooks/xxx
```

### Watchtower (Auto-update)

Use Watchtower to automatically update containers:

```yaml
watchtower:
  image: containrrr/watchtower
  volumes:
    - /var/run/docker.sock:/var/run/docker.sock
  command: --interval 300 midpot-api
```

## Security Notes

- Never commit `.env` file with real credentials
- Use GitHub secrets for CI/CD credentials
- Regularly update base images
- Scan images for vulnerabilities:
  ```bash
  docker scan ghcr.io/aybarci/midpotapi:latest
  ```
