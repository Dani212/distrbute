using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ObjectStorage.Sdk.Dtos;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public abstract class BaseBrandDistributor : BaseModel
{
    public int RelevanceScore { get; set; } = 1;
    [Required] public string Name { get; set; }
    [Required] public Email Email { get; set; }
    [Column(TypeName = "jsonb")] public DocumentFile? ProfilePicture { get; set; }
    [Column(TypeName = "jsonb")] public Location? Location { get; set; }
}