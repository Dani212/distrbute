"use client";

import { ROUTES } from "@/lib/constants/routes";
import { useSession, signOut } from "next-auth/react";
import { useRouter } from "next/navigation";

export function useAuth() {
  const { data: session, status, update } = useSession();
  const router = useRouter();

  const isLoading = status === "loading";
  const isAuthenticated = !!session?.user;

  const logout = async () => {
    await signOut({ redirect: false });
    router.push(ROUTES.AUTH.LOGIN);
  };

  return {
    user: session?.user,
    session,
    isLoading,
    isAuthenticated,
    logout,
    update,
  };
}
