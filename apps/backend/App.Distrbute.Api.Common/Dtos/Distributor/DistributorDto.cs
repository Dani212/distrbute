using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos.Distributor;

public class CreateDistributorDto
{
    [Required] 
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
    public string Name { get; set; } = null!;
    public DocumentFileDto? ProfilePicture { get; set; }
}

public class DistributorDto : CreateDistributorDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
}