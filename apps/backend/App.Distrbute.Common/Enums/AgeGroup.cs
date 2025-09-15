using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum AgeGroup
{
    Default = 0,

    [EnumMember(Value = "Under 18")] Under18,
    [EnumMember(Value = "18-24")] Between18And24,
    [EnumMember(Value = "25-34")] Between25And34,
    [EnumMember(Value = "25-44")] Between35And44,
    [EnumMember(Value = "45-54")] Between45And54,
    [EnumMember(Value = "Above 55")] Above55
}