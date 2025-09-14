namespace App.Distrbute.Common;

public class CommonConstants
{
    public const string AUTHORIZATION = "Authorization";
    public const string AUTHORIZATION_COOKIE = "token";

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
}