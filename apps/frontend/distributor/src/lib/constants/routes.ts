/**
 * Route constants used throughout the application
 * Centralized constants to avoid hardcoded route strings and prevent typos
 */
export const ROUTES = {
  // Public routes
  HOME: "/",

  // Auth routes
  AUTH: {
    LOGIN: "/auth",
    OTP: "/otp",
    ONBOARDING: "/onboarding",
  },


  // Dashboard routes (for future use)
  DASHBOARD: {
    ROOT: "/dashboard",
    PROFILE: "/profile",
    EDIT_PROFILE: "/profile/edit",
    // Add more dashboard routes as needed
    // SETTINGS: "/dashboard/settings",
  },

  // API routes
  API: {
    DISTRIBUTOR: {
      AUTH: "/api/distributor/v1/auth",
      // Add more API routes as needed
      // PROFILE: "/api/distributor/v1/profile",
      // WALLET: "/api/distributor/v1/wallet",
    },
  },
} as const;

/**
 * Type for route constants to ensure type safety
 */
export type Route =
  | (typeof ROUTES)[keyof typeof ROUTES]
  | (typeof ROUTES.AUTH)[keyof typeof ROUTES.AUTH]
  | (typeof ROUTES.DASHBOARD)[keyof typeof ROUTES.DASHBOARD]
  | (typeof ROUTES.API.DISTRIBUTOR)[keyof typeof ROUTES.API.DISTRIBUTOR];

/**
 * Helper function to get nested route values
 */
export const getRoute = (path: string) => {
  const keys = path.split(".");
  let result: any = ROUTES;

  for (const key of keys) {
    result = result[key];
    if (result === undefined) {
      throw new Error(`Route path "${path}" not found`);
    }
  }

  return result;
};
