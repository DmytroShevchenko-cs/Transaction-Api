# Transaction API Deployment Instructions

## Prerequisites
- Docker Desktop installed and running
- PostgreSQL database available

## Building Docker Image

### Option 1: Using bat file
```bash
build.bat
```

### Option 2: Direct Docker command
```bash
docker build -t transaction-api .
```

## Running Container

### Basic run
```bash
docker run -p 8080:8080 transaction-api
```

### Run with environment variables
```bash
docker run -p 8080:8080 -e CONNECTION_STRING="Host=host.docker.internal;Database=TransactionDb;Username=postgres;Password=verysecurepass" transaction-api
```

### Run in background
```bash
docker run -d -p 8080:8080 --name transaction-api-container transaction-api
```

## Verification

After starting the container:
1. Open your browser
2. Navigate to: http://localhost:8080
3. You should see Swagger UI with API documentation

## Useful Commands

### View logs
```bash
docker logs transaction-api-container
```

### Stop container
```bash
docker stop transaction-api-container
```

### Remove container
```bash
docker rm transaction-api-container
```

### Remove image
```bash
docker rmi transaction-api
```

## Environment Variables

- `CONNECTION_STRING` - PostgreSQL connection string
- `ASPNETCORE_ENVIRONMENT` - Environment (Production/Development)
- `ASPNETCORE_URLS` - Application URL

## Troubleshooting

### If application doesn't start:
1. Check logs: `docker logs <container_name>`
2. Ensure database is available
3. Verify connection string

### If port is busy:
Change port in run command:
```bash
docker run -p 8081:8080 transaction-api
```

