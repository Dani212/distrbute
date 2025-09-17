"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { LoginForm, SignupForm } from "@/components/auth";
import {
  type LoginFormData,
  type SignupFormData,
} from "@/lib/validations/auth";
import { AuthApi } from "@/lib/api/auth";
import { STORAGE_KEYS, type OtpData } from "@/lib/constants/storage";
import { ROUTES } from "@/lib/constants/routes";
import { extractErrorMessage } from "@distrbute/next-shared";
import { toast } from "sonner";

export default function AuthPage() {
  const [isLogin, setIsLogin] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const router = useRouter();

  const onLoginSubmit = (data: LoginFormData) => {
    setIsLoading(true);

    AuthApi.sendOtp({ email: data.email })
      .then((response) => {
        // Store the response data in sessionStorage to pass to OTP page
        const otpData: OtpData = {
          verificationId: response.data.verificationId,
          requestId: response.data.requestId,
          otpPrefix: response.data.otpPrefix,
          email: response.data.email,
          // No name for login flow
        };

        sessionStorage.setItem(STORAGE_KEYS.OTP_DATA, JSON.stringify(otpData));
        router.push(ROUTES.AUTH.OTP);
      })
      .catch((error) => {
        console.error("Login error:", error);
        toast.error(extractErrorMessage(error));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const onSignupSubmit = (data: SignupFormData) => {
    setIsLoading(true);

    AuthApi.sendOtp({ email: data.email, name: data.fullName })
      .then((response) => {
        // Store the response data in sessionStorage to pass to OTP page
        const otpData: OtpData = {
          verificationId: response.data.verificationId,
          requestId: response.data.requestId,
          otpPrefix: response.data.otpPrefix,
          email: response.data.email,
          name: data.fullName, // Store the name for resend functionality
        };

        sessionStorage.setItem(STORAGE_KEYS.OTP_DATA, JSON.stringify(otpData));
        router.push(ROUTES.AUTH.OTP);
      })
      .catch((error) => {
        console.error("Signup error:", error);
        toast.error(extractErrorMessage(error));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const toggleMode = () => {
    setIsLogin(!isLogin);
  };

  return (
    <div className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <div className="w-full max-w-[440px]">
        <Card className="shadow-2xs border-[0.1px] bg-white/80 backdrop-blur-sm">
          <CardHeader className="text-center space-y-2 pb-8">
            <CardTitle className="text-3xl font-light tracking-tight text-slate-900">
              {isLogin ? "Welcome back" : "Create account"}
            </CardTitle>
            <CardDescription className="text-slate-500 text-sm font-normal">
              {isLogin
                ? "Enter your email to continue"
                : "Get started with your details"}
            </CardDescription>
          </CardHeader>
          <CardContent className="px-8 pb-8">
            {isLogin ? (
              <LoginForm onSubmit={onLoginSubmit} isLoading={isLoading} />
            ) : (
              <SignupForm onSubmit={onSignupSubmit} isLoading={isLoading} />
            )}

            <div className="mt-8 text-center">
              <p className="text-sm text-slate-500">
                {isLogin
                  ? "Don't have an account? "
                  : "Already have an account? "}
                <button
                  type="button"
                  onClick={toggleMode}
                  className="font-medium text-slate-900 hover:text-slate-700 transition-colors focus:outline-none focus:underline underline-offset-2"
                >
                  {isLogin ? "Sign up" : "Sign in"}
                </button>
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
