# Marketing Site

A Next.js application for the public-facing marketing website and landing pages.

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

Open [http://localhost:3000](http://localhost:3000) in your browser.

## 📱 Features

- **Landing Page**: Hero section with call-to-action
- **Features Showcase**: Highlight platform capabilities
- **Pricing Information**: Display pricing plans
- **Contact Forms**: Lead generation and support
- **SEO Optimized**: Search engine friendly pages

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

- Button, Card components for consistent styling
- Utility functions for formatting
- Responsive design patterns

## 🔗 Navigation

The marketing site provides links to:

- **Brand Portal**: http://localhost:3001
- **Distributor Portal**: http://localhost:3002
- **About, Pricing, Contact**: Internal pages

## 📈 SEO Features

- Meta tags and descriptions
- Structured data markup
- Optimized images and performance
- Mobile-responsive design
