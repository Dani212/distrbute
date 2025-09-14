using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TransactionProcessingStep
{
    Default = 0,

    [EnumMember(Value = "New transaction record was created")]
    TransactionRecordCreated,

    [EnumMember(Value = "Transaction was verified from payment processor")]
    VerifiedPaymentFromPaymentProcessor,

    [EnumMember(Value = "Retrieving existing wallet from database if exists")]
    RetrievingWalletFromDatabase,

    [EnumMember(Value = "Retrieved existing wallet from database")]
    RetrievedWalletFromDatabase,

    [EnumMember(Value = "Creating new wallet on ledger")]
    CreatingNewWalletOnLedger,

    [EnumMember(Value = "Created new wallet on ledger")]
    CreatedNewWalletOnLedger,

    [EnumMember(Value = "Creating new recipient with payment processor")]
    CreatingNewRecipientOnPaymentProcessor,

    [EnumMember(Value = "Created new recipient with payment processor")]
    CreatedNewRecipientOnPaymentProcessor,

    [EnumMember(Value = "Creating new wallet in database")]
    CreatingNewWalletInDb,

    [EnumMember(Value = "Created new wallet in database")]
    CreatedNewWalletInDb,

    [EnumMember(Value = "Depositing to wallet")]
    DepositingToWallet,

    [EnumMember(Value = "Deposited to wallet")]
    DepositedToWallet,
    [EnumMember(Value = "Successful")] Successful
}