using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Common.Models;

public class Location
{
    [Required] public float Latitude { get; set; }

    [Required] public float Longitude { get; set; }

    [Required] public string Name { get; set; } = string.Empty;
}