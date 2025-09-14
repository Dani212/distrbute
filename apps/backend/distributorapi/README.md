# Distributor API

A .NET 8 Web API for distributor/retailer services including catalog access and order management.

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Installation

```bash
# Navigate to the project directory
cd apps/backend/distributorapi

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The API will be available at:

- **HTTPS**: https://localhost:5002
- **HTTP**: http://localhost:5002
- **Swagger UI**: https://localhost:5002/swagger

## 📱 Features

- **Catalog Management**: Browse and search products from brands
- **Order Management**: Place and track orders
- **Distributor Management**: Distributor profile and settings
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

- **Distributors**: Distributor information and settings
- **Orders**: Order management and tracking

### Seeded Data

The database is automatically seeded with sample data:

- 2 sample distributors (Tech Distributors Inc., Retail Solutions Ltd.)
- 2 sample orders with different statuses

## 🔗 API Endpoints

### Catalog

- `GET /api/catalog` - Get product catalog
- `GET /api/catalog/{productId}` - Get product details
- `GET /api/catalog/search?q={query}` - Search products
- `GET /api/catalog/category/{category}` - Get products by category

### Orders

- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order
- `DELETE /api/orders/{id}` - Delete order
- `GET /api/orders/distributor/{distributorId}` - Get orders by distributor

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
dotnet ./publish/DistributorApi.dll
```

## 🔒 Security Considerations

- CORS is configured for development
- Input validation on all endpoints
- Entity Framework parameterized queries
- HTTPS redirection enabled

## 📝 Notes

- The catalog service currently returns mock data
- In a production environment, this would integrate with the Brand API
- Product data is shared between Brand and Distributor APIs through the shared models
