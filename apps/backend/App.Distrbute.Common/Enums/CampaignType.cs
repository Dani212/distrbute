using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum CampaignType
{
    Default = 0,
    [EnumMember(Value = "Broadcast")] Broadcast,
    [EnumMember(Value = "UGC")] UGC
}

[JsonConverter(typeof(StringEnumConverter))]
public enum CampaignSubType
{
    Default = 0,
    [EnumMember(Value = "Budget")] Budget,
    [EnumMember(Value = "Reach")] Reach
}