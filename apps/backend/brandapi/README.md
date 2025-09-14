# Brand API

A .NET 8 Web API for brand/manufacturer services including product and order management.

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Installation

```bash
# Navigate to the project directory
cd apps/backend/brandapi

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The API will be available at:

- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger

## 📱 Features

- **Product Management**: CRUD operations for products
- **Order Management**: Track and manage orders from distributors
- **Brand Management**: Brand profile and settings
- **In-Memory Database**: Development database with seeded data
- **Swagger Documentation**: Interactive API documentation

## 🛠️ Available Commands

```bash
dotnet run              # Start the API
dotnet watch run        # Start with hot reload
dotnet build            # Build the project
dotnet test             # Run tests
dotnet clean            # Clean build artifacts
```

## 📚 Dependencies

- **.NET 8**: Web API framework
- **Entity Framework Core**: Data access
- **Swashbuckle.AspNetCore**: API documentation
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database

## 🗄️ Database

The API uses an in-memory database with the following entities:

- **Brands**: Brand information and settings
- **Products**: Product catalog with specifications
- **Orders**: Order management and tracking

### Seeded Data

The database is automatically seeded with sample data:

- 2 sample brands (TechBrand, FashionCorp)
- 3 sample products across different categories
- 1 sample order

## 🔗 API Endpoints

### Products

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `GET /api/products/brand/{brandId}` - Get products by brand

### Orders

- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order
- `DELETE /api/orders/{id}` - Delete order
- `GET /api/orders/brand/{brandId}` - Get orders by brand

## 🔧 Configuration

The API is configured with:

- **CORS**: Enabled for all origins (development only)
- **Swagger**: Available in development mode
- **Entity Framework**: In-memory database provider
- **Dependency Injection**: Services registered for DI

## 🧪 Testing

```bash
# Run tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 Deployment

```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Run published application
dotnet ./publish/BrandApi.dll
```

## 🔒 Security Considerations

- CORS is configured for development
- Input validation on all endpoints
- Entity Framework parameterized queries
- HTTPS redirection enabled
