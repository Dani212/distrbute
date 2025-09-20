using System.Collections.Generic;
using App.Distrbute.Common.Enums;
using Socials.Sdk.Enums;

namespace App.Distrbute.Distributor.Api.Dtos;

public class SocialAccountDto : SocialAccountPreferencesReq
{
    public string Id { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Handle { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public Platform? Platform { get; set; }
    public long FollowingCount { get; set; }
    public long FollowersCount { get; set; }
    public bool IsDisabled { get; set; }
    public IDictionary<CampaignType, IDictionary<ContentType, double>> Payouts { get; set; } = null!;
}