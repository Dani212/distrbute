using System;
using App.Distrbute.Common.Enums;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;

namespace App.Distrbute.Distributor.Api.Dtos;

public class PostDto
{
    public string Id { get; set; } = null!;
    public string? CampaignInviteId { get; set; } = null!;
    public string? BrandId { get; set; } = null!;
    public string? BrandName { get; set; } = null!;
    public string? BrandBio { get; set; } = null!;
    public Platform? Platform { get; set; }
    public double Payout { get; set; }
    public double PaidOut { get; set; }
    public PostStatus? PostStatus { get; set; }
    public PostApprovalStatus? PostApprovalStatus { get; set; }
    public PostPayoutStatus? PostPayoutStatus { get; set; }
    public string Link { get; set; } = null!;
    public double Views { get; set; }
    public long Comments { get; set; }
    public double Likes { get; set; }
    public double EngagementRate { get; set; }
    public Embedding? Embedding { get; set; }
    public DateTime CreatedAt { get; set; }
}