using ObjectStorage.Sdk.Dtos;

namespace App.Distrbute.Common;

public class CommonConstants
{
    public const string AUTHORIZATION = "Authorization";
    public const string AUTHORIZATION_COOKIE = "token";
    public const string ES_OUTBOX_PREFIX = "outbox.event.";
    public const string PROFILE_PICTURE_STORAGE_PREFIX = "logo";
    public const string CONTENT_STORAGE_PREFIX = "content";

    public static MediaProcessingReq DistrbuteMediaProcessingOptions()
    {
        return new MediaProcessingReq();
    }

    public static class AuthScheme
    {
        public const string BEARER = "Bearer";
    }

    public static class RedisKeys
    {
        public const string OtpCacheKey = "distrbute:otp-cache";
        public const string UserDetailCacheKey = "distrbute:user-detail-cache";
        public const string CampaignEstimateCacheKey = "distrbute:campaign-estimate-cache";
    }

    public static class IntegrationChannel
    {
        public const string BrandPortal = "distrbute-brand-portal";
        public const string DistributorPortal = "distrbute-distributor-portal";
        public const string Platform = "distrbute-platform";
    }

    public static class AcceptedContentMediaTypes
    {
        public static readonly List<string> Types = AcceptedProfilePictureMediaTypes.Types.Concat(
        [
            ".mp4"
        ]).ToList();
    }

    public static class AcceptedProfilePictureMediaTypes
    {
        public static readonly List<string> Types =
        [
            ".png",
            ".jpg",
            ".jpeg",
            ".heic"
        ];
    }

    public static class TypesToBeConvertedToJpg
    {
        public static readonly List<string> Types =
        [
            ".heic"
        ];
    }

    public static class GraphLabel
    {
        public const string Age = "Age";
        public const string Gender = "Gender";
        public const string Location = "Location";
        public const string Demographic = "Demographic";
    }
}