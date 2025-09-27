import { getSession } from "next-auth/react";
import { fetchWrapper } from "./fetch-wrapper";
import { APIResponse } from "@distrbute/next-shared";

const endpoint = "/api/distributor/v1";

interface UserResponse {
  id: string;
  email: string;
  name: string;
  profilePicture: {
    id: string;
    fileType: string;
    filename: string;
    rawUrl: string;
    size: number;
    sizeReadable: string;
    url: string;
    otherFormats: {
      HD: string;
      LARGE: string;
      SMALL: string;
      MEDIUM: string;
    };
  };
}

interface UserRequest {
  name: string;
  profilePicture: {
    id: string;
    fileType: string;
    filename: string;
    rawUrl: string;
    size: number;
    sizeReadable: string;
    url: string;
    thumbnail: string;
  };
}

interface PresignedUrlResponse {
  id: string;
  fileType: string;
  filename: string;
  url: string;
  size: number;
  sizeReadable: string;
  uploadedAt: string;
  uploadUrl: string;
  presignExpiresAt: string;
  isPublic: boolean;
}

export class UserApi {
  static async getUser(token?: string) {
    const session = await getSession();
    const accessToken = token || session?.accessToken;

    return fetchWrapper<APIResponse<UserResponse>>(`${endpoint}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  }

  static async createUser(body: UserRequest, token?: string) {
    const session = await getSession();
    const accessToken = token || session?.accessToken;

    console.log(body, "body");

    return fetchWrapper<APIResponse<UserResponse>>(`${endpoint}`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    });
  }

  static async updateUser(body: Partial<UserRequest>, token?: string) {
    const session = await getSession();
    const accessToken = token || session?.accessToken;

    return fetchWrapper<APIResponse<UserResponse>>(`${endpoint}`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    });
  }

  static async generatePresignedUrlProfilePicture(
    body: FormData,
    token?: string
  ) {
    const session = await getSession();
    const headers = {
      Authorization: `Bearer ${token || session?.accessToken}`,
    };

    return fetchWrapper<APIResponse<PresignedUrlResponse>>(
      `${endpoint}/upload-profile-picture`,
      {
        method: "POST",
        headers,
        body,
      }
    );
  }

  static async uploadProfilePicture({
    ownerId,
    file,
    fileName,
    url,
    isPublic = true,
    createdAt,
  }: {
    ownerId: string;
    file: File | Blob;
    fileName: string;
    url: string;
    isPublic?: boolean;
    createdAt?: string;
  }) {
    console.log( {
      "Content-Type": (file as File)?.type || "application/octet-stream",
      "x-amz-meta-original-filename": fileName,
      "x-amz-meta-owner-id": ownerId,
      "x-amz-meta-public": isPublic ? "true" : "false",
      "x-amz-meta-created-at": createdAt || new Date().toISOString(),
    }, "file");

  const formData = new FormData();
  formData.append("blob", file);
    // Upload directly to presigned URL (commonly using PUT). Do not prepend base URL or auth headers.
    return await fetch(url, {
      method: "PUT",
      headers: {
        "Content-Type": (file as File)?.type || "application/octet-stream",
        "x-amz-meta-original-filename": fileName,
        "x-amz-meta-owner-id": ownerId,
        "x-amz-meta-public": isPublic ? "true" : "false",
        "x-amz-meta-created-at": createdAt || new Date().toISOString(),
      },
      body: formData,
    });
  }
}
