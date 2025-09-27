"use client";

import { useState, useEffect, useRef } from "react";
import { useRouter } from "next/navigation";
import { useSession } from "next-auth/react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import {
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Input,
  extractErrorMessage,
  FetchError,
} from "@distrbute/next-shared";
import { Label } from "@/components/ui/label";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { UserApi } from "@/lib/api/user";
import { ROUTES } from "@/lib/constants/routes";
import { ProfilePicture } from "@/types/user";
import { RefreshCw, X } from "lucide-react";

const onboardingSchema = z.object({
  name: z
    .string()
    .min(2, "Name must be at least 2 characters")
    .max(50, "Name must be less than 50 characters"),
});

type OnboardingFormData = z.infer<typeof onboardingSchema>;

export default function OnboardingPage() {
  const { data: session, update } = useSession();
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [profileImage, setProfileImage] = useState<string | null>(null);
  const [profilePicture, setProfilePicture] = useState<ProfilePicture | null>(
    null
  );
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm<OnboardingFormData>({
    resolver: zodResolver(onboardingSchema),
    defaultValues: {
      name: session?.user?.name || "",
    },
  });

  // Update form when session loads
  useEffect(() => {
    if (session?.user?.name) {
      setValue("name", session.user.name);
    }
  }, [session, setValue]);

  const onSubmit = async (data: OnboardingFormData) => {
    if (!session?.user?.accessToken) {
      toast.error("No access token found. Please sign in again.");
      return;
    }

    setIsLoading(true);

    try {
      // Create user profile
      const userRequest: any = {
        name: data.name,
      };

      // Add profile picture if available
      if (profileImage) {
        userRequest.profilePicture = {
          id: "temp-id", // This would be replaced with actual file upload logic
          fileType: "image/jpeg",
          filename: "profile.jpg",
          rawUrl: profileImage,
          size: 0,
          sizeReadable: "0 B",
          url: profileImage,
          thumbnail: profileImage,
        };
      }

      console.log(userRequest, "userRequest");
      const response = await UserApi.createUser(
        userRequest,
        session.user.accessToken
      );

      console.log(response, "response");

      if (response.code === 200) {
        toast.success("Profile created successfully!");

        // Update the session with the new user data
        await update({
          name: data.name,
          image: profileImage || session.user.image,
        });

        // Redirect to dashboard
        router.push(ROUTES.DASHBOARD.ROOT);
      } else {
        toast.error(response.message || "Failed to create profile");
      }
    } catch (error) {
      console.error("Profile creation failed:", error);
      if (error instanceof FetchError) {
        toast.error(extractErrorMessage(error));
      } else {
        toast.error("An unexpected error occurred");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleImageUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      setUploadError(null);

      const reader = new FileReader();
      reader.onload = (e) => {
        setProfileImage(e.target?.result as string);
      };
      reader.readAsDataURL(file);
      uploadProfilePicture(file);
    }
  };

  const retryUpload = async () => {
    if (!selectedFile) {
      toast.error("No file selected for retry. Please select a new image.");
      return;
    }

    uploadProfilePicture(selectedFile);
  };

  const clearError = () => {
    setUploadError(null);
    setSelectedFile(null);
    setProfileImage(null);
    setProfilePicture(null);

    // Reset the file input so new files can be selected
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const uploadProfilePicture = async (file: File) => {
    try {
      setIsUploading(true);
      setUploadError(null);

      // Prepare FormData for presigned URL request
      const formData = new FormData();
      formData.append("blob", file);
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
          ownerId: session?.user?.email || "unknown",
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
          setSelectedFile(null); // Clear selected file on success
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
      console.log("Error uploading profile picture: ", typeof error);
      const errorMsg = extractErrorMessage(error);
      setUploadError(errorMsg);
      toast.error(errorMsg);
      // Keep the preview - don't reset profileImage
    } finally {
      setIsUploading(false);
    }
  };

  if (!session?.user) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900">Loading...</h1>
          <p className="text-gray-600">
            Please wait while we load your session.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <div className="max-w-sm sm:max-w-md lg:max-w-lg xl:max-w-xl w-full">
        <Card className="w-full">
          <CardHeader className="px-4 sm:px-6">
            <CardTitle className="text-lg sm:text-xl">
              Complete Your Profile
            </CardTitle>
            <CardDescription className="text-sm sm:text-base">
              Complete your basic profile information
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4 sm:space-y-6 px-4 sm:px-6">
            {/* Display current session info */}
            <div className="bg-blue-50 p-3 sm:p-4 rounded-lg">
              <h3 className="font-medium text-blue-900 mb-2 text-sm sm:text-base">
                Account Information
              </h3>
              <div className="space-y-1 text-xs sm:text-sm text-blue-800">
                <p className="break-all">
                  <strong>Email:</strong> {session.user.email}
                </p>
              </div>
            </div>

            <form
              onSubmit={handleSubmit(onSubmit)}
              className="space-y-4 sm:space-y-6"
            >
              {/* Profile Picture */}
              <div className="space-y-2">
                <Label
                  htmlFor="profile-picture"
                  className="text-sm sm:text-base"
                >
                  Profile Picture
                </Label>
                <div className="flex flex-col items-center space-y-4">
                  <Avatar className="h-20 w-20 sm:h-24 sm:w-24">
                    <AvatarImage
                      src={profileImage || session.user.image || ""}
                    />
                    <AvatarFallback className="text-lg sm:text-xl">
                      {session.user.name?.charAt(0).toUpperCase() || "U"}
                    </AvatarFallback>
                  </Avatar>
                  <div className="flex flex-col items-center space-y-2">
                    <input
                      ref={fileInputRef}
                      id="profile-picture"
                      type="file"
                      accept="image/*"
                      onChange={handleImageUpload}
                      disabled={isUploading}
                      className="hidden"
                    />
                    <Button
                      type="button"
                      variant="outline"
                      disabled={isUploading}
                      onClick={() =>
                        document.getElementById("profile-picture")?.click()
                      }
                      className="w-full text-sm"
                    >
                      {isUploading
                        ? "Uploading..."
                        : profileImage
                        ? "Change Photo"
                        : "Upload Photo"}
                    </Button>
                    <p className="text-xs text-gray-500 mt-1">
                      JPG, PNG up to 2MB
                    </p>

                    {/* Error message and retry button */}
                    {uploadError && (
                      <div className="mt-3 p-2 bg-red-50 border border-red-200 rounded-lg w-full">
                        <p className="text-xs text-red-600 mb-2 text-center">
                          {uploadError}
                        </p>
                        <div className="flex gap-2 justify-center">
                          <Button
                            onClick={retryUpload}
                            disabled={isUploading}
                            size="sm"
                            variant="outline"
                            className="text-red-600 border-red-300 hover:bg-red-50 text-xs"
                          >
                            <RefreshCw className="mr-1 h-3 w-3" />
                            Retry Upload
                          </Button>
                          <Button
                            onClick={clearError}
                            disabled={isUploading}
                            size="sm"
                            variant="ghost"
                            className="text-gray-600 hover:bg-gray-100 text-xs"
                          >
                            <X className="mr-1 h-3 w-3" />
                            Clear
                          </Button>
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              </div>

              {/* Name */}
              <div className="space-y-2">
                <Label htmlFor="name" className="text-sm sm:text-base">
                  Display Name *
                </Label>
                <Input
                  id="name"
                  {...register("name")}
                  placeholder="Enter your display name"
                  className={`text-sm sm:text-base ${
                    errors.name ? "border-red-500" : ""
                  }`}
                />
                {errors.name && (
                  <p className="text-xs sm:text-sm text-red-600">
                    {errors.name.message}
                  </p>
                )}
              </div>

              {/* Submit Button */}
              <Button type="submit" className="w-full" disabled={isLoading}>
                {isLoading ? "Creating Profile..." : "Complete Setup"}
              </Button>
            </form>
          </CardContent>
        </Card>

        <div className="mt-6 text-center">
          <p className="text-sm text-gray-600">
            You can add more details later in your profile settings.
          </p>
        </div>
      </div>
    </div>
  );
}
