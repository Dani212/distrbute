using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos.Distributor;

public class CreateDistributorDto
{
    [Required] public string Name { get; set; } = null!;
    public DocumentFileDto? ProfilePicture { get; set; }
}

public class DistributorDto : CreateDistributorDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
}