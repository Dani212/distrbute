/**
 * Authentication constants used throughout the application
 * Centralized constants to avoid hardcoded values and prevent typos
 */
export const AUTH = {
  // Authentication providers
  PROVIDERS: {
    OTP: "otp",
    // Add other providers as needed
    // GOOGLE: "google",
    // GITHUB: "github",
  },

  // Authentication options
  OPTIONS: {
    REDIRECT: false,
    // Add other auth options as needed
  },

  // OTP specific constants
  OTP: {
    LENGTH: 6,
    RESEND_COUNTDOWN_SECONDS: 60,
    INPUT_SLOTS: Array.from({ length: 6 }, (_, i) => i),
  },

  // Session keys
  SESSION_KEYS: {
    OTP_DATA: "otpData",
    // Add other session keys as needed
  },

  // Credentials configuration
  CREDENTIALS: {
    EMAIL: { label: "Email", type: "email" },
    NAME: { label: "Name", type: "text" },
    IMAGE: { label: "Image", type: "text" },
    ACCESS_TOKEN: { label: "Access Token", type: "text" },
    EXPIRATION_MILLIS: { label: "Expiration Millis", type: "text" },
    ACCOUNT_CREATED: { label: "Account Created", type: "text" },
    PROFILE_PICTURE: { label: "Profile Picture", type: "object" },
  },

  // Pages configuration
  PAGES: {
    SIGN_IN: "/auth",
    ERROR: "/auth",
  },

  // Session configuration
  SESSION: {
    STRATEGY: "jwt" as const,
    MAX_AGE_DAYS: 6.9,
    MAX_AGE_SECONDS: 6.9 * 24 * 60 * 60, // 6.9 days in seconds
  },

  // API response codes
  API_CODES: {
    SUCCESS: 200,
  },
} as const;

// /**
//  * Type for auth constants to ensure type safety
//  */
// export type AuthProvider = (typeof AUTH.PROVIDERS)[keyof typeof AUTH.PROVIDERS];
// export type AuthOption = (typeof AUTH.OPTIONS)[keyof typeof AUTH.OPTIONS];
// export type SessionKey =
//   (typeof AUTH.SESSION_KEYS)[keyof typeof AUTH.SESSION_KEYS];

// /**
//  * Helper function to get auth provider
//  */
// export const getAuthProvider = (provider: AuthProvider) =>
//   AUTH.PROVIDERS[provider];

// /**
//  * Helper function to get auth options
//  */
// export const getAuthOptions = () => AUTH.OPTIONS;
