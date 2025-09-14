using System.Runtime.Serialization;

namespace App.Distrbute.Common.Enums;

public enum BrandRole
{
    Default = 0,

    [EnumMember(Value = "Owner")] Owner,
    [EnumMember(Value = "Manager")] Manager,
    [EnumMember(Value = "Analyst")] Analyst
}