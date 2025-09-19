using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;
using Redact.Sdk.Attributes;
using Rest.Sdk.Core;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Models;

[Redactable]
public class SocialAccountBase : BaseModel
{
    [Required] public string DisplayName { get; set; } = null!;
    [Required] public string Handle { get; set; } = null!;
    
    public string? ProfileImageUrl { get; set; }
    [Required] public string ExternalId { get; set; } = null!;
    [Required] public Platform? Platform { get; set; }
    [Required] public string ProfileLink { get; set; } = null!;

    public long FollowingCount { get; set; }
    public long FollowersCount { get; set; }

    [Required, Redacted] public string AccessToken { get; set; } = null!;
    [Required, Redacted] public string RefreshToken { get; set; } = null!;
    [Required] public DateTime? AccessTokenExpiry { get; set; } = null!;
    [Required] public DateTime? RefreshTokenExpiry { get; set; } = null!;
    [Required] public OAuthTokenKind? OAuthTokenKind { get; set; } = null!;

    [Required] public DateTime? LastSynced { get; set; }
    
    public bool IsDisabled { get; set; }
}

public static class SocialAccountExtensions
{
    public static SocialProfile AsSocialProfile(this SocialAccountBase socialAccount)
    {
        var socialProfile = new SocialProfile();

        socialProfile.UserId = socialAccount.Id;
        socialProfile.Handle = socialAccount.Handle;
        socialProfile.DisplayName = socialAccount.DisplayName;
        socialProfile.ProfileImageUrl = socialAccount.ProfileImageUrl;
        socialProfile.ExternalId = socialAccount.ExternalId;
        socialProfile.Platform = socialAccount.Platform;
        socialProfile.FollowersCount = socialAccount.FollowersCount;
        socialProfile.FollowingCount = socialAccount.FollowingCount;

        socialProfile.AccessToken = socialAccount.AccessToken;
        socialProfile.RefreshToken = socialAccount.RefreshToken;
        socialProfile.AccessTokenExpiry = socialAccount.AccessTokenExpiry!.Value;
        socialProfile.RefreshTokenExpiry = socialAccount.RefreshTokenExpiry!.Value;

        return socialProfile;
    }
}