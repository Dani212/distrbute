import { withAuth } from "next-auth/middleware";
import { NextResponse } from "next/server";
import { ROUTES } from "./lib/constants/routes";

const authRoutes = [ROUTES.AUTH.LOGIN, ROUTES.AUTH.OTP];
const publicRoutes = [...authRoutes] as string[];

export default withAuth(
  function middleware(req) {
    const { nextUrl } = req;
    const token = req.nextauth.token;
    const isAuthPage = authRoutes.includes(
      nextUrl.pathname as (typeof authRoutes)[number]
    );

    // If user is authenticated and trying to access auth pages, redirect to dashboard
    if (token && isAuthPage) {
      return NextResponse.redirect(
        new URL(ROUTES.DASHBOARD.ROOT, nextUrl.origin)
      );
    }

    // Continue with normal flow
    return NextResponse.next();
  },
  {
    callbacks: {
      authorized: ({ token, req }) => {
        // Allow access to auth pages and OTP page without token
        if (publicRoutes.includes(req.nextUrl.pathname)) {
          return true;
        }

        // Require token for all other pages
        return !!token;
      },
    },
  }
);

export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     */
    "/((?!api|_next/static|_next/image|favicon.ico).*)",
  ],
};
