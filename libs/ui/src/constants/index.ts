export const API_ENDPOINTS = {
  BRAND_API: process.env.NEXT_PUBLIC_BRAND_API_URL || "http://localhost:5001",
  DISTRIBUTOR_API:
    process.env.NEXT_PUBLIC_DISTRIBUTOR_API_URL || "http://localhost:5002",
} as const;

export const ROUTES = {
  BRAND: {
    HOME: "/",
    PRODUCTS: "/products",
    ORDERS: "/orders",
    PROFILE: "/profile",
  },
  DISTRIBUTOR: {
    HOME: "/",
    CATALOG: "/catalog",
    ORDERS: "/orders",
    PROFILE: "/profile",
  },
  MARKETING: {
    HOME: "/",
    ABOUT: "/about",
    CONTACT: "/contact",
    PRICING: "/pricing",
  },
} as const;

export const VALIDATION_RULES = {
  EMAIL: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  PHONE: /^\+?[\d\s\-\(\)]+$/,
  PASSWORD_MIN_LENGTH: 8,
  NAME_MIN_LENGTH: 2,
  NAME_MAX_LENGTH: 50,
} as const;

export const PAGINATION = {
  DEFAULT_PAGE_SIZE: 10,
  MAX_PAGE_SIZE: 100,
} as const;

export const CURRENCIES = {
  USD: "USD",
  EUR: "EUR",
  GBP: "GBP",
  CAD: "CAD",
} as const;

export const ORDER_STATUS_LABELS = {
  PENDING: "Pending",
  CONFIRMED: "Confirmed",
  PROCESSING: "Processing",
  SHIPPED: "Shipped",
  DELIVERED: "Delivered",
  CANCELLED: "Cancelled",
} as const;

export const USER_ROLE_LABELS = {
  BRAND: "Brand",
  DISTRIBUTOR: "Distributor",
  ADMIN: "Admin",
} as const;
