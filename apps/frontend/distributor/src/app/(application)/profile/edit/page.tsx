"use client";

import { useState, useRef, useEffect } from "react";
import { useForm } from "react-hook-form";
import { useAuth } from "@/hooks/use-auth";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  ArrowLeft,
  Camera,
  Loader2,
  User,
  Mail,
  Save,
  X,
  RefreshCw,
} from "lucide-react";
import { useRouter } from "next/navigation";
import { ROUTES } from "@/lib/constants/routes";
import { UserApi } from "@/lib/api/user";
import { toast } from "sonner";
import { extractErrorMessage } from "@distrbute/next-shared";
import { ProfilePicture } from "@/types/user";

interface ProfileFormData {
  name: string;
  email: string;
}

export default function EditProfilePage() {
  const { user, isLoading: isLoadingAuth, update } = useAuth();
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [profileImage, setProfileImage] = useState<string | null>(
    user?.image || null
  );
  const [profilePicture, setProfilePicture] = useState<ProfilePicture | null>(
    user?.profilePicture || null
  );
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  // Track original values to detect changes
  const originalName = user?.name || "";
  const originalImage = user?.image || null;

  const form = useForm<ProfileFormData>({
    defaultValues: {
      name: user?.name || "",
      email: user?.email || "",
    },
  });

  useEffect(() => {
    setProfileImage(user?.image || null);
    setProfilePicture(user?.profilePicture || null);
    form.setValue("name", user?.name || "");
    form.setValue("email", user?.email || "");
  }, [user]);

  // Check if any changes have been made
  const hasChanges = () => {
    const currentName = form.watch("name");
    const nameChanged = currentName !== originalName;
    const imageChanged = profileImage !== originalImage;
    return nameChanged || imageChanged;
  };

  const handleImageChange = async (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);

      try {
        setIsLoading(true);
        setUploadError(null);

        // First, show preview
        const reader = new FileReader();
        reader.onload = (e) => {
          const result = e.target?.result as string;
          setProfileImage(result);
        };
        reader.readAsDataURL(file);

        uploadProfilePicture(file);
      } catch (error) {
        console.error("Error uploading profile picture:", error);
        setUploadError(extractErrorMessage(error));
        toast.error(extractErrorMessage(error));
      }
    }
  };

  const retryUpload = async () => {
    if (!selectedFile) {
      toast.error("No file selected for retry. Please select a new image.");
      return;
    }

    uploadProfilePicture(selectedFile);
  };

  const uploadProfilePicture = async (file: File) => {
    try {
      setIsLoading(true);
      setUploadError(null);

      // Prepare FormData for presigned URL request
      const formData = new FormData();
      formData.append("file", file);
      formData.append("fileName", file.name);
      formData.append("fileType", file.type);
      formData.append("size", file.size.toString());

      // Generate presigned URL
      const presignedResponse =
        await UserApi.generatePresignedUrlProfilePicture(formData);

      if (presignedResponse.data) {
        const { uploadUrl, filename, id } = presignedResponse.data;

        console.log(uploadUrl, "uploadUrl");
        console.log(filename, "filename");
        console.log(id, "id");

        // Upload file to presigned URL
        const uploadResponse = await UserApi.uploadProfilePicture({
          ownerId: user?.email || "unknown",
          file: file,
          fileName: filename,
          url: uploadUrl,
          isPublic: true,
          createdAt: new Date().toISOString(),
        });

        if (uploadResponse.ok) {
          // Update profile image with the uploaded URL
          setProfileImage(presignedResponse.data.uploadUrl);
          setProfilePicture({
            id: id,
            fileType: file.type,
            filename: filename,
            rawUrl: presignedResponse.data.url,
            size: file.size,
            sizeReadable: file.size.toString(),
            url: presignedResponse.data.url,
            thumbnail: presignedResponse.data.url,
          });
          toast.success("Profile picture uploaded successfully!");
          console.log("Profile picture uploaded successfully");
        } else {
          console.error("Failed to upload profile picture");
          const errorMsg =
            "Failed to upload profile picture. Please try again.";
          setUploadError(errorMsg);
          toast.error(errorMsg);
          // Keep the preview - don't reset profileImage
        }
      } else {
        console.error("Failed to generate presigned URL");
        const errorMsg = "Failed to generate upload URL. Please try again.";
        setUploadError(errorMsg);
        toast.error(errorMsg);
        // Keep the preview - don't reset profileImage
      }
    } catch (error) {
      console.log("Error uploading profile picture:");
      const errorMsg = extractErrorMessage(error);
      setUploadError(errorMsg);
      toast.error(errorMsg);
      // Keep the preview - don't reset profileImage
    } finally {
      setIsLoading(false);
    }
  };

  const onSubmit = async (data: ProfileFormData) => {
    setIsLoading(true);

    const userRequest: any = {
      name: data.name,
      profilePicture: profilePicture,
    };

    UserApi.updateUser(userRequest)
      .then(async () => {
        toast.success("Profile updated successfully!");
        await update({
          ...user,
          name: data.name,
          image: profileImage,
          profilePicture: profilePicture,
        });
        toast.success("Profile updated successfully!");

        // Navigate back to dashboard
        router.push(ROUTES.DASHBOARD.PROFILE);
      })
      .catch((error) => {
        console.error("Error updating profile:", error);
        toast.error(extractErrorMessage(error));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleBack = () => {
    router.back();
  };

  const clearError = () => {
    setUploadError(null);
    setSelectedFile(null);
    // Reset profile image to original when clearing error
    setProfileImage(originalImage);

    // Reset the file input so new files can be selected
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  if (isLoadingAuth) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-gray-600">Loading...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      <div className="max-w-7xl mx-auto px-3 sm:px-4 lg:px-6 xl:px-8 py-4 sm:py-6 xl:py-8">
        {/* Header Section */}
        <div className="mb-6 xl:mb-8">
          <div className="flex items-center justify-between mb-4 xl:mb-6">
            <Button
              variant="ghost"
              onClick={handleBack}
              className="group flex items-center gap-2 text-slate-600 hover:text-slate-900 hover:bg-white/50 transition-all duration-200"
            >
              <ArrowLeft className="h-4 w-4 group-hover:-translate-x-1 transition-transform" />
              Back
            </Button>
          </div>

          <div className="">
            <h1
              className="text-2xl xl:text-3xl font-bold bg-gradient-to-r from-slate-900
              via-blue-900 to-indigo-900 bg-clip-text text-transparent mb-0.5"
            >
              Edit Profile
            </h1>
            <p className="text-sm xl:text-md text-slate-600 max-w-2xl">
              Personalize your account and make it uniquely yours
            </p>
          </div>
        </div>

        <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 lg:gap-8">
          {/* Profile Image Section */}
          <div className="xl:col-span-1">
            <Card className="backdrop-blur-sm">
              <CardHeader className="text-center pb-4">
                <CardTitle className="text-lg xl:text-xl font-semibold text-slate-800">
                  Profile Picture
                </CardTitle>
                <CardDescription className="text-sm">
                  Your photo helps others recognize you
                </CardDescription>
              </CardHeader>
              <CardContent className="flex flex-col items-center space-y-4 lg:space-y-6">
                <div className="relative group">
                  <Avatar
                    className={`h-24 w-24 sm:h-28 sm:w-28 lg:h-32 lg:w-32 ring-4 ring-white shadow-2xl ${
                      uploadError ? "ring-2 ring-orange-400" : ""
                    }`}
                  >
                    <AvatarImage
                      src={profilePicture?.thumbnail || "avatar.png"}
                      alt={form.watch("name") || "Profile"}
                      className="object-cover"
                    />
                    <AvatarFallback className="text-3xl font-bold bg-gradient-to-br from-blue-500 to-indigo-600 text-white">
                      {(form.watch("name") || "U").charAt(0).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>

                  {/* Preview indicator when there's an error */}
                  {uploadError && (
                    <div className="absolute -top-2 -left-2 bg-orange-500 text-white text-xs px-2 py-1 rounded-full font-medium">
                      Preview
                    </div>
                  )}

                  <label
                    htmlFor="profile-image"
                    className={`absolute -bottom-1 -right-1 xl:-bottom-2 xl:-right-2 bg-gradient-to-r from-blue-500 to-indigo-600 text-white rounded-full p-2 xl:p-3 cursor-pointer hover:from-blue-600 hover:to-indigo-700 transition-all duration-200 shadow-lg hover:shadow-xl group-hover:scale-110 ${
                      isLoading ? "opacity-50 cursor-not-allowed" : ""
                    }`}
                  >
                    {isLoading ? (
                      <Loader2 className="h-4 w-4 xl:h-5 xl:w-5 animate-spin" />
                    ) : (
                      <Camera className="h-4 w-4 xl:h-5 xl:w-5" />
                    )}
                    <span className="sr-only">Change profile image</span>
                  </label>

                  <input
                    ref={fileInputRef}
                    id="profile-image"
                    type="file"
                    accept="image/*"
                    onChange={handleImageChange}
                    disabled={isLoading}
                    className="hidden"
                  />
                </div>

                <div className="text-center">
                  <p className="text-xs xl:text-sm text-slate-600 mb-2">
                    {isLoading
                      ? "Uploading..."
                      : "Click the camera icon to upload a new photo"}
                  </p>
                  <p className="text-xs text-slate-400">
                    JPG, PNG or GIF. Max size 5MB.
                  </p>

                  {/* Error message and retry button */}
                  {uploadError && (
                    <div className="mt-3 xl:mt-4 p-2 xl:p-3 bg-red-50 border border-red-200 rounded-lg">
                      <p className="text-xs xl:text-sm text-red-600 mb-2 xl:mb-3">
                        {uploadError}
                      </p>
                      <div className="flex flex-col sm:flex-row gap-2 justify-center">
                        <Button
                          onClick={retryUpload}
                          disabled={isLoading}
                          size="sm"
                          variant="outline"
                          className="text-red-600 border-red-300 hover:bg-red-50 text-xs xl:text-sm"
                        >
                          <RefreshCw className="mr-1 xl:mr-2 h-3 w-3 xl:h-4 xl:w-4" />
                          Retry Upload
                        </Button>
                        <Button
                          onClick={clearError}
                          disabled={isLoading}
                          size="sm"
                          variant="ghost"
                          className="text-gray-600 hover:bg-gray-100 text-xs xl:text-sm"
                        >
                          <X className="mr-1 xl:mr-2 h-3 w-3 xl:h-4 xl:w-4" />
                          Clear
                        </Button>
                      </div>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Form Section */}
          <div className="xl:col-span-2">
            <Card className="backdrop-blur-sm">
              <CardHeader className="border-b border-slate-200/50">
                <CardTitle className="text-xl xl:text-2xl font-semibold text-slate-800 flex items-center gap-2 xl:gap-3">
                  <User className="h-5 w-5 xl:h-6 xl:w-6 text-blue-600" />
                  Personal Information
                </CardTitle>
                <CardDescription className="text-sm xl:text-base text-slate-600">
                  Update your details to keep your profile current
                </CardDescription>
              </CardHeader>

              <CardContent className="pt-6">
                <Form {...form}>
                  <form
                    onSubmit={form.handleSubmit(onSubmit)}
                    className="space-y-8"
                  >
                    {/* Name Field */}
                    <FormField
                      control={form.control}
                      name="name"
                      rules={{
                        required: "Name is required",
                        minLength: {
                          value: 2,
                          message: "Name must be at least 2 characters",
                        },
                      }}
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-sm xl:text-base font-medium text-slate-700 flex items-center gap-2">
                            <User className="h-4 w-4" />
                            Full Name
                          </FormLabel>
                          <FormControl>
                            <Input
                              placeholder="Enter your full name"
                              {...field}
                              disabled={isLoading}
                              className="h-10 xl:h-12 text-sm xl:text-base border-slate-300 focus:border-blue-500 focus:ring-blue-500/20 transition-all duration-200"
                            />
                          </FormControl>
                          <FormMessage className="text-red-500" />
                        </FormItem>
                      )}
                    />

                    {/* Email Field */}
                    <FormField
                      control={form.control}
                      name="email"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-sm xl:text-base font-medium text-slate-700 flex items-center gap-2">
                            <Mail className="h-4 w-4" />
                            Email Address
                          </FormLabel>
                          <FormControl>
                            <Input
                              {...field}
                              disabled
                              className="h-10 xl:h-12 text-sm xl:text-base bg-slate-50 text-slate-500 border-slate-200"
                            />
                          </FormControl>
                          <div className="flex items-start gap-2 mt-2 p-3 bg-amber-50 border border-amber-200 rounded-lg">
                            <div className="w-1 h-1 bg-amber-500 rounded-full mt-2 flex-shrink-0"></div>
                            <p className="text-xs xl:text-sm text-amber-700">
                              Email cannot be changed. Contact support if you
                              need to update your email address.
                            </p>
                          </div>
                        </FormItem>
                      )}
                    />

                    {/* Action Buttons */}
                    <div className="flex flex-col sm:flex-row gap-3 xl:gap-4 pt-6 xl:pt-8 border-t border-slate-200/50">
                      <Button
                        type="button"
                        variant="outline"
                        onClick={handleBack}
                        disabled={isLoading}
                        className="flex-1 h-10 xl:h-12 text-sm xl:text-base font-medium border-slate-300 text-slate-700 hover:bg-slate-50 hover:border-slate-400 transition-all duration-200"
                      >
                        <X className="mr-2 h-4 w-4" />
                        Cancel
                      </Button>
                      <Button
                        type="submit"
                        disabled={isLoading || !hasChanges()}
                        className="flex-1 h-10 xl:h-12 text-sm xl:text-base font-medium bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                        {isLoading ? (
                          <>
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            Saving Changes...
                          </>
                        ) : (
                          <>
                            <Save className="mr-2 h-4 w-4" />
                            Save Changes
                          </>
                        )}
                      </Button>
                    </div>
                  </form>
                </Form>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}
