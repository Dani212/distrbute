import { getSession } from "next-auth/react";
import { fetchWrapper } from "./fetch-wrapper";
import { APIResponse } from "@distrbute/next-shared";

const endpoint = "/api/distributor/v1";

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

export async function generatePresignedUrlProfilePicture(
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

export async function uploadProfilePicture({
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
    body: file,
  });
}
