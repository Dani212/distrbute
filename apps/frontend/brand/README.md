# Brand Portal

A Next.js application for brand/manufacturer users to manage their products and orders.

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

Open [http://localhost:3001](http://localhost:3001) in your browser.

## 📱 Features

- **Dashboard**: Overview of products, orders, and revenue
- **Product Management**: Add, edit, and manage product catalog
- **Order Tracking**: Monitor orders from distributors
- **Analytics**: View performance metrics and insights

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

- **Brand API** (Port 5001): Product and order management
- **Distributor API** (Port 5002): Cross-platform data access
