import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Button,
} from "@distrbute/ui";
import {
  Product,
  ProductCategory,
  OrderStatus,
} from "@distrbute/shared-models";

export default function DistributorDashboard() {
  // Mock data - in real app this would come from API
  const stats = {
    totalOrders: 12,
    activeOrders: 5,
    totalSpent: 15600,
    favoriteBrands: 8,
  };

  const featuredProducts = [
    {
      id: "PROD-001",
      name: "Wireless Headphones Pro",
      brand: "TechBrand",
      price: 299.99,
      category: ProductCategory.ELECTRONICS,
      image: "/placeholder-product.jpg",
    },
    {
      id: "PROD-002",
      name: "Smart Watch Series 5",
      brand: "TechBrand",
      price: 399.99,
      category: ProductCategory.ELECTRONICS,
      image: "/placeholder-product.jpg",
    },
  ];

  const recentOrders = [
    {
      id: "ORD-001",
      brand: "TechBrand",
      amount: 899.98,
      status: OrderStatus.SHIPPED,
      date: new Date("2024-01-15"),
    },
    {
      id: "ORD-002",
      brand: "FashionCorp",
      amount: 450.0,
      status: OrderStatus.DELIVERED,
      date: new Date("2024-01-12"),
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <p className="text-muted-foreground">
          Welcome to your distributor portal
        </p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Orders</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalOrders}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Orders</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.activeOrders}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Spent</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              ${stats.totalSpent.toLocaleString()}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Favorite Brands
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.favoriteBrands}</div>
          </CardContent>
        </Card>
      </div>

      {/* Featured Products */}
      <Card>
        <CardHeader>
          <CardTitle>Featured Products</CardTitle>
          <CardDescription>
            Popular products from your favorite brands
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {featuredProducts.map((product) => (
              <Card key={product.id}>
                <CardContent className="p-4">
                  <div className="aspect-square bg-muted rounded-lg mb-4"></div>
                  <h3 className="font-medium">{product.name}</h3>
                  <p className="text-sm text-muted-foreground">
                    {product.brand}
                  </p>
                  <p className="text-lg font-bold">${product.price}</p>
                  <Button className="w-full mt-2">Add to Cart</Button>
                </CardContent>
              </Card>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Recent Orders */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Orders</CardTitle>
          <CardDescription>Your latest orders</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {recentOrders.map((order) => (
              <div
                key={order.id}
                className="flex items-center justify-between p-4 border rounded-lg"
              >
                <div>
                  <div className="font-medium">{order.id}</div>
                  <div className="text-sm text-muted-foreground">
                    {order.brand}
                  </div>
                </div>
                <div className="text-right">
                  <div className="font-medium">
                    ${order.amount.toLocaleString()}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    {order.status}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
