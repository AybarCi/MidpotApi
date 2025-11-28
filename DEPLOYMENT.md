# Deployment Guide with GitHub Actions & GHCR

This guide explains how to deploy the MidpotApi using Docker images built by GitHub Actions and stored in GitHub Container Registry (GHCR).

## Prerequisites

1.  **GitHub Repository**: Ensure your code is pushed to a GitHub repository.
2.  **Server**: A server with Docker and Docker Compose installed.
3.  **Personal Access Token (PAT)**: A GitHub PAT with `read:packages` scope to pull images on the server.

## 1. GitHub Actions Setup

The workflow file `.github/workflows/docker-publish.yml` is already created.
It triggers on every push to `main` or `master` branch.

1.  Push your code to GitHub:
    ```bash
    git remote add origin https://github.com/<YOUR_USERNAME>/<YOUR_REPO>.git
    git push -u origin main
    ```
2.  Go to the "Actions" tab in your GitHub repository to see the build progress.
3.  Once successful, the image will be available at `ghcr.io/<your_username>/<your_repo>/midpotapi:latest`.

## 2. Server Setup

### Login to GHCR
On your remote server, log in to GitHub Container Registry:

```bash
# Replace <USERNAME> with your GitHub username
# When prompted for password, paste your Personal Access Token (PAT)
docker login ghcr.io -u <USERNAME>
```

### Deploying

1.  Copy `docker-compose.prod.yml` and `.env` (or create one) to your server.
2.  Set the `DOCKER_IMAGE` variable in `.env` or export it:

    ```bash
    export DOCKER_IMAGE=ghcr.io/<your_username>/<your_repo>/midpotapi:latest
    ```

3.  Run Docker Compose:

    ```bash
    docker-compose -f docker-compose.prod.yml up -d
    ```

4.  Verify running containers:

    ```bash
    docker-compose -f docker-compose.prod.yml ps
    ```

## 3. Updating Deployment

To update the application after a new code push:

1.  Pull the latest image:
    ```bash
    docker-compose -f docker-compose.prod.yml pull
    ```
2.  Restart containers:
    ```bash
    docker-compose -f docker-compose.prod.yml up -d
    ```
