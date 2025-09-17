import Link from "next/link";
import { Button } from "@distrbute/next-shared";
import { ROUTES } from "@/lib/constants/routes";

export default function NotFound() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">404</h1>
        <p className="text-lg text-gray-600 mb-8">Page not found</p>
        <Link href={ROUTES.HOME}>
          <Button>Go back home</Button>
        </Link>
      </div>
    </div>
  );
}
