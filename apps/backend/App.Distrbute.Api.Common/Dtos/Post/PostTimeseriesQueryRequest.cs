using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;
using Socials.Sdk.Enums;

namespace App.Distrbute.Api.Common.Dtos.Post;


public class PostTimeseriesQueryRequest
{
    [Required] public Timeframe? Timeframe { get; set; }
    public Platform? Platform { get; set; }
}