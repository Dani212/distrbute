using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum CampaignStatus
{
    Default = 0,

    [EnumMember(Value = "Pending")] Pending,
    [EnumMember(Value = "Activated")] Activated,
    [EnumMember(Value = "Ended")] Ended
}