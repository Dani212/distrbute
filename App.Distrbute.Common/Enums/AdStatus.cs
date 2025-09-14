using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum AdStatus
{
    Default = 0,

    [EnumMember(Value = "Live")] Live,
    [EnumMember(Value = "Ended")] Ended
}

[JsonConverter(typeof(StringEnumConverter))]
public enum AdApprovalStatus
{
    Default = 0,

    [EnumMember(Value = "Pending")] Pending,
    [EnumMember(Value = "Approved")] Approved,
    [EnumMember(Value = "Disputed")] Disputed
}

[JsonConverter(typeof(StringEnumConverter))]
public enum AdPayoutStatus
{
    Default = 0,

    [EnumMember(Value = "Pending")] Pending,
    [EnumMember(Value = "Paid")] Paid,
    [EnumMember(Value = "InProgress")] InProgress,
    [EnumMember(Value = "Failed")] Failed
}