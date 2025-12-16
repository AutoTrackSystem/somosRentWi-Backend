#!/bin/sh
# Railway entrypoint script to debug environment variables

echo "=== RAILWAY ENVIRONMENT VARIABLES DEBUG ==="
echo "DB_HOST: ${DB_HOST}"
echo "DB_PORT: ${DB_PORT}"
echo "DB_NAME: ${DB_NAME}"
echo "CLOUDINARY_CLOUD_NAME: ${CLOUDINARY_CLOUD_NAME}"
echo "==========================================="

# Export all Railway variables
export DB_HOST="${DB_HOST}"
export DB_PORT="${DB_PORT}"
export DB_USER="${DB_USER}"
export DB_PASSWORD="${DB_PASSWORD}"
export DB_NAME="${DB_NAME}"
export DB_SSL_MODE="${DB_SSL_MODE}"
export JWT_SECRET="${JWT_SECRET}"
export CLOUDINARY_CLOUD_NAME="${CLOUDINARY_CLOUD_NAME}"
export CLOUDINARY_API_KEY="${CLOUDINARY_API_KEY}"
export CLOUDINARY_API_SECRET="${CLOUDINARY_API_SECRET}"

# Start the application
exec dotnet SomosRentWi.Api.dll
