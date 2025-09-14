using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Options;

public class MailTemplateConfig
{
    [Required(AllowEmptyStrings = false)] public string OtpTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string CampaignInviteTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string AdPaymentTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string AdPostedTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string AdBrandApprovedTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string AdSystemApprovedTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string AdBrandDisputedTemplateId { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string InvitedToCollaborate { get; set; } = null!;
}