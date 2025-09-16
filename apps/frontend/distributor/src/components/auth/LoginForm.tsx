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
import { loginSchema, type LoginFormData } from "@/lib/validations/auth";

interface LoginFormProps {
  onSubmit: (data: LoginFormData) => void;
  isLoading?: boolean;
}

export default function LoginForm({
  onSubmit,
  isLoading = false,
}: LoginFormProps) {
  const form = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    mode: "onChange",
    defaultValues: {
      email: "",
    },
  });

  const handleSubmit = (data: LoginFormData) => {
    onSubmit(data);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
        <FormField
          control={form.control}
          name="email"
          render={({ field, fieldState }) => (
            <FormItem className="space-y-1">
              <FormLabel className="text-sm font-medium text-slate-700">
                Email
              </FormLabel>
              <FormControl>
                <Input
                  type="email"
                  placeholder="name@example.com"
                  disabled={isLoading}
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  name={field.name}
                  ref={field.ref}
                  className={`h-12 transition-colors ${
                    fieldState.error
                      ? "border-red-300 focus:border-red-500 focus:ring-red-500/20"
                      : "border-slate-200 focus:border-slate-400 focus:ring-slate-400/20"
                  }`}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button
          type="submit"
          className="w-full h-12 bg-slate-900 hover:bg-slate-800 text-white font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          disabled={isLoading || form.formState.isSubmitting}
        >
          {isLoading ? "Signing in..." : "Continue"}
        </Button>
      </form>
    </Form>
  );
}
