import type { DefaultSession, DefaultUser } from "next-auth";
import type { DefaultJWT } from "next-auth/jwt";
import type { AuthUser } from "./user";

declare module "next-auth" {
  interface Session {
    user: AuthUser & DefaultSession["user"];
    accessToken?: string;
  }

  interface User extends AuthUser, DefaultUser {
    
  }
}
declare module "next-auth/jwt" {
  interface JWT extends DefaultJWT {
    id: string;
    email: string;
    name: string;
    provider?: string;
    accessToken?: string;
    image?: string | null;
    accountCreated?: boolean;
    profilePicture?: ProfilePicture | null;
  }
}
