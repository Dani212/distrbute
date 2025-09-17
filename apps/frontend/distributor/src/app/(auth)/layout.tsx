import Image from "next/image";

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div>
      <div className="min-h-screen w-screen flex flex-col">
        <div className="w-full p-3 xl:px-12">
          <h1 className="text-2xl font-bold text-primary italic">Distributor Portal</h1>
        </div>
        <div className="flex-1 flex">
          {children}
        </div>
      </div>
    </div>
  );
}
