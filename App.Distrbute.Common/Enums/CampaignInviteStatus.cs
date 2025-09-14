using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum CampaignInviteStatus
{
    Default = 0,

    [EnumMember(Value = "Pending")] Pending,
    [EnumMember(Value = "Accepted")] Accepted,
    [EnumMember(Value = "Declined")] Declined,
    [EnumMember(Value = "Expired")] Expired
}