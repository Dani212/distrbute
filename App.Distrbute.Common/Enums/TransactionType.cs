using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TransactionType
{
    Default = 0,

    [EnumMember(Value = "ActivateCampaignNewWallet")]
    ActivateCampaignNewWallet,

    [EnumMember(Value = "ActivateCampaignExistingWallet")]
    ActivateCampaignExistingWallet,
    [EnumMember(Value = "Withdrawal")] Withdrawal,
    [EnumMember(Value = "Payment")] Payment,
    [EnumMember(Value = "NewWallet")] NewWallet
}