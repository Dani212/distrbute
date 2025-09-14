namespace App.Distrbute.Api.Common.Dtos;

public class AuthUser
{
    public string Id { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int LedgerClientId { get; set; }
    public bool IsDeleted { get; set; }
}