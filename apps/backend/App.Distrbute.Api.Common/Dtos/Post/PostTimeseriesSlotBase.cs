namespace App.Distrbute.Api.Common.Dtos.Post;

public class PostTimeseriesSlotBase
{
    public long Likes { get; set; }
    public double AvgLikes { get; set; }
    public long Views { get; set; }
    public double AvgViews { get; set; }
    public long Comments { get; set; }
    public double AvgComments { get; set; }
    public double EngagementRate { get; set; }
    public double AvgEngagementRate { get; set; }
    public DateTime? Date { get; set; }
}