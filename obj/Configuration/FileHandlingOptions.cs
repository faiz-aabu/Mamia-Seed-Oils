namespace MamiaSeedsOil.Web.Configuration;

public sealed class FileHandlingOptions
{
    public const string SectionName = "FileHandling";

    public long MaxUploadBytes { get; set; } = 5_242_880;
    public string[] AllowedExtensions { get; set; } = [".pdf", ".png", ".jpg", ".jpeg", ".webp"];
    public string[] AllowedContentTypes { get; set; } = ["application/pdf", "image/png", "image/jpeg", "image/webp"];
}
