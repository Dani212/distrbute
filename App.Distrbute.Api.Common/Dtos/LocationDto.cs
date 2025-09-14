using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos;

public class LocationDto
{
    public float Latitude { get; set; }

    public float Longitude { get; set; }

    [Required] public string Name { get; set; } = null!;
}