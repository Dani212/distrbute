export class FetchError extends Error {
  constructor(
    message: string,
    public readonly status: number,
    public readonly statusText: string
  ) {
    super(message);
  }
}

/**
 * Extracts a user-friendly error message from various error types
 * @param error - The error object to extract message from
 * @returns A user-friendly error message
 */
export function extractErrorMessage(error: unknown): string {
  // Handle FetchError instances
  if (error instanceof FetchError) {
    if (error.status >= 500) {
      return "Something went wrong on our end. Please try again later.";
    }
    return error.message || "An error occurred. Please try again.";
  }

  // Handle standard Error instances
  if (error instanceof Error) {
    return error.message || "An unexpected error occurred.";
  }

  // Handle string errors
  if (typeof error === "string") {
    return error;
  }

  // Handle objects with message property
  if (error && typeof error === "object" && "message" in error) {
    const message = (error as { message: unknown }).message;
    if (typeof message === "string") {
      return message;
    }
  }

  // Fallback for unknown error types
  return "An unexpected error occurred. Please try again.";
}
