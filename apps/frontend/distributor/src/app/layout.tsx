import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Distributor Portal - Distrbute",
  description: "Browse products and manage orders as a distributor",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <div className="min-h-screen bg-background">
          <nav className="border-b">
            <div className="container mx-auto px-4 py-4">
              <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-primary">
                  Distributor Portal
                </h1>
                <div className="flex space-x-4">
                  <a href="/catalog" className="text-sm hover:text-primary">
                    Catalog
                  </a>
                  <a href="/orders" className="text-sm hover:text-primary">
                    Orders
                  </a>
                  <a href="/profile" className="text-sm hover:text-primary">
                    Profile
                  </a>
                </div>
              </div>
            </div>
          </nav>
          <main className="container mx-auto px-4 py-8">{children}</main>
        </div>
      </body>
    </html>
  );
}
