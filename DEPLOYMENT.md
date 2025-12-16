# SomosRentWi Backend - Deployment Guide

## 🚀 Docker Deployment

### Prerequisites
- Docker installed
- Docker Compose installed
- .env file configured with your credentials

### Build and Run with Docker Compose

```bash
# Build the Docker image
docker-compose build

# Run the application
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop the application
docker-compose down
```

The API will be available at: **http://localhost:5000**

### Build Docker Image Manually

```bash
#Build the image
docker build -t somosrentwi-api .

# Run the container
docker run -d \
  -p 5000:8080 \
  --env-file .env \
  --name somosrentwi-api \
  somosrentwi-api
```

---

## 👤 Admin User Credentials

An admin user has been seeded in the database for testing:

- **Email**: `admin@somosrentwi.com`
- **Password**: `Admin123!`
- **Role**: Admin

Use these credentials to test admin endpoints in Swagger.

---

## 📦 Deploying to Railway/Render/Other Platforms

### Railway

1. Push your code to GitHub
2. Connect Railway to your GitHub repository
3. Add environment variables in Railway dashboard:
   ```
   DB_HOST=your-database-host
   DB_PORT=your-database-port
   DB_USER=your-database-user
   DB_PASSWORD=your-database-password
   DB_NAME=your-database-name
   DB_SSL_MODE=REQUIRED
   JWT_SECRET=your-jwt-secret
   CLOUDINARY_CLOUD_NAME=your-cloud-name
   CLOUDINARY_API_KEY=your-api-key
   CLOUDINARY_API_SECRET=your-api-secret
   ```
4. Railway will automatically detect the Dockerfile and deploy

### Render

1. Push your code to GitHub
2. Create a new Web Service in Render
3. Connect to your GitHub repository
4. Select "Docker" as the runtime
5. Add environment variables in Render dashboard
6. Deploy!

---

## 🗄️ Database Migrations

Before first deployment, run migrations:

```bash
# Create migration (if needed)
dotnet ef migrations add InitialCreate \
  --project src/SomosRentWi.Infrastructure \
  --startup-project src/SomosRentWi.Api

# Apply migrations to database
dotnet ef database update \
  --project src/SomosRentWi.Infrastructure \
  --startup-project src/SomosRentWi.Api
```

The admin user will be automatically seeded when migrations are applied.

---

## 🔧 Environment Variables

Make sure these are set in your deployment platform:

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_HOST` | Database hostname | `somor-rentwi.aivencloud.com` |
| `DB_PORT` | Database port | `28755` |
| `DB_USER` | Database username | `avnadmin` |
| `DB_PASSWORD` | Database password | `your-password` |
| `DB_NAME` | Database name | `defaultdb` |
| `DB_SSL_MODE` | SSL mode | `REQUIRED` |
| `JWT_SECRET` | Secret key for JWT | `your-secret-key-min-32-chars` |
| `CLOUDINARY_CLOUD_NAME` | Cloudinary cloud name | `your-cloud-name` |
| `CLOUDINARY_API_KEY` | Cloudinary API key | `123456789` |
| `CLOUDINARY_API_SECRET` | Cloudinary API secret | `your-api-secret` |

---

## ✅ Testing the Deployment

1. Access Swagger UI: `https://your-deployment-url/swagger`
2. Login with admin credentials
3. Copy the JWT token
4. Click "Authorize" in Swagger
5. Enter: `Bearer {your-token}`
6. Test the endpoints!

---

## 📝 Git Commands for Deployment

```bash
# Add all changes
git add .

# Commit changes
git commit -m "feat: Add Docker configuration and admin seed"

# Push to GitHub
git push origin main
```

Once pushed, your deployment platform (Railway/Render) will automatically detect changes and redeploy.
