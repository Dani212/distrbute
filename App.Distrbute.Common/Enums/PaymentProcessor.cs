using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum PaymentProcessor
{
    Default = 0,
    [EnumMember(Value = "PayStack")] PayStack,
    [EnumMember(Value = "Distrbute")] Distrbute
}