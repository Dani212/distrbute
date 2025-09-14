# Distrbute - B2B Distribution Platform

A comprehensive monorepo containing a B2B distribution platform that connects brands with distributors through modern web applications and APIs.

## 🏗️ Project Structure

```
distrbute/
├── apps/
│   ├── frontend/
│   │   ├── brand/          # Next.js app for brand management (Port 3001)
│   │   ├── distributor/    # Next.js app for distributor portal (Port 3002)
│   │   └── marketing/      # Next.js app for marketing site (Port 3000)
│   └── backend/
│       ├── brandapi/       # .NET Web API for brand services (Port 5001)
│       └── distributorapi/ # .NET Web API for distributor services (Port 5002)
└── libs/
    ├── ui/                 # Shared UI components and utilities
    └── shared-models/      # Shared data models and contracts
```

## 🚀 Quick Start

### Prerequisites

- **Node.js** 18.0.0 or higher (recommended: 20.x LTS)
- **npm** 8.0.0 or higher
- **.NET 8.0** SDK
- **Git**

### Installation

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd distrbute
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Build shared libraries**
   ```bash
   npm run build:shared-models
   npm run build:ui
   ```

### Running the Applications

#### Frontend Applications

```bash
# Start all frontend apps
npm run dev

# Or start individual apps
npm run dev:brand        # http://localhost:3001
npm run dev:distributor  # http://localhost:3002
npm run dev:marketing    # http://localhost:3000
```

#### Backend APIs

```bash
# Brand API
cd apps/backend/brandapi
dotnet run

# Distributor API (in a new terminal)
cd apps/backend/distributorapi
dotnet run
```

## 📱 Applications Overview

### Marketing Site (Port 3000)

- **Purpose**: Public-facing marketing and landing pages
- **Features**: Hero section, features showcase, pricing, contact
- **Tech Stack**: Next.js 15.5.3, TypeScript 5.7.2, Tailwind CSS 4.1.11

### Brand Portal (Port 3001)

- **Purpose**: Interface for brand/manufacturer users
- **Features**: Product management, order tracking, analytics dashboard
- **Tech Stack**: Next.js 15.5.3, TypeScript 5.7.2, Tailwind CSS 4.1.11, ShadCN UI

### Distributor Portal (Port 3002)

- **Purpose**: Interface for distributor/retailer users
- **Features**: Product catalog, order management, brand discovery
- **Tech Stack**: Next.js 15.5.3, TypeScript 5.7.2, Tailwind CSS 4.1.11, ShadCN UI

### Brand API (Port 5001)

- **Purpose**: Backend services for brand operations
- **Endpoints**: Products, Orders, Brands management
- **Tech Stack**: .NET 8, Entity Framework Core, Swagger

### Distributor API (Port 5002)

- **Purpose**: Backend services for distributor operations
- **Endpoints**: Catalog, Orders, Distributor management
- **Tech Stack**: .NET 8, Entity Framework Core, Swagger

## 📚 Shared Libraries

### @distrbute/ui

Shared UI component library containing:

- **Components**: Button, Input, Card, and other ShadCN components
- **Hooks**: useLocalStorage, useDebounce
- **Utils**: Formatting, validation, utility functions
- **Constants**: API endpoints, routes, validation rules

### @distrbute/shared-models

Shared data models and contracts:

- **TypeScript**: User, Product, Order, Brand, Distributor models
- **Enums**: UserRole, ProductCategory, OrderStatus, BusinessType
- **API Models**: ApiResponse, PaginatedResponse, ValidationResult

## 🛠️ Development

### Available Scripts

```bash
# Development
npm run dev                 # Start all frontend apps
npm run dev:brand          # Start brand app only
npm run dev:distributor    # Start distributor app only
npm run dev:marketing      # Start marketing app only

# Building
npm run build              # Build all projects
npm run build:ui           # Build UI library only
npm run build:shared-models # Build shared models only

# Linting
npm run lint               # Lint all projects

# Cleaning
npm run clean              # Clean all build artifacts
```

### Backend Development

```bash
# Brand API
cd apps/backend/brandapi
dotnet run                 # Start the API
dotnet watch run          # Start with hot reload

# Distributor API
cd apps/backend/distributorapi
dotnet run                 # Start the API
dotnet watch run          # Start with hot reload
```

### Database

Both APIs use in-memory databases for development with seeded data:

- **Brand API**: Products, Orders, Brands
- **Distributor API**: Orders, Distributors

## 🔧 Configuration

### Environment Variables

Create `.env.local` files in each frontend app for environment-specific configuration:

```env
# Brand App (.env.local)
NEXT_PUBLIC_BRAND_API_URL=http://localhost:5001
NEXT_PUBLIC_DISTRIBUTOR_API_URL=http://localhost:5002

# Distributor App (.env.local)
NEXT_PUBLIC_BRAND_API_URL=http://localhost:5001
NEXT_PUBLIC_DISTRIBUTOR_API_URL=http://localhost:5002

# Marketing App (.env.local)
NEXT_PUBLIC_BRAND_API_URL=http://localhost:5001
NEXT_PUBLIC_DISTRIBUTOR_API_URL=http://localhost:5002
```

### API Configuration

The APIs are configured to run on:

- **Brand API**: `https://localhost:5001` (HTTP: `http://localhost:5001`)
- **Distributor API**: `https://localhost:5002` (HTTP: `http://localhost:5002`)

## 📖 API Documentation

### Brand API Endpoints

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order

### Distributor API Endpoints

- `GET /api/catalog` - Get product catalog
- `GET /api/catalog/{productId}` - Get product details
- `GET /api/catalog/search?q={query}` - Search products
- `GET /api/catalog/category/{category}` - Get products by category
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order

## 🧪 Testing

```bash
# Frontend testing
cd apps/frontend/brand
npm test

# Backend testing
cd apps/backend/brandapi
dotnet test
```

## 🚀 Deployment

### Frontend Deployment

Each Next.js app can be deployed independently:

```bash
# Build for production
npm run build

# Start production server
npm run start
```

### Backend Deployment

Each .NET API can be deployed independently:

```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Run published app
dotnet ./publish/BrandApi.dll
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:

- Create an issue in the repository
- Contact the development team
- Check the documentation in each app's README

---

**Happy Coding! 🎉**
