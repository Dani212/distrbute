export interface AuthUser {
  id: string;
  email: string;
  name: string;
  image?: string | null;
  accessToken?: string;
  expirationMillis?: number;
  accountCreated?: boolean;
  profilePicture?: ProfilePicture | null;
}

export interface ProfilePicture {
  id: string;
  fileType: string;
  filename: string;
  rawUrl: string;
  size: number;
  sizeReadable: string;
  url: string;
  thumbnail: string;
}
