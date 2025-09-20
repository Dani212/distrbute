using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Distributor.Api.Dtos;

public class PostsQueryRequest
{
    [Required] public string CampaignInviteId { get; set; } = null!;
}