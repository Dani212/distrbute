import { fetchWrapper } from "./fetch-wrapper";
import { ROUTES } from "@/lib/constants/routes";
import { HTTP_METHODS, CONTENT_TYPES } from "@/lib/constants/http";

export interface SendOtpRequest {
  // Based on the curl command, it seems the body is empty or not specified.
  // If there are specific body parameters, please let me know.
  // For now, I'll assume it might take an email or phone number.
  email: string; // e.g., email or phone number
  name?: string;
}

export interface SendOtpResponseData {
  requestId: string;
  verificationId: string;
  otpPrefix: string;
  email: string; // Assuming email is always returned, adjust if it can be phone too
}

export interface SendOtpApiResponse {
  message: string;
  code: number;
  data: SendOtpResponseData;
  errors: string | null; // Assuming errors can be null if no errors
}

export interface VerifyOtpRequest {
  verificationId: string;
  requestId: string;
  otpPrefix: string;
  otpCode: string;
  email: string;
}

export interface VerifyOtpApiResponse {
  message: string;
  code: number;
  data: {
    token?: string;
    expirationMillis: number;
    email: string;
  };
  errors: string | null;
}

const endpoint = ROUTES.API.DISTRIBUTOR.AUTH;

export class AuthApi {
  static async sendOtp(request: SendOtpRequest): Promise<SendOtpApiResponse> {
    // Prepare the request body
    const requestBody: { email: string; name?: string } = {
      email: request.email,
    };

    // Only include name if it's provided and not empty
    if (request.name && request.name.trim() !== "") {
      requestBody.name = request.name;
    }

    return await fetchWrapper(`${endpoint}/send-otp`, {
      method: HTTP_METHODS.POST,
      headers: {
        "Content-Type": CONTENT_TYPES.JSON,
        Accept: CONTENT_TYPES.JSON,
      },
      body: JSON.stringify(requestBody),
    });
  }

  static async verifyOtp(
    request: VerifyOtpRequest
  ): Promise<VerifyOtpApiResponse> {
    return await fetchWrapper(`${endpoint}/verify-otp`, {
      method: HTTP_METHODS.POST,
      headers: {
        "Content-Type": CONTENT_TYPES.JSON,
        Accept: CONTENT_TYPES.JSON,
      },
      body: JSON.stringify({
        verificationId: request.verificationId,
        otpCode: request.otpCode,
        email: request.email,
        requestId: request.requestId,
        otpPrefix: request.otpPrefix,
      }),
    });
  }
}
