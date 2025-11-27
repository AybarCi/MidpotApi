# MidpotApi - Dating Application Backend

[![.NET CI/CD Pipeline](https://github.com/AybarCi/MidpotApi/actions/workflows/dotnet-ci-cd.yml/badge.svg)](https://github.com/AybarCi/MidpotApi/actions/workflows/dotnet-ci-cd.yml)
[![CodeQL Security Analysis](https://github.com/AybarCi/MidpotApi/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/AybarCi/MidpotApi/actions/workflows/codeql-analysis.yml)
[![Dependency Review](https://github.com/AybarCi/MidpotApi/actions/workflows/dependency-review.yml/badge.svg)](https://github.com/AybarCi/MidpotApi/actions/workflows/dependency-review.yml)

**Last Updated**: January 2025

## Overview

MidpotApi is a comprehensive dating application backend built with .NET 7.0, featuring real-time chat, location-based matching, Redis caching, and PostgreSQL database. Now with enhanced security through automated CodeQL analysis.

## Overview

MidpotApi is a comprehensive dating application backend built with .NET 7.0, featuring real-time chat, location-based matching, Redis caching, and PostgreSQL database.

## Features

- 🔐 **Authentication & Authorization** - JWT-based authentication
- 💬 **Real-time Chat** - SignalR integration for instant messaging
- 📍 **Location-based Matching** - GPS coordinates for nearby user matching
- 🔴 **Redis Caching** - High-performance caching for better response times
- 🗄️ **PostgreSQL Database** - Robust data storage with advanced queries
- 📱 **Mobile-First Design** - Optimized for React Native mobile application
- 🚀 **Docker Support** - Containerized deployment ready
- 🔒 **Security** - CodeQL analysis and dependency vulnerability scanning

## Tech Stack

- **Framework**: .NET 7.0
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis (StackExchange.Redis)
- **Real-time**: SignalR
- **Authentication**: JWT Bearer tokens
- **Containerization**: Docker
- **CI/CD**: GitHub Actions

## Prerequisites

- .NET 7.0 SDK
- PostgreSQL 12+
- Redis 6+
- Docker (optional)

## Getting Started

### Local Development

1. Clone the repository:
```bash
git clone https://github.com/AybarCi/MidpotApi.git
cd MidpotApi
```

2. Install dependencies:
```bash
dotnet restore
```

3. Configure connection strings in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "PostgreConnection": "Host=localhost;Database=midpot;Username=postgres;Password=your_password"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

4. Run database migrations:
```bash
dotnet ef database update
```

5. Start the application:
```bash
dotnet run --urls "http://localhost:5050"
```

6. Access Swagger UI: http://localhost:5050/swagger/index.html

### Docker Deployment

#### Production Deployment (GitHub Container Registry)

**Recommended for production**: Use pre-built images from GitHub Container Registry:

```bash
# 1. Copy environment template
cp .env.example .env

# 2. Edit .env with your production values
nano .env

# 3. Pull and start services
docker-compose pull
docker-compose up -d

# OR use production compose file
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d
```

**Available image tags:**
- `ghcr.io/aybarci/midpotapi:latest` - Latest production build (main branch)
- `ghcr.io/aybarci/midpotapi:develop` - Latest staging build (develop branch)
- `ghcr.io/aybarci/midpotapi:main-<sha>` - Specific production commit
- `ghcr.io/aybarci/midpotapi:develop-<sha>` - Specific staging commit

See [GHCR_DEPLOYMENT.md](GHCR_DEPLOYMENT.md) for detailed deployment guide.

#### Local Development (Build from Source)

1. **Setup Environment Variables:**
```bash
# Copy the example environment file
cp .env.example .env

# Edit .env with your production values
# IMPORTANT: Change all default passwords and keys!
nano .env
```

2. **Start Services:**
```bash
# Start all services (PostgreSQL, Redis, API)
docker-compose up -d

# View logs
docker-compose logs -f midpot-api

# Check service health
docker-compose ps
```

3. **Access the Application:**
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Health Check: http://localhost:5000/health

4. **Stop Services:**
```bash
docker-compose down
```

#### Production Deployment

For production deployment with Nginx reverse proxy and SSL:

1. **Add SSL Certificates:**
```bash
mkdir ssl
cp your_certificate.pem ssl/cert.pem
cp your_private_key.pem ssl/key.pem
chmod 600 ssl/key.pem
```

2. **Deploy with Nginx:**
```bash
docker-compose --profile with-proxy up -d
```

See [DOCKER_DEPLOYMENT_GUIDE.md](DOCKER_DEPLOYMENT_GUIDE.md) for detailed production deployment instructions.

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh` - Refresh JWT token

### Users
- `GET /api/user/profile/{id}` - Get user profile
- `PUT /api/user/profile` - Update user profile
- `GET /api/user/settings` - Get user settings
- `PUT /api/user/settings` - Update user settings

### Matches
- `GET /api/match/getmatches` - Get potential matches
- `POST /api/match/like` - Like a user
- `POST /api/match/dislike` - Dislike a user

### Chat
- `GET /api/chat/conversations` - Get user conversations
- `GET /api/chat/messages/{conversationId}` - Get conversation messages
- `POST /api/chat/send` - Send message (WebSocket via SignalR)

## Database Schema

The application uses PostgreSQL with the following main entities:
- **ApplicationUsers** - User profiles and authentication
- **Matches** - User matching data
- **Messages** - Chat messages
- **Conversations** - Chat conversations
- **UserSettings** - User preferences and settings

## Redis Caching

The application implements Redis caching for improved performance:
- User profiles are cached with 15-minute TTL
- Match results are cached with 15-minute TTL
- Cache invalidation occurs on profile updates

## CI/CD Pipeline

GitHub Actions workflows provide:
- ✅ **Build & Test** - Automated testing with PostgreSQL and Redis
- 🔒 **Security Scanning** - CodeQL analysis and vulnerability detection
- 📊 **Code Quality** - Format checking and static analysis
- 🐳 **Docker Build** - Automated container image creation
- 🚀 **Deployment** - Staging and production deployment (configurable)

## Environment Variables

Create a `.env` file based on `.env.example`:

### Required Variables (Production)

```bash
# Database
POSTGRES_DB=socialdb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_production_password

# JWT Authentication
JWT_KEY=your_super_secure_jwt_key_minimum_64_characters_required
JWT_ISSUER=https://www.midpot.net
JWT_AUDIENCE=https://www.midpot.net
JWT_LIFETIME=48
JWT_REFRESH_LIFETIME=360

# Security
SECURITY_KEY=your_security_key_for_encryption

# Distance Configuration
DISTANCE_LIMIT=100

# Redis Cache
REDIS_DEFAULT_EXPIRY=60
REDIS_USER_PROFILE_EXPIRY=120
REDIS_MATCH_EXPIRY=30
REDIS_LOCATION_EXPIRY=15
```

### Optional Variables

```bash
# Azure Blob Storage (for file uploads)
AZURE_BLOB_STORAGE=DefaultEndpointsProtocol=https;AccountName=your_account;AccountKey=your_key;EndpointSuffix=core.windows.net

# Firebase Cloud Messaging (for push notifications)
FCM_SENDER_ID=your_sender_id
FCM_SERVER_KEY=your_server_key

# SMS Service (for verification)
SMS_SERVICE_URL=https://api.vatansms.net/api/v1/1toN
SMS_USER_ID=your_sms_user_id
SMS_PASSWORD=your_sms_password
SMS_SENDER=YourAppName

# Database Migrations
RUN_MIGRATIONS=false  # Set to 'true' to auto-run migrations on container startup
```

**Security Note:** Never commit the `.env` file to version control!

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, email support@midpot.com or join our Slack channel.# Mon Nov 10 10:31:33 +03 2025: Trigger GitHub Actions workflow
