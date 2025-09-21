using App.Distrbute.Common.Models;
using Ledgr.Sdk.Dtos;
using Ledgr.Sdk.Dtos.Wallet.Transfer;

namespace App.Distrbute.Api.Common.Dtos.Wallet;

public class PayoutProcessingContext
{
    public string PostId { get; set; }
}

public class RetrievedPost : PayoutProcessingContext
{
    public Distrbute.Common.Models.Post PlatformPost { get; set; }
}

public class InitializedPost : RetrievedPost
{
}

public class PostWithCampaign : InitializedPost
{
    public Distrbute.Common.Models.Campaign Campaign { get; set; }
}

public class PostWithTransaction : PostWithCampaign
{
    public DistrbuteTransaction Transaction { get; set; }
}

public class PostWithResolvedWallets : PostWithTransaction
{
    public Depository SourceWallet { get; set; }
    public Depository DestinationWallet { get; set; }
}

public class PostWithTransferRequest : PostWithResolvedWallets
{
    public TransferRequest TransferReq { get; set; }
}

public class ExecutedTransfer : PostWithResolvedWallets
{
    public BaseFineractResponseDto TransferResponse { get; set; }
}

public class CompletedPayout : ExecutedTransfer
{
    public bool EmailSent { get; set; }
}