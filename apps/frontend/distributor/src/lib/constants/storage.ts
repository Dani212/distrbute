/**
 * Storage keys used throughout the application
 * Centralized constants to avoid hardcoded strings and prevent typos
 */
export const STORAGE_KEYS = {
  OTP_DATA: "otpData",
  // Add other storage keys as needed
  // USER_DATA: 'userData',
  // AUTH_TOKEN: 'authToken',
  // PREFERENCES: 'preferences',
} as const;

/**
 * Type for storage keys to ensure type safety
 */
export type StorageKey = (typeof STORAGE_KEYS)[keyof typeof STORAGE_KEYS];

/**
 * OTP data structure stored in sessionStorage
 */
export interface OtpData {
  verificationId: string;
  requestId: string;
  otpPrefix: string;
  email: string;
  name?: string; // Optional for login flow
}
