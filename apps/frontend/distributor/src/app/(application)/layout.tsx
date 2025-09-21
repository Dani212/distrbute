import CustomSidebar from "@/components/sidebar/custom-sidebar";
import {
  SidebarInset,
  SidebarProvider,
} from "@/components/ui/sidebar";

export default function Layout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex h-screen bg-amber-500">
      <SidebarProvider className="bg-sidebar">
        <CustomSidebar className={"!border-r-0"} />

        <SidebarInset className="flex-1 flex flex-col overflow-hidden mt-2 ml-2 rounded-tl-2xl">
          <main className="flex-1 overflow-auto">{children}</main>
        </SidebarInset>
      </SidebarProvider>
    </div>
  );
}
