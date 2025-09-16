/**
 * HTTP method constants used throughout the application
 * Centralized constants to avoid hardcoded HTTP method strings and prevent typos
 */
export const HTTP_METHODS = {
  GET: "GET",
  POST: "POST",
  PUT: "PUT",
  DELETE: "DELETE",
  PATCH: "PATCH",
  HEAD: "HEAD",
  OPTIONS: "OPTIONS",
} as const;

/**
 * Type for HTTP methods to ensure type safety
 */
export type HttpMethod = (typeof HTTP_METHODS)[keyof typeof HTTP_METHODS];

/**
 * Common HTTP status codes for reference
 */
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  METHOD_NOT_ALLOWED: 405,
  CONFLICT: 409,
  UNPROCESSABLE_ENTITY: 422,
  INTERNAL_SERVER_ERROR: 500,
  BAD_GATEWAY: 502,
  SERVICE_UNAVAILABLE: 503,
} as const;

/**
 * Type for HTTP status codes
 */
export type HttpStatus = (typeof HTTP_STATUS)[keyof typeof HTTP_STATUS];

/**
 * Common content types for API requests
 */
export const CONTENT_TYPES = {
  JSON: "application/json",
  FORM_DATA: "multipart/form-data",
  URL_ENCODED: "application/x-www-form-urlencoded",
  TEXT: "text/plain",
  HTML: "text/html",
} as const;

/**
 * Type for content types
 */
export type ContentType = (typeof CONTENT_TYPES)[keyof typeof CONTENT_TYPES];
