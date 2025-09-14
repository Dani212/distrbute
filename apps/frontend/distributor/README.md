# Distributor Portal

A Next.js application for distributor/retailer users to browse products and manage orders.

## 🚀 Getting Started

### Prerequisites

- Node.js 18.0.0 or higher
- npm 8.0.0 or higher

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev
```

Open [http://localhost:3002](http://localhost:3002) in your browser.

## 📱 Features

- **Dashboard**: Overview of orders, spending, and favorite brands
- **Product Catalog**: Browse and search products from various brands
- **Order Management**: Place and track orders
- **Brand Discovery**: Find and connect with new brands

## 🛠️ Available Scripts

```bash
npm run dev      # Start development server
npm run build    # Build for production
npm run start    # Start production server
npm run lint     # Run ESLint
npm run clean    # Clean build artifacts
```

## 🔧 Configuration

Create a `.env.local` file for environment variables:

```env
NEXT_PUBLIC_BRAND_API_URL=http://localhost:5001
NEXT_PUBLIC_DISTRIBUTOR_API_URL=http://localhost:5002
```

## 📚 Dependencies

- **Next.js 15.5.3**: React framework with Turbopack
- **TypeScript 5.7.2**: Type safety
- **Tailwind CSS 4.1.11**: Styling
- **@distrbute/ui**: Shared UI components
- **@distrbute/shared-models**: Shared data models

## 🎨 UI Components

The app uses components from the shared UI library:

- Button, Input, Card components
- Custom hooks for state management
- Utility functions for formatting and validation

## 🔗 API Integration

The app integrates with:

- **Distributor API** (Port 5002): Order management and catalog access
- **Brand API** (Port 5001): Product information and cross-platform data
