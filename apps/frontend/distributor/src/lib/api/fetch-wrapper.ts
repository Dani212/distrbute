import { FetchError } from "@distrbute/next-shared";
import { HTTP_METHODS } from "@/lib/constants/http";

export async function fetchWrapper<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const {
    method = HTTP_METHODS.GET,
    body,
    headers: optionsHeaders,
  } = options || {};

  // Create headers object and merge with provided headers
  const headers = new Headers();

  // Add any headers from options first
  if (optionsHeaders) {
    if (optionsHeaders instanceof Headers) {
      optionsHeaders.forEach((value, key) => {
        headers.set(key, value);
      });
    } else if (Array.isArray(optionsHeaders)) {
      optionsHeaders.forEach(([key, value]) => {
        headers.set(key, value);
      });
    } else {
      // Handle plain object headers
      Object.entries(optionsHeaders).forEach(([key, value]) => {
        if (value !== undefined) {
          headers.set(key, value);
        }
      });
    }
  }

  const base = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "";
  const url = `${base}${endpoint}`;

  console.log("Request URL:", url);
  console.log("Request Method:", method);

  const requestOptions: RequestInit = {
    method,
    headers,
    body,
  };
  const response = await fetch(url, requestOptions);

  const data = await response.json();

  if (!response.ok) {
    const error = new FetchError(
      data.title || data.message,
      response.status,
      response.statusText
    );
    throw error;
  }
  return data;
}
