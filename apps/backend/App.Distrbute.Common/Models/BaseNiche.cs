using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class BaseNiche : BaseModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }
}