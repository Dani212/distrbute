import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@distrbute/ui";
import { Product, OrderStatus } from "@distrbute/shared-models";

export default function BrandDashboard() {
  // Mock data - in real app this would come from API
  const stats = {
    totalProducts: 24,
    activeOrders: 8,
    totalRevenue: 45600,
    pendingOrders: 3,
  };

  const recentOrders = [
    {
      id: "ORD-001",
      distributor: "Tech Distributors Inc.",
      amount: 2400,
      status: OrderStatus.PENDING,
      date: new Date("2024-01-15"),
    },
    {
      id: "ORD-002",
      distributor: "Retail Solutions Ltd.",
      amount: 1800,
      status: OrderStatus.CONFIRMED,
      date: new Date("2024-01-14"),
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <p className="text-muted-foreground">Welcome to your brand portal</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Total Products
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalProducts}</div>
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
            <CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              ${stats.totalRevenue.toLocaleString()}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Pending Orders
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.pendingOrders}</div>
          </CardContent>
        </Card>
      </div>

      {/* Recent Orders */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Orders</CardTitle>
          <CardDescription>Latest orders from distributors</CardDescription>
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
                    {order.distributor}
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
