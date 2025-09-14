using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos;

public class DocumentFileDto
{
    [Required(AllowEmptyStrings = false)] public string Id { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)] public string FileType { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)] public string Filename { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)] public string Url { get; set; } = string.Empty;
    [Range(0, long.MaxValue)] public long Size { get; set; }
    [Required(AllowEmptyStrings = false)] public string SizeReadable { get; set; } = string.Empty;
    public string Thumbnail { get; set; }
    public Dictionary<string, string> OtherFormats { get; set; } = new();
    public DateTime? UploadedAt { get; set; }
}