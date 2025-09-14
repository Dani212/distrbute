# @distrbute/shared-models

Shared data models and contracts used across frontend and backend applications.

## 🚀 Getting Started

### Installation

This library is automatically available in all applications through the workspace configuration.

### Usage

```typescript
import {
  User,
  Product,
  Order,
  UserRole,
  OrderStatus,
} from "@distrbute/shared-models";

// Use in TypeScript
const user: User = {
  id: "1",
  email: "user@example.com",
  name: "John Doe",
  role: UserRole.BRAND,
  createdAt: new Date(),
  updatedAt: new Date(),
};

// Use in React components
const [products, setProducts] = useState<Product[]>([]);
```

## 📦 Models

### User Models

- **User**: User account information
- **UserRole**: Enum for user roles (BRAND, DISTRIBUTOR, ADMIN)

### Product Models

- **Product**: Product information and specifications
- **ProductCategory**: Enum for product categories

### Order Models

- **Order**: Order information and items
- **OrderItem**: Individual order line items
- **OrderStatus**: Enum for order statuses

### Address Models

- **Address**: Shipping and billing addresses

### Brand Models

- **Brand**: Brand/manufacturer information

### Distributor Models

- **Distributor**: Distributor/retailer information
- **BusinessType**: Enum for business types

### API Response Models

- **ApiResponse<T>**: Standard API response wrapper
- **PaginatedResponse<T>**: Paginated data response
- **ValidationError**: Validation error details
- **ValidationResult**: Validation result wrapper

## 🔧 TypeScript Configuration

The library is built with TypeScript and includes:

- Type definitions for all models
- Enum definitions for constants
- Interface definitions for contracts
- Generic types for API responses

## 🛠️ Development

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

- **TypeScript**: Type definitions and compilation
- **@types/node**: Node.js type definitions

## 🎯 Design Principles

- **Type Safety**: All models are strongly typed
- **Consistency**: Shared models ensure data consistency across apps
- **Validation**: Models include validation-friendly structure
- **Extensibility**: Easy to extend with new fields and models
- **Documentation**: Well-documented interfaces and enums

## 🔄 Cross-Platform Usage

### Frontend (TypeScript/JavaScript)

```typescript
import { User, Product, OrderStatus } from "@distrbute/shared-models";

// Use in React components
const [user, setUser] = useState<User | null>(null);
const [orders, setOrders] = useState<Order[]>([]);

// Use in API calls
const response: ApiResponse<Product[]> = await fetch("/api/products");
```

### Backend (C#)

The C# models should mirror the TypeScript models for consistency:

```csharp
// C# equivalent models
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum UserRole
{
    Brand,
    Distributor,
    Admin
}
```

## 📝 Notes

- Models are designed to be serializable for API communication
- All dates are represented as JavaScript Date objects in TypeScript
- Enums use string values for better API compatibility
- Optional fields are marked with `?` in TypeScript interfaces
