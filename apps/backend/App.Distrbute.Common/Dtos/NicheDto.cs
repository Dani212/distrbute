using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Common.Dtos;

public class NicheDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Name must not be empty.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters with no spaces.")]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(1, ErrorMessage = "Description must not be empty.")]
    public string Description { get; set; } = null!;
}