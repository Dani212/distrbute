using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TransactionStatus
{
    Default = 0,
    [EnumMember(Value = "Pending")] Pending,
    [EnumMember(Value = "InProgress")] InProgress,
    [EnumMember(Value = "Challenged")] Challenged,
    [EnumMember(Value = "Successful")] Successful,

    [EnumMember(Value = "LockedForDeposit")]
    LockedForDeposit
}