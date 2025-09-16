"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@distrbute/next-shared";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { signupSchema, type SignupFormData } from "@/lib/validations/auth";

interface SignupFormProps {
  onSubmit: (data: SignupFormData) => void;
  isLoading?: boolean;
}

export default function SignupForm({
  onSubmit,
  isLoading = false,
}: SignupFormProps) {
  const form = useForm<SignupFormData>({
    resolver: zodResolver(signupSchema),
    mode: "onChange",
    defaultValues: {
      email: "",
      fullName: "",
    },
  });

  const handleSubmit = (data: SignupFormData) => {
    onSubmit(data);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
        <FormField
          control={form.control}
          name="fullName"
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel className="text-sm font-medium text-slate-700">
                Full Name
              </FormLabel>
              <FormControl>
                <Input
                  type="text"
                  placeholder="John Doe"
                  disabled={isLoading}
                  className={`h-12 transition-colors ${
                    fieldState.error
                      ? "border-red-300 focus:border-red-500 focus:ring-red-500/20"
                      : "border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                  }`}
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="email"
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel className="text-sm font-medium text-slate-700">
                Email
              </FormLabel>
              <FormControl>
                <Input
                  type="email"
                  placeholder="name@example.com"
                  disabled={isLoading}
                  className={`h-12 transition-colors ${
                    fieldState.error
                      ? "border-red-300 focus:border-red-500 focus:ring-red-500/20"
                      : "border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                  }`}
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <Button
          type="submit"
          className="w-full h-12 bg-slate-900 hover:bg-slate-800 text-white font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          disabled={
            isLoading || form.formState.isSubmitting || !form.formState.isValid
          }
        >
          {isLoading ? "Creating account..." : "Create Account"}
        </Button>
      </form>
    </Form>
  );
}
