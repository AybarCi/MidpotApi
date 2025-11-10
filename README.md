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

```bash
# Build Docker image
docker build -t midpotapi .

# Run with Docker Compose (recommended)
docker-compose up -d
```

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

### Development
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__PostgreConnection=Host=localhost;Database=midpot;Username=postgres;Password=password
Redis__ConnectionString=localhost:6379
JWT__SecretKey=your-secret-key
JWT__Issuer=MidpotApi
JWT__Audience=MidpotUsers
```

### Production
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__PostgreConnection=Host=prod-server;Database=midpot;Username=postgres;Password=secure-password
Redis__ConnectionString=redis-prod-server:6379
JWT__SecretKey=very-secure-secret-key
JWT__Issuer=MidpotApi
JWT__Audience=MidpotUsers
```

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
