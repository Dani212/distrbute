# @distrbute/ui

Shared UI component library for all Next.js applications in the Distrbute platform.

## 🚀 Getting Started

### Installation

This library is automatically available in all frontend applications through the workspace configuration.

### Usage

```typescript
import { Button, Card, CardContent, CardHeader, CardTitle } from '@distrbute/ui'
import { useLocalStorage, formatCurrency } from '@distrbute/ui'

// Use components
<Button variant="primary" size="lg">Click me</Button>

<Card>
  <CardHeader>
    <CardTitle>Product Title</CardTitle>
  </CardHeader>
  <CardContent>
    Product content here
  </CardContent>
</Card>

// Use hooks
const [value, setValue] = useLocalStorage('key', 'defaultValue')

// Use utilities
const formattedPrice = formatCurrency(299.99, 'USD')
```

## 📦 Components

### Button

- **Variants**: default, destructive, outline, secondary, ghost, link
- **Sizes**: default, sm, lg, icon
- **Props**: asChild, className, and all standard button props

### Input

- Standard input component with consistent styling
- Supports all HTML input attributes

### Card

- **Card**: Main container
- **CardHeader**: Header section
- **CardTitle**: Title text
- **CardDescription**: Description text
- **CardContent**: Main content area
- **CardFooter**: Footer section

## 🎣 Hooks

### useLocalStorage

Persist state in localStorage with type safety.

```typescript
const [value, setValue] = useLocalStorage<string>("key", "defaultValue");
```

### useDebounce

Debounce values to prevent excessive API calls.

```typescript
const debouncedValue = useDebounce(searchTerm, 500);
```

## 🛠️ Utilities

### Formatting

- `formatCurrency(amount, currency)` - Format currency values
- `formatDate(date)` - Format dates
- `formatDateTime(date)` - Format date and time

### General

- `cn(...inputs)` - Merge class names with Tailwind
- `generateId()` - Generate unique IDs
- `debounce(func, wait)` - Debounce function calls
- `throttle(func, limit)` - Throttle function calls

## 📚 Constants

### API Endpoints

```typescript
API_ENDPOINTS.BRAND_API;
API_ENDPOINTS.DISTRIBUTOR_API;
```

### Routes

```typescript
ROUTES.BRAND.HOME;
ROUTES.DISTRIBUTOR.CATALOG;
ROUTES.MARKETING.ABOUT;
```

### Validation Rules

```typescript
VALIDATION_RULES.EMAIL;
VALIDATION_RULES.PASSWORD_MIN_LENGTH;
```

## 🎨 Styling

The library uses Tailwind CSS with custom design tokens:

- Consistent color palette
- Typography scale
- Spacing system
- Border radius values
- Animation keyframes

## 🔧 Development

### Building

```bash
npm run build
```

### Development Mode

```bash
npm run dev
```

### Cleaning

```bash
npm run clean
```

## 📦 Dependencies

- **React**: Component library
- **class-variance-authority**: Component variants
- **clsx**: Class name utility
- **tailwind-merge**: Tailwind class merging
- **lucide-react**: Icon library
- **@distrbute/shared-models**: Shared data models

## 🎯 Design Principles

- **Consistency**: All components follow the same design patterns
- **Accessibility**: Built with accessibility in mind
- **Type Safety**: Full TypeScript support
- **Performance**: Optimized for minimal bundle size
- **Flexibility**: Customizable through props and CSS classes
