namespace App.Distrbute.Distributor.Api.Dtos;

public class CampaignSummary
{
    public double Earnings { get; set; }
    public int Completed { get; set; }
    public int Active { get; set; }
    public int PendingApproval { get; set; }
    public int PendingSubmission { get; set; }
    public int Disputed { get; set; }
}