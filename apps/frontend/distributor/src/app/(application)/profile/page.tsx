"use client";

import { useAuth } from "@/hooks/use-auth";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  ArrowLeft,
  Edit,
  Mail,
  User,
  Calendar,
  Shield,
  Settings,
} from "lucide-react";
import { useRouter } from "next/navigation";
import { ROUTES } from "@/lib/constants/routes";
import { UserApi } from "@/lib/api/user";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { extractErrorMessage } from "@distrbute/next-shared";

interface UserDetails {
  name: string;
  email: string;
  image: string;
}

export default function ProfilePage() {
  const { user, update } = useAuth();
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);
  const [error, setError] = useState<string | null>(null);

  const fetchUserDetails = async () => {
    console.log(user?.accessToken, "user?.accessToken3");
    UserApi.getUser(user?.accessToken)
      .then(async (response) => {
        console.log(response, "response2");
        await update({
          name: response.data.name,
          image: response.data.profilePicture?.url,
          email: response.data.email,
          profilePicture: response.data?.profilePicture,
        });

        return response;
      })
      .then((response) => {
        console.log(response, "response3");
        setUserDetails({
          name: response.data.name,
          email: response.data.email,
          image: response.data.profilePicture?.url,
        });
      })
      .catch((error) => {
        console.log(error, "error3");
        setError(extractErrorMessage(error));
        toast.error(extractErrorMessage(error));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  useEffect(() => {
    fetchUserDetails();
  }, []);

  const handleBack = () => {
    router.back();
  };

  const handleEditProfile = () => {
    router.push(ROUTES.DASHBOARD.EDIT_PROFILE);
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-gray-600">Loading...</p>
        </div>
      </div>
    );
  }

  if (!!error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <p className="mt-2 text-gray-600">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header Section */}
        <div className="mb-8">
          <div className="flex items-center justify-between mb-6">
            <Button
              variant="ghost"
              onClick={handleBack}
              className="group flex items-center gap-2 text-slate-600 hover:text-slate-900 hover:bg-white/50 transition-all duration-200"
            >
              <ArrowLeft className="h-4 w-4 group-hover:-translate-x-1 transition-transform" />
              Back
            </Button>
          </div>

          <div className="text-left">
            <h1 className="text-3xl font-bold bg-gradient-to-r from-slate-900 via-blue-900 to-indigo-900 bg-clip-text text-transparent mb-0.5">
              Profile
            </h1>
            <p className="text-md text-slate-600 max-w-2xl">
              Your personal dashboard and account information
            </p>
          </div>
        </div>

        <div className="grid grid-cols-1 xl:grid-cols-1 gap-8">
          {/* Profile Overview */}
          <div className="lg:col-span-1">
            <Card className="backdrop-blur-sm">
              <CardContent className="p-8">
                <div className="flex flex-col items-center space-y-6 relative">
                  <div className="relative">
                    <Avatar className="h-32 w-32 ring-4 ring-white shadow-2xl">
                      <AvatarImage
                        src={userDetails?.image || ""}
                        alt={userDetails?.name || "Profile"}
                        className="object-cover"
                      />
                      <AvatarFallback className="text-3xl font-bold bg-gradient-to-br from-blue-500 to-indigo-600 text-white">
                        {(userDetails?.name || "U").charAt(0).toUpperCase()}
                      </AvatarFallback>
                    </Avatar>
                    <div className="absolute -bottom-2 -right-2 bg-green-500 text-white rounded-full p-2 shadow-lg">
                      <Shield className="h-4 w-4" />
                    </div>
                  </div>

                  <div className="text-center">
                    <h3 className="text-2xl font-bold text-slate-800 mb-1">
                      {userDetails?.name || "User"}
                    </h3>
                    <p className="text-slate-600 mb-4">
                      {userDetails?.email || "user@example.com"}
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Profile Details */}
          <div className="lg:col-span-2 space-y-6">
            {/* Personal Information */}
            <Card className="backdrop-blur-sm">
              <CardHeader className="border-b border-slate-200/50">
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle className="text-2xl font-semibold text-slate-800 flex items-center gap-3">
                      <User className="h-6 w-6 text-blue-600" />
                      Personal Information
                    </CardTitle>
                    <CardDescription className="text-slate-600">
                      Your account details and personal information
                    </CardDescription>
                  </div>
                  <Button
                    onClick={handleEditProfile}
                    className="text-white shadow-lg hover:shadow-xl transition-all duration-200"
                  >
                    <Edit className="mr-2 h-4 w-4" />
                    Edit Profile
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="pt-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="space-y-4">
                    <div className="flex items-center space-x-4 p-4 bg-slate-50 rounded-lg">
                      <div className="p-3 bg-blue-100 rounded-lg">
                        <User className="h-5 w-5 text-blue-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-slate-500">
                          Full Name
                        </p>
                        <p className="text-lg font-semibold text-slate-800">
                          {userDetails?.name || "Not provided"}
                        </p>
                      </div>
                    </div>

                    <div className="flex items-center space-x-4 p-4 bg-slate-50 rounded-lg">
                      <div className="p-3 bg-green-100 rounded-lg">
                        <Mail className="h-5 w-5 text-green-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-slate-500">
                          Email Address
                        </p>
                        <p className="text-lg font-semibold text-slate-800">
                          {userDetails?.email || "Not provided"}
                        </p>
                      </div>
                    </div>
                  </div>

                  <div className="space-y-4">
                    <div className="flex items-center space-x-4 p-4 bg-slate-50 rounded-lg">
                      <div className="p-3 bg-purple-100 rounded-lg">
                        <Calendar className="h-5 w-5 text-purple-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-slate-500">
                          Member Since
                        </p>
                        <p className="text-lg font-semibold text-slate-800">
                          {new Date().toLocaleDateString("en-US", {
                            year: "numeric",
                            month: "long",
                          })}
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Quick Actions */}
            {/* <Card className="backdrop-blur-sm">
              <CardHeader>
                <CardTitle className="text-xl font-semibold text-slate-800 flex items-center gap-3">
                  <Settings className="h-5 w-5 text-blue-600" />
                  Quick Actions
                </CardTitle>
                <CardDescription className="text-slate-600">
                  Manage your account and preferences
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <Button
                    onClick={handleEditProfile}
                    variant="outline"
                    className="h-16 justify-start text-left hover:bg-blue-50 hover:border-blue-200 transition-all duration-200"
                  >
                    <div className="flex items-center space-x-3">
                      <Edit className="h-5 w-5 text-blue-600" />
                      <div>
                        <p className="font-medium">Edit Profile</p>
                        <p className="text-sm text-slate-500">
                          Update your information
                        </p>
                      </div>
                    </div>
                  </Button>
                </div>
              </CardContent>
            </Card> */}
          </div>
        </div>
      </div>
    </div>
  );
}
