# GitHub Actions Environment Setup Guide

This guide explains how to set up environments and secrets for the MidpotApi CI/CD pipeline.

## Required Secrets

Add these secrets in your GitHub repository settings (Settings → Secrets and variables → Actions):

### Docker Hub Secrets (for Docker build job)
- `DOCKERHUB_USERNAME` - Your Docker Hub username
- `DOCKERHUB_TOKEN` - Your Docker Hub access token

### Database Connection Secrets (optional)
- `DB_CONNECTION_STRING` - Production database connection string
- `REDIS_CONNECTION_STRING` - Production Redis connection string

### JWT Secrets (optional)
- `JWT_SECRET_KEY` - Secret key for JWT token generation
- `JWT_ISSUER` - JWT token issuer
- `JWT_AUDIENCE` - JWT token audience

## Environment Protection Rules

Set up environments in your repository settings (Settings → Environments):

### Staging Environment
1. Go to Settings → Environments → New environment
2. Name: `staging`
3. Configure protection rules:
   - Required reviewers: Add at least 1 reviewer
   - Deployment branches: `develop` branch only
   - Environment secrets: Add staging-specific secrets

### Production Environment
1. Go to Settings → Environments → New environment
2. Name: `production`
3. Configure protection rules:
   - Required reviewers: Add at least 2 reviewers
   - Deployment branches: `main` branch only
   - Environment secrets: Add production-specific secrets
   - Wait timer: Optional (e.g., 30 minutes)

## Branch Protection Rules

Set up branch protection for `main` and `develop` branches:

1. Go to Settings → Branches → Add branch protection rule
2. Branch name pattern: `main`
3. Enable:
   - Require pull request reviews before merging
   - Require status checks to pass before merging
   - Require branches to be up to date before merging
   - Require conversation resolution before merging
   - Include administrators

4. Repeat for `develop` branch

## Required Status Checks

The following status checks should be required:

### For main branch:
- `build-and-test`
- `security-scan`
- `code-quality`
- `CodeQL Security Analysis`
- `dependency-review` (for PRs)

### For develop branch:
- `build-and-test`
- `security-scan`
- `code-quality`

## Monitoring

### GitHub Actions Usage
Monitor your GitHub Actions usage to avoid exceeding limits:
- Go to Settings → Actions → Usage
- Set up spending limits if needed

### Workflow Notifications
Configure notifications for workflow failures:
1. Go to Settings → Notifications
2. Enable notifications for Actions
3. Add email addresses for critical alerts

## Troubleshooting

### Common Issues

1. **Docker Hub Authentication Failed**
   - Ensure DOCKERHUB_TOKEN is valid and has proper permissions
   - Check if the token has read/write access to repositories

2. **Database Connection Failures**
   - Verify connection strings are correct
   - Ensure database server allows GitHub Actions IP ranges
   - Check firewall settings

3. **Build Failures**
   - Check .NET SDK version compatibility
   - Verify all NuGet packages are available
   - Review build logs for specific errors

4. **CodeQL Analysis Taking Too Long**
   - This is normal for initial runs
   - Subsequent runs will be faster due to caching
   - Consider adjusting analysis frequency if needed

## Security Best Practices

1. **Secret Management**
   - Never commit secrets to code
   - Use GitHub Secrets for all sensitive data
   - Rotate secrets regularly

2. **Access Control**
   - Limit who can approve deployments
   - Use environment protection rules
   - Monitor deployment activities

3. **Audit Logging**
   - Enable audit logging for your repository
   - Regularly review deployment logs
   - Monitor for suspicious activities

## Support

For issues with GitHub Actions workflows:
1. Check the [GitHub Actions documentation](https://docs.github.com/en/actions)
2. Review workflow logs in the Actions tab
3. Contact your repository administrators