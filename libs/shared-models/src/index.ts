// User Models
export interface User {
  id: string;
  email: string;
  name: string;
  role: UserRole;
  createdAt: Date;
  updatedAt: Date;
}

export enum UserRole {
  BRAND = "brand",
  DISTRIBUTOR = "distributor",
  ADMIN = "admin",
}

// Product Models
export interface Product {
  id: string;
  name: string;
  description: string;
  sku: string;
  price: number;
  currency: string;
  brandId: string;
  category: ProductCategory;
  images: string[];
  specifications: Record<string, any>;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export enum ProductCategory {
  ELECTRONICS = "electronics",
  CLOTHING = "clothing",
  HOME_GARDEN = "home_garden",
  SPORTS = "sports",
  BEAUTY = "beauty",
  AUTOMOTIVE = "automotive",
}

// Order Models
export interface Order {
  id: string;
  distributorId: string;
  brandId: string;
  items: OrderItem[];
  totalAmount: number;
  currency: string;
  status: OrderStatus;
  shippingAddress: Address;
  billingAddress: Address;
  createdAt: Date;
  updatedAt: Date;
}

export interface OrderItem {
  productId: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export enum OrderStatus {
  PENDING = "pending",
  CONFIRMED = "confirmed",
  PROCESSING = "processing",
  SHIPPED = "shipped",
  DELIVERED = "delivered",
  CANCELLED = "cancelled",
}

// Address Models
export interface Address {
  id: string;
  street: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  isDefault: boolean;
}

// Brand Models
export interface Brand {
  id: string;
  name: string;
  description: string;
  logo: string;
  website: string;
  contactEmail: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

// Distributor Models
export interface Distributor {
  id: string;
  name: string;
  description: string;
  logo: string;
  website: string;
  contactEmail: string;
  businessType: BusinessType;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export enum BusinessType {
  RETAILER = "retailer",
  WHOLESALER = "wholesaler",
  ECOMMERCE = "ecommerce",
  B2B = "b2b",
}

// API Response Models
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  pagination: {
    page: number;
    limit: number;
    total: number;
    totalPages: number;
  };
}

// Validation Models
export interface ValidationError {
  field: string;
  message: string;
  code: string;
}

export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
}
