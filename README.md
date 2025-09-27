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
│       ├── App.Distrbute.Common/        # Shared .NET models and database context
│       ├── App.Distrbute.Api.Common/    # Common API utilities and base controllers
│       └── App.Distrbute.Distributor.Api/ # .NET Web API for distributor services (Port 4011)
└── libs/
    └── next-shared/        # Shared UI components and utilities (@distrbute/next-shared)
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
# Distributor API
cd apps/backend/App.Distrbute.Distributor.Api
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

### Distributor API (Port 4011)

- **Purpose**: Backend services for distributor operations
- **Endpoints**: Authentication, Distributor management, Social accounts, Wallet, Ads, Static data
- **Tech Stack**: .NET 8, Entity Framework Core, Swagger
- **Features**: JWT authentication, wallet management, social media integration

## 📚 Shared Libraries

### @distrbute/next-shared

Shared UI component library containing:

- **Components**: Button, Input, Card, and other ShadCN components
- **Hooks**: useLocalStorage, useDebounce
- **Utils**: Formatting, validation, utility functions
- **Constants**: API endpoints, routes, validation rules
- **Types**: TypeScript interfaces and types for the application

## 🛠️ Development

### Available Scripts

```bash
# Development
npm run dev                 # Start all frontend apps in parallel
npm run dev:brand          # Start brand app only (Port 3001)
npm run dev:distributor    # Start distributor app only (Port 3002)
npm run dev:marketing      # Start marketing app only (Port 3000)

# Building
npm run build              # Build all projects (shared libs first, then apps)
npm run build:ui           # Build shared UI library only
npm run build:shared-models # Build shared models only
npm run build:brand        # Build brand app only
npm run build:distributor  # Build distributor app only
npm run build:marketing    # Build marketing app only

# Linting
npm run lint               # Lint all frontend projects
npm run lint:brand         # Lint brand app only
npm run lint:distributor   # Lint distributor app only
npm run lint:marketing     # Lint marketing app only

# Cleaning
npm run clean              # Clean all build artifacts
npm run clean:brand        # Clean brand app only
npm run clean:distributor  # Clean distributor app only
npm run clean:marketing    # Clean marketing app only
npm run clean:shared-models # Clean shared library only
```

### Backend Development

```bash
# Distributor API
cd apps/backend/App.Distrbute.Distributor.Api
dotnet run                 # Start the API (Port 4011)
dotnet watch run          # Start with hot reload
```

### Database

The API uses Entity Framework Core with SQL Server:

- **Models**: Brand, Distributor, Campaign, Post, Wallet, Transaction, and more
- **Features**: Migrations, relationships, data validation
- **Context**: ApplicationDbContext with comprehensive entity configuration

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

The API is configured to run on:

- **Distributor API**: `http://localhost:4011`
- **Swagger Documentation**: `http://localhost:4011/swagger`

## 📖 API Documentation

### Distributor API Endpoints

#### Authentication

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh` - Refresh JWT token

#### Distributor Management

- `GET /api/distributor` - Get distributor profile
- `PUT /api/distributor` - Update distributor profile
- `GET /api/distributor/niches` - Get distributor niches

#### Social Accounts

- `GET /api/socials` - Get social media accounts
- `POST /api/socials` - Add social media account
- `PUT /api/socials/{id}` - Update social account
- `DELETE /api/socials/{id}` - Remove social account

#### Wallet & Transactions

- `GET /api/wallet` - Get wallet information
- `GET /api/wallet/transactions` - Get transaction history
- `POST /api/wallet/transactions` - Create new transaction

#### Static Data

- `GET /api/static/niches` - Get available niches
- `GET /api/static/locations` - Get location data

#### Webhooks

- `POST /api/webhooks` - Handle webhook events

Visit `http://localhost:4011/swagger` for complete API documentation.

## 🧪 Testing

```bash
# Frontend testing
cd apps/frontend/brand
npm test

# Backend testing
cd apps/backend/App.Distrbute.Distributor.Api
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
dotnet ./publish/App.Distrbute.Distributor.Api.dll
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
