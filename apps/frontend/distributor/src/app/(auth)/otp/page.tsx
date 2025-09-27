"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { z } from "zod";
import {
  Button,
  extractErrorMessage,
  FetchError,
} from "@distrbute/next-shared";
import { AuthApi } from "@/lib/api/auth";
import { STORAGE_KEYS, type OtpData } from "@/lib/constants/storage";
import { ROUTES } from "@/lib/constants/routes";
import { toast } from "sonner";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSeparator,
  InputOTPSlot,
} from "@/components/ui/input-otp";
import { getSession, signIn } from "next-auth/react";
import { AUTH } from "@/lib/constants/auth";
import { UserApi } from "@/lib/api/user";

const otpSchema = z.object({
  otp: z
    .string()
    .min(1, "OTP is required")
    .length(6, "OTP must be exactly 6 digits")
    .regex(/^\d+$/, "OTP must contain only numbers"),
});

type OtpFormData = z.infer<typeof otpSchema>;

export default function OtpPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [otpData, setOtpData] = useState<OtpData | null>(null);
  const [resendCountdown, setResendCountdown] = useState(0);
  const router = useRouter();

  const form = useForm<OtpFormData>({
    resolver: zodResolver(otpSchema),
    defaultValues: {
      otp: "",
    },
  });

  useEffect(() => {
    // Get the OTP data from sessionStorage
    const storedOtpData = sessionStorage.getItem(STORAGE_KEYS.OTP_DATA);
    if (storedOtpData) {
      setOtpData(JSON.parse(storedOtpData));
    } else {
      // If no OTP data, redirect back to auth page
      router.push(ROUTES.AUTH.LOGIN);
    }
  }, [router]);

  // Countdown timer effect
  useEffect(() => {
    let interval: NodeJS.Timeout;
    if (resendCountdown > 0) {
      interval = setInterval(() => {
        setResendCountdown((prev) => {
          if (prev <= 1) {
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }
    return () => {
      if (interval) {
        clearInterval(interval);
      }
    };
  }, [resendCountdown]);

  const onSubmit = (data: OtpFormData) => {
    if (!otpData) {
      toast.error("OTP data not found. Please try again.");
      return;
    }

    setIsLoading(true);

    AuthApi.verifyOtp({
      email: otpData.email,
      otpCode: data.otp,
      verificationId: otpData.verificationId,
      requestId: otpData.requestId,
      otpPrefix: otpData.otpPrefix,
    })
      .then(async (response) => {
        if (response.code === AUTH.API_CODES.SUCCESS && response.data.token) {
          // Get user data
          const userResponse = await UserApi.getUser(response.data.token).catch(
            (error) => {
              if (error instanceof FetchError && error.status === 404) {
                return {
                  data: null,
                };
              }
              throw error;
            }
          );

          const userData = {
            id: otpData.email,
            email: otpData.email,
            name: userResponse.data?.name || otpData.name || otpData.email,
            image: userResponse.data?.profilePicture?.url || null,
            accessToken: response.data.token,
            expirationMillis: response.data.expirationMillis,
            accountCreated: userResponse.data !== null,
            profilePicture: userResponse.data?.profilePicture || null,
          };

          // Sign in with NextAuth using OTP credentials
          signIn(AUTH.PROVIDERS.OTP, { ...userData, redirect: false }).then(
            (result) => {
              if (result?.error) {
                toast.error(extractErrorMessage(result.error));
              } else {
                toast.success("OTP verified successfully!");
                // Clear the OTP data from sessionStorage
                sessionStorage.removeItem(STORAGE_KEYS.OTP_DATA);

                if (userData.accountCreated) {
                  // Redirect to dashboard on successful authentication
                  router.push(ROUTES.DASHBOARD.ROOT);
                } else {
                  router.push(ROUTES.AUTH.ONBOARDING);
                }
              }
            }
          );
        }
      })
      .catch((error) => {
        console.error("OTP verification failed:", error);
        toast.error(extractErrorMessage(error));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleResendOtp = () => {
    if (!otpData) {
      toast.error("OTP data not found. Please try again.");
      return;
    }

    if (resendCountdown > 0) {
      toast.error(`Please wait ${resendCountdown} seconds before resending.`);
      return;
    }

    setIsLoading(true);

    // Prepare the resend request with name if available
    const resendRequest: { email: string; name?: string } = {
      email: otpData.email,
    };
    if (otpData.name) {
      resendRequest.name = otpData.name;
    }

    AuthApi.sendOtp(resendRequest)
      .then((response) => {
        // Update the stored OTP data with new response
        const newOtpData = {
          verificationId: response.data.verificationId,
          requestId: response.data.requestId,
          otpPrefix: response.data.otpPrefix,
          email: response.data.email,
          name: otpData.name, // Preserve the original name
        };

        sessionStorage.setItem(
          STORAGE_KEYS.OTP_DATA,
          JSON.stringify(newOtpData)
        );
        setOtpData(newOtpData);
        setResendCountdown(60); // Start 60-second countdown
        toast.success("OTP resent successfully!");
      })
      .catch((error) => {
        console.error("Resend OTP error:", error);
        toast.error(extractErrorMessage(error));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  // Show loading state while OTP data is being loaded
  if (!otpData) {
    return (
      <div className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8">
        <div className="w-full max-w-sm">
          <Card className="border-[0.1px] shadow-2xs bg-white/80 backdrop-blur-sm">
            <CardContent className="px-8 pb-8 pt-8">
              <div className="text-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-slate-900 mx-auto mb-4"></div>
                <p className="text-slate-500">Loading...</p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <div className="w-full max-w-[440px]">
        <Card className="border-[0.1px] shadow-2xs bg-white/80 backdrop-blur-sm">
          <CardHeader className="text-center space-y-2 pb-8">
            <CardTitle className="text-3xl font-light tracking-tight text-slate-900">
              Verify Email
            </CardTitle>
            <CardDescription className="text-slate-500 text-sm font-normal">
              Enter the 6-digit code sent to{" "}
              <span className="font-medium text-slate-700">
                {otpData?.email || "your email"}
              </span>
            </CardDescription>
          </CardHeader>
          <CardContent className="px-8 pb-8">
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-8"
              >
                <FormField
                  control={form.control}
                  name="otp"
                  render={({ field }) => (
                    <FormItem className="space-y-4">
                      <FormControl>
                        <div className="flex justify-center">
                          <InputOTP
                            maxLength={6}
                            value={field.value}
                            onChange={field.onChange}
                            className="gap-2"
                          >
                            <InputOTPGroup className="gap-2">
                              <InputOTPSlot
                                index={0}
                                className="w-12 h-12 border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                              />
                              <InputOTPSlot
                                index={1}
                                className="w-12 h-12 border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                              />
                              <InputOTPSlot
                                index={2}
                                className="w-12 h-12 border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                              />
                            </InputOTPGroup>
                            <InputOTPSeparator className="text-slate-300" />
                            <InputOTPGroup className="gap-2">
                              <InputOTPSlot
                                index={3}
                                className="w-12 h-12 border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                              />
                              <InputOTPSlot
                                index={4}
                                className="w-12 h-12 border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                              />
                              <InputOTPSlot
                                index={5}
                                className="w-12 h-12 border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                              />
                            </InputOTPGroup>
                          </InputOTP>
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <Button
                  type="submit"
                  className="w-full h-12 bg-slate-900 hover:bg-slate-800 text-white font-medium transition-colors"
                  disabled={isLoading}
                >
                  {isLoading ? "Verifying..." : "Verify Code"}
                </Button>
              </form>
            </Form>

            <div className="mt-8 text-center">
              {resendCountdown > 0 ? (
                <p>Resend code in {resendCountdown}s</p>
              ) : (
                <p className="text-sm text-slate-500">
                  Didn't receive the code?{" "}
                  <button
                    type="button"
                    onClick={handleResendOtp}
                    disabled={resendCountdown > 0 || isLoading}
                    className={`font-medium transition-colors focus:outline-none focus:underline underline-offset-2 ${
                      resendCountdown > 0 || isLoading
                        ? "text-slate-400 cursor-not-allowed"
                        : "text-slate-900 hover:text-slate-700"
                    }`}
                  >
                    Resend code
                  </button>
                </p>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
