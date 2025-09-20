using Socials.Sdk.Enums;

namespace App.Distrbute.Distributor.Api.Dtos;

public class ConnectedPlatformsSummary
{
    public Platform? Platform { get; set; }
    public int AccountCount { get; set; }
    public long FollowersCount { get; set; }
    public long FollowingCount { get; set; }
}