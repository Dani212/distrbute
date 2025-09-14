using System.Runtime.Serialization;

namespace App.Distrbute.Common.Enums;

public enum Timeframe
{
    Default = 0,

    [EnumMember(Value = "Today")] Today = 1,
    [EnumMember(Value = "Yesterday")] Yesterday = 2,
    [EnumMember(Value = "LastWeek")] LastWeek = 7,
    [EnumMember(Value = "LastMonth")] LastMonth = 20,
    [EnumMember(Value = "Last90Days")] Last90Days = 90,
    [EnumMember(Value = "ThisYear")] ThisYear = 365
}

public enum Metric
{
    Default = 0,

    [EnumMember(Value = "Average")] Average,
    [EnumMember(Value = "Sum")] Sum
}