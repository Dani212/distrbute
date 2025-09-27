import { NextAuthOptions } from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";
import { AUTH } from "./constants/auth";
import { ProfilePicture } from "@/types/user";

export const authOptions: NextAuthOptions = {
  providers: [
    CredentialsProvider({
      id: AUTH.PROVIDERS.OTP,
      name: AUTH.PROVIDERS.OTP,
      credentials: {
        email: AUTH.CREDENTIALS.EMAIL,
        name: AUTH.CREDENTIALS.NAME,
        image: AUTH.CREDENTIALS.IMAGE,
        accessToken: AUTH.CREDENTIALS.ACCESS_TOKEN,
        expirationMillis: AUTH.CREDENTIALS.EXPIRATION_MILLIS,
        accountCreated: AUTH.CREDENTIALS.ACCOUNT_CREATED,
        profilePicture: AUTH.CREDENTIALS.PROFILE_PICTURE,
      },
      async authorize(credentials) {
        if (!credentials?.email) {
          return null;
        }

        return {
          id: credentials?.email,
          name: credentials?.name,
          email: credentials?.email,
          image: credentials?.image,
          accessToken: credentials?.accessToken,
          expirationMillis: Number(credentials?.expirationMillis),
          accountCreated: Boolean(credentials?.accountCreated),
          profilePicture: credentials?.profilePicture
            ? (JSON.parse(credentials.profilePicture) as ProfilePicture)
            : undefined,
        };
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user, trigger, session }) {
      if (trigger === "update" && session) {
        token.name = (session as any).name || token.name;
        token.image = (session as any).image || token.image;
      } else if (user) {
        token.accessToken = (user as any).accessToken;
        token.expirationMillis = (user as any).expirationMillis;
        token.name = (user as any).name;
        token.image = (user as any).image;
        token.accountCreated = (user as any).accountCreated;
        token.profilePicture = (user as any).profilePicture;
      }

      return token;
    },
    async session({ session, token }) {
      if (token && session) {
        (session.user as any).id = token.sub || "";
        (session as any).accessToken = token.accessToken;
        (session as any).expirationMillis = token.expirationMillis;
        (session as any).name = token.name;
        (session as any).image = token.image;
        (session as any).accountCreated = token.accountCreated;
      }

      if (session.user) {
        (session.user as any).id = token.sub || "";
        (session.user as any).accessToken = token.accessToken;
        (session.user as any).expirationMillis = token.expirationMillis;
        (session.user as any).name = token.name;
        (session.user as any).image = token.image;
        (session.user as any).accountCreated = token.accountCreated;
        (session.user as any).profilePicture = token.profilePicture;
      }
      return session;
    },
  },
  pages: {
    signIn: AUTH.PAGES.SIGN_IN,
    error: AUTH.PAGES.ERROR,
  },
  session: {
    strategy: AUTH.SESSION.STRATEGY,
    maxAge: AUTH.SESSION.MAX_AGE_SECONDS,
  },
  secret: process.env.NEXTAUTH_SECRET,
};
