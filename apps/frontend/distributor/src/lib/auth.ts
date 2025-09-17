import { NextAuthOptions } from "next-auth";
import { JWT } from "next-auth/jwt";
import CredentialsProvider from "next-auth/providers/credentials";
import { AuthApi } from "./api/auth";
import { AUTH } from "./constants/auth";

export const authOptions: NextAuthOptions = {
  providers: [
    CredentialsProvider({
      id: AUTH.PROVIDERS.OTP,
      name: AUTH.PROVIDERS.OTP,
      credentials: {
        email: AUTH.CREDENTIALS.EMAIL,
        otpCode: AUTH.CREDENTIALS.OTP_CODE,
        verificationId: AUTH.CREDENTIALS.VERIFICATION_ID,
        requestId: AUTH.CREDENTIALS.REQUEST_ID,
        otpPrefix: AUTH.CREDENTIALS.OTP_PREFIX,
      },
      async authorize(credentials) {
        if (!credentials?.email || !credentials?.otpCode) {
          return null;
        }

        try {
          const response = await AuthApi.verifyOtp({
            email: credentials.email,
            otpCode: credentials.otpCode,
            verificationId: credentials.verificationId || "",
            requestId: credentials.requestId || "",
            otpPrefix: credentials.otpPrefix || "",
          });

          if (response.code === AUTH.API_CODES.SUCCESS && response.data.token) {
            return {
              id: credentials.email,
              email: credentials.email,
              name: credentials.email,
              accessToken: response.data.token,
              expirationMillis: response.data.expirationMillis,
            };
          }
        } catch (error) {
          console.error("OTP verification failed:", error);
        }

        return null;
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.accessToken = (user as any).accessToken;
        token.expirationMillis = (user as any).expirationMillis;
      }
      return token;
    },
    async session({ session, token }) {
      if (token && session.user) {
        (session.user as any).id = token.sub || "";
        (session as any).accessToken = token.accessToken;
        (session as any).expirationMillis = token.expirationMillis;
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
