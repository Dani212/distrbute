import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Distrbute - Connect Brands with Distributors",
  description:
    "The leading B2B platform connecting brands with distributors worldwide",
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
                <h1 className="text-2xl font-bold text-primary">Distrbute</h1>
                <div className="flex space-x-4">
                  <a href="/about" className="text-sm hover:text-primary">
                    About
                  </a>
                  <a href="/pricing" className="text-sm hover:text-primary">
                    Pricing
                  </a>
                  <a href="/contact" className="text-sm hover:text-primary">
                    Contact
                  </a>
                  <a
                    href="http://localhost:3001"
                    className="text-sm hover:text-primary"
                  >
                    Brand Portal
                  </a>
                  <a
                    href="http://localhost:3002"
                    className="text-sm hover:text-primary"
                  >
                    Distributor Portal
                  </a>
                </div>
              </div>
            </div>
          </nav>
          <main>{children}</main>
        </div>
      </body>
    </html>
  );
}
