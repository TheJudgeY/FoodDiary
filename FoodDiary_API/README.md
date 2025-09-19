# 🍽️ FoodDiary API – Backend Services

A robust ASP.NET Core 9.0 API built with Clean Architecture principles, providing comprehensive nutrition tracking and analytics services.

---

## 🏗️ Architecture

### Clean Architecture Layers

* **Core**: Domain entities, interfaces, and business rules
* **Infrastructure**: Data access, external services, and implementations
* **UseCases**: Business logic and application services
* **Web**: API controllers, endpoints, and configuration

### Key Technologies

* **ASP.NET Core 9.0** – Web API framework
* **Entity Framework Core** – ORM with PostgreSQL
* **MediatR** – CQRS pattern implementation
* **JWT Authentication** – Secure token‑based auth
* **Serilog** – Structured logging
* **Swagger/OpenAPI** – API documentation

---

## 🚀 Quick Start

### Prerequisites

* **.NET 9.0 SDK**
* **PostgreSQL 13+**
* **Git**

### 1) Configuration Setup

```bash
cd src/FoodDiary.Web

# Copy configuration templates
cp appsettings.template.json appsettings.json
cp appsettings.Development.template.json appsettings.Development.json
```

### 2) Database Configuration

Update `appsettings.json` with your database and security settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=YOUR_DATABASE_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;"
  },
  "JwtSettings": {
    "Secret": "YOUR_SUPER_SECRET_JWT_KEY_MINIMUM_32_CHARACTERS_LONG",
    "Issuer": "FoodDiaryApi",
    "Audience": "FoodDiaryApiUsers",
    "ExpiresInMinutes": 60
  },
  "Mailserver": {
    "Hostname": "YOUR_SMTP_SERVER",
    "Port": 587,
    "Username": "YOUR_EMAIL_USERNAME",
    "Password": "YOUR_EMAIL_PASSWORD",
    "EnableSsl": true
  }
}
```

### 3) Database Migration

```bash
# Restore packages
 dotnet restore

# Apply migrations (run from the Web project folder or pass --project)
 dotnet ef database update
```

### 4) Run the API

```bash
# Development mode
 dotnet run --environment Development

# Production mode
 dotnet run --environment Production
```

**API Endpoints:**

* **Base URL**: [http://localhost:5001](http://localhost:5001)
* **Swagger UI**: [http://localhost:5001/swagger](http://localhost:5001/swagger)
* **Health Check**: [http://localhost:5001/health](http://localhost:5001/health)

---

## 📊 API Features

### Authentication & Authorization

* **POST** `/api/auth/register` – User registration
* **POST** `/api/auth/login` – User login
* **POST** `/api/auth/refresh` – Token refresh
* **POST** `/api/auth/logout` – User logout

### Food Management

* **GET** `/api/food-entries` – List food entries
* **POST** `/api/food-entries` – Create food entry
* **PUT** `/api/food-entries/{id}` – Update food entry
* **DELETE** `/api/food-entries/{id}` – Delete food entry

### Product Catalog

* **GET** `/api/products` – List products
* **GET** `/api/products/{id}` – Get product details
* **POST** `/api/products` – Create product
* **PUT** `/api/products/{id}` – Update product
* **DELETE** `/api/products/{id}` – Delete product

### Recipe Management

* **GET** `/api/recipes` – List recipes
* **GET** `/api/recipes/{id}` – Get recipe details
* **POST** `/api/recipes` – Create recipe
* **PUT** `/api/recipes/{id}` – Update recipe
* **DELETE** `/api/recipes/{id}` – Delete recipe

### Analytics & Insights

* **GET** `/api/analytics/trends` – Get nutrition trends
* **GET** `/api/analytics/daily` – Get daily analysis
* **GET** `/api/analytics/recommendations` – Get recommendations

### User Management

* **GET** `/api/users/profile` – Get user profile
* **PUT** `/api/users/profile` – Update user profile
* **GET** `/api/users/body-metrics` – Get body metrics
* **PUT** `/api/users/body-metrics` – Update body metrics

---

## 🔧 Configuration

### Required Environment Values (placeholders to replace)

#### Database Configuration

* `YOUR_DATABASE_NAME` – PostgreSQL database name
* `YOUR_USERNAME` – PostgreSQL username
* `YOUR_PASSWORD` – PostgreSQL password

#### Security Configuration

* `YOUR_SUPER_SECRET_JWT_KEY_MINIMUM_32_CHARACTERS_LONG` – JWT secret (32+ characters)

#### Email Configuration

* `YOUR_SMTP_SERVER` – SMTP server hostname
* `YOUR_EMAIL_USERNAME` – Email username
* `YOUR_EMAIL_PASSWORD` – Email password

### Launch Profiles

* **http** – Development (`localhost:5001`)
* **http-mobile** – Mobile development (`0.0.0.0:5001`)
* **https** – Production with SSL

---

## 🧪 Testing

### Run All Tests

```bash
cd FoodDiary_API
 dotnet test
```

### Test Categories

* **Unit Tests** – Individual component testing
* **Integration Tests** – Database and service integration
* **Functional Tests** – End‑to‑end API testing

### Test Coverage

* ✅ **Authentication flows**
* ✅ **CRUD operations**
* ✅ **Business logic validation**
* ✅ **Error handling**
* ✅ **Data mapping**

---

## 📁 Project Structure

```
FoodDiary_API/
├── src/
│   ├── FoodDiary.Core/           # Domain layer
│   │   ├── AnalyticsAggregate/   # Analytics domain models
│   │   ├── FoodEntryAggregate/   # Food entry domain models
│   │   ├── ProductAggregate/     # Product domain models
│   │   ├── RecipeAggregate/      # Recipe domain models
│   │   ├── UserAggregate/        # User domain models
│   │   └── Interfaces/           # Domain interfaces
│   ├── FoodDiary.Infrastructure/ # Infrastructure layer
│   │   ├── Data/                 # Entity Framework context
│   │   ├── Auth/                 # JWT authentication
│   │   ├── Email/                # Email services
│   │   └── Services/             # External service implementations
│   ├── FoodDiary.UseCases/       # Application layer
│   │   ├── Auth/                 # Authentication use cases
│   │   ├── Analytics/            # Analytics use cases
│   │   ├── FoodEntries/          # Food entry use cases
│   │   ├── Products/             # Product use cases
│   │   ├── Recipes/              # Recipe use cases
│   │   └── Users/                # User use cases
│   └── FoodDiary.Web/            # Presentation layer
│       ├── Auth/                 # Authentication endpoints
│       ├── Analytics/            # Analytics endpoints
│       ├── FoodEntries/          # Food entry endpoints
│       ├── Products/             # Product endpoints
│       ├── Recipes/              # Recipe endpoints
│       └── Users/                # User endpoints
└── tests/                        # Test projects
    ├── FoodDiary.UnitTests/        # Unit tests
    ├── FoodDiary.IntegrationTests/ # Integration tests
    └── FoodDiary.FunctionalTests/  # Functional tests
```

---

## 🔒 Security Features

### Authentication

* **JWT Bearer tokens** with configurable expiration
* **Password hashing** using BCrypt
* **Email confirmation** for new registrations
* **Refresh token** support

### Authorization

* **Role‑based access control** *(future enhancement)*
* **Resource ownership validation**
* **API endpoint protection**

### Data Protection

* **Input validation** using FluentValidation
* **SQL injection prevention** via Entity Framework
* **CORS configuration** for frontend integration
* **Sensitive data encryption** in configuration

---

## 📈 Performance & Monitoring

### Logging

* **Structured logging** with Serilog
* **Console and file output** (configurable)
* **Request/response logging**
* **Error tracking and monitoring**

### Health Checks

* **Database connectivity**
* **External service health**
* **Memory and performance metrics**

### Caching

* **In‑memory caching** for frequently accessed data
* **Query optimization** with Entity Framework
* **Response compression** for large payloads

---

## 🚀 Deployment

### Development

```bash
 dotnet run --environment Development
```

### Production

```bash
 dotnet publish -c Release -o ./publish
 dotnet ./publish/FoodDiary.Web.dll --environment Production
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY ./publish /app
WORKDIR /app
EXPOSE 5000
ENTRYPOINT ["dotnet", "FoodDiary.Web.dll"]
```

---

## 🔧 Troubleshooting

### Common Issues

#### Database Connection

* **Check PostgreSQL service**: `pg_isready`
* **Verify connection string**: Test with `psql`
* **Check firewall**: Ensure port 5432 is accessible

#### Authentication Issues

* **JWT secret length**: Must be 32+ characters
* **Token expiration**: Check `ExpiresInMinutes` setting
* **CORS configuration**: Verify `FrontendUrl` (if used) and allowed origins

#### Build Issues

* **.NET version**: Ensure .NET 9.0 SDK is installed
* **Package restore**: Run `dotnet restore`
* **Clean build**: Try `dotnet clean && dotnet build`

### Debugging

* **Enable detailed logging**: Set log level to `Debug`
* **Check application logs**: Review configured log sinks
* **Use Swagger UI**: Test endpoints interactively
* **Database inspection**: Use pgAdmin or similar tool

---

## 📚 API Documentation

### Swagger/OpenAPI

When running in development mode, visit:

* **Swagger UI**: [http://localhost:5001/swagger](http://localhost:5001/swagger)
* **OpenAPI JSON**: [http://localhost:5001/swagger/v1/swagger.json](http://localhost:5001/swagger/v1/swagger.json)

### Authentication

All protected endpoints require a valid JWT token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Response Format

All API responses follow a consistent format (example):

```json
{
  "success": true,
  "data": { },
  "message": "Operation completed successfully",
  "errors": []
}
```

---

## 🤝 Contributing

### Development Workflow

1. **Fork the repository**
2. **Create feature branch**:

   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Follow coding standards**: Clean Architecture principles
4. **Write tests**: Ensure coverage for new features
5. **Run quality checks**:

   ```bash
   dotnet test && dotnet build
   ```
6. **Submit pull request**

### Code Quality Standards

* ✅ **Clean Architecture compliance**
* ✅ **Unit test coverage**
* ✅ **Integration test coverage**
* ✅ **Code documentation**
* ✅ **Error handling**
* ✅ **Input validation**

---

## 📄 License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.
