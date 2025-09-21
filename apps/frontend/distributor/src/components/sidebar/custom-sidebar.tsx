"use client";

import { HomeIcon, Link } from "lucide-react";
import {
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  Sidebar,
  SidebarMenuButton,
  SidebarMenuItem,
} from "../ui/sidebar";
import { NavUser } from "./nav-user";
import { cn } from "@/lib/utils";
import { usePathname } from "next/navigation";
import { ROUTES } from "@/lib/constants/routes";

interface SidebarProps {
  className?: string;
}

export default function CustomSidebar({ className }: SidebarProps) {
  const pathname = usePathname();

  const menuItems = [
    {
      name: "dashboard",
      label: "Dashboard",
      icon: HomeIcon,
      href: ROUTES.DASHBOARD.ROOT,
    },
  ];
  return (
    <Sidebar className={className}>
      {/* Header with Logo */}
      <SidebarHeader className="p-6 pb-4">
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium text-sidebar-foreground">
            Distributor
          </span>
        </div>
      </SidebarHeader>

      {/* Main Content */}
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu className="px-1 py-4">
              {menuItems.map((item) => {
                const Icon = item.icon;
                const isActive = pathname.includes(item.href);

                return (
                  <SidebarMenuItem key={item.name} className="py-1">
                    <SidebarMenuButton
                      asChild
                      isActive={isActive}
                      tooltip={item.label}
                    >
                      <Link
                        href={item.href}
                        className={cn(
                          "flex items-center justify-between px-3 rounded-md text-sm font-medium transition-colors",
                          isActive
                            ? "text-sidebar-accent-foreground bg-sidebar-accent"
                            : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
                        )}
                      >
                        <div className="flex items-center space-x-3">
                          <Icon />
                          <span>{item.label}</span>
                        </div>
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                );
              })}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      {/* Footer with Switchers */}
      <SidebarFooter className="p-2 space-y-2">
        <NavUser />
      </SidebarFooter>
    </Sidebar>
  );
}
