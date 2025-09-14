using System.Runtime.Serialization;

namespace App.Distrbute.Common.Enums;

public enum BrandInviteStatus
{
    Default = 0,

    [EnumMember(Value = "Pending")] Pending,
    [EnumMember(Value = "Accepted")] Accepted,
    [EnumMember(Value = "Expired")] Expired
}