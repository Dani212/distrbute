using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum AudienceGender
{
    Default = 0,

    [EnumMember(Value = "Male")] Male,
    [EnumMember(Value = "Female")] Female
}