using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Extensions;

public static class ContentTypeExtensions
{
    public static readonly List<ContentType> All =
    [
        ContentType.Story,
        ContentType.Reel,
        ContentType.Short,
        ContentType.Post
    ];

    private static readonly int PAYOUT_AFTER_HOURS_EPHEMERAL = 25;
    private static readonly int POST_AUTO_APPROVAL_HOURS_EPHEMERAL = PAYOUT_AFTER_HOURS_EPHEMERAL - 1;
    private static readonly int PAYOUT_AFTER_HOURS_PERMANENT = 73;
    private static readonly int POST_AUTO_APPROVAL_HOURS_PERMANENT = PAYOUT_AFTER_HOURS_PERMANENT - 1;

    public static List<Platform> EligiblePlatforms(this ContentType contentType)
    {
        return contentType switch
        {
            // ephemeral
            ContentType.Story => new List<Platform>
            {
                Platform.Instagram,
                Platform.Facebook,
                Platform.Snapchat
            },


            // permanent
            ContentType.Reel => new List<Platform>
            {
                Platform.Instagram,
                Platform.Facebook
            },

            ContentType.Short => new List<Platform>
            {
                Platform.YouTube,
                Platform.TikTok
            },

            ContentType.Post => new List<Platform>
            {
                Platform.YouTube,
                Platform.Facebook,
                Platform.Instagram,
                Platform.LinkedIn,
                Platform.Twitter,
                Platform.TikTok
            },

            _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null)
        };
    }

    public static DateTime GetPayoutTime(this ContentType contentType, DateTime submittedAt)
    {
        return contentType switch
        {
            ContentType.Story => submittedAt.AddHours(PAYOUT_AFTER_HOURS_EPHEMERAL),
            _ => submittedAt.AddHours(PAYOUT_AFTER_HOURS_PERMANENT)
        };
    }

    public static DateTime GetAutoApprovalTime(this ContentType contentType, DateTime submittedAt)
    {
        return contentType switch
        {
            ContentType.Story => submittedAt.AddHours(POST_AUTO_APPROVAL_HOURS_EPHEMERAL),
            _ => submittedAt.AddHours(POST_AUTO_APPROVAL_HOURS_PERMANENT)
        };
    }

    public static string GetReviewWindow(this ContentType contentType)
    {
        return contentType switch
        {
            ContentType.Story => $"{POST_AUTO_APPROVAL_HOURS_EPHEMERAL} hours",

            _ => $"{POST_AUTO_APPROVAL_HOURS_PERMANENT} hours"
        };
    }

    public static List<TimeSpan> GetTrackingPeriods(this ContentType contentType)
    {
        return contentType switch
        {
            ContentType.Story => new List<TimeSpan>
            {
                TimeSpan.FromHours(6), // early spike
                TimeSpan.FromHours(22) // just before expiry (~24h)
            },

            _ => new List<TimeSpan>
            {
                TimeSpan.FromHours(24), // day 1 spike
                TimeSpan.FromHours(71) // day 3 tail (final)
            }
        };
    }
}