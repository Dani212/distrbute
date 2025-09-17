import {
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@distrbute/next-shared";

export default function MarketingHome() {
  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="bg-gradient-to-r from-primary to-primary/80 text-primary-foreground py-20">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-5xl font-bold mb-6">
            Connect Brands with Distributors
          </h1>
          <p className="text-xl mb-8 max-w-2xl mx-auto">
            The leading B2B platform that streamlines distribution channels,
            enabling seamless connections between brands and distributors
            worldwide.
          </p>
          <div className="flex gap-4 justify-center">
            <Button size="lg" variant="secondary">
              <a href="http://localhost:3001">For Brands</a>
            </Button>
            <Button
              size="lg"
              variant="outline"
              className="bg-transparent border-primary-foreground text-primary-foreground hover:bg-primary-foreground hover:text-primary"
            >
              <a href="http://localhost:3002">For Distributors</a>
            </Button>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <div className="text-center mb-16">
            <h2 className="text-3xl font-bold mb-4">Why Choose Distrbute?</h2>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              Our platform provides everything you need to manage your
              distribution network efficiently.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <Card>
              <CardHeader>
                <CardTitle>For Brands</CardTitle>
                <CardDescription>
                  Manage your product catalog and connect with verified
                  distributors
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm">
                  <li>• Product catalog management</li>
                  <li>• Order tracking and analytics</li>
                  <li>• Distributor network expansion</li>
                  <li>• Real-time inventory updates</li>
                </ul>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>For Distributors</CardTitle>
                <CardDescription>
                  Discover new products and manage orders from trusted brands
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm">
                  <li>• Browse product catalogs</li>
                  <li>• Streamlined ordering process</li>
                  <li>• Order history and tracking</li>
                  <li>• Direct brand communication</li>
                </ul>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Analytics & Insights</CardTitle>
                <CardDescription>
                  Get detailed insights into your distribution performance
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm">
                  <li>• Sales performance metrics</li>
                  <li>• Market trend analysis</li>
                  <li>• Inventory optimization</li>
                  <li>• Custom reporting</li>
                </ul>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="bg-muted py-20">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-3xl font-bold mb-4">Ready to Get Started?</h2>
          <p className="text-muted-foreground mb-8 max-w-2xl mx-auto">
            Join thousands of brands and distributors already using Distrbute to
            grow their business.
          </p>
          <div className="flex gap-4 justify-center">
            <Button size="lg">
              <a href="http://localhost:3001">Start as Brand</a>
            </Button>
            <Button size="lg" variant="outline">
              <a href="http://localhost:3002">Start as Distributor</a>
            </Button>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t py-8">
        <div className="container mx-auto px-4">
          <div className="flex flex-col md:flex-row justify-between items-center">
            <div className="text-sm text-muted-foreground">
              © 2024 Distrbute. All rights reserved.
            </div>
            <div className="flex space-x-6 mt-4 md:mt-0">
              <a href="/about" className="text-sm hover:text-primary">
                About
              </a>
              <a href="/pricing" className="text-sm hover:text-primary">
                Pricing
              </a>
              <a href="/contact" className="text-sm hover:text-primary">
                Contact
              </a>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}
