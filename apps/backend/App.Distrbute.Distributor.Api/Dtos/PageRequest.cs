using App.Distrbute.Common.Enums;
using Persistence.Sdk.Dtos;
using Socials.Sdk.Enums;

namespace App.Distrbute.Distributor.Api.Dtos;

public class CampaignInvitesPageRequest : PageRequest
{
    public CampaignType? CampaignType { get; set; }
    public Platform? Platform { get; set; }
    public CampaignInviteStatus? Status { get; set; }
    public bool PendingPost { get; set; }
}

public class PostPageRequest : PageRequest
{
    public string? InviteId { get; set; }
    public Platform? Platform { get; set; }
    public PostStatus? Status { get; set; }
    public PostApprovalStatus? ApprovalStatus { get; set; }
    public PostPayoutStatus? PayoutStatus { get; set; }
}

public class SocialAccountsPageRequest : PageRequest
{
    public Platform? Platform { get; set; }
}