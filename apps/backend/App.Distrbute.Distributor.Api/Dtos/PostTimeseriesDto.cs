using App.Distrbute.Api.Common.Dtos.Post;

namespace App.Distrbute.Distributor.Api.Dtos;

public class PostTimeseriesSlot : PostTimeseriesSlotBase
{
    public double Earnings { get; set; }
    public double AvgEarnings { get; set; }
}