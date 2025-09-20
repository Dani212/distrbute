using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Distributor.Api.Dtos;

public class CreatePostReq
{
    [Required] public string InviteId { get; set; } = null!;
    [Required] public string Link { get; set; } = null!;
}