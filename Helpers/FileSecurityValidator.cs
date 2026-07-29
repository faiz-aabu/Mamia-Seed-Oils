using MamiaSeedsOil.Web.Configuration;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Helpers;

public interface IFileSecurityValidator
{
    bool IsAllowed(string? fileName, string? contentType, long fileSizeBytes);
    string SanitizeFileName(string? fileName);
}

public sealed class FileSecurityValidator : IFileSecurityValidator
{
    private readonly FileHandlingOptions _options;

    public FileSecurityValidator(IOptions<FileHandlingOptions> options)
    {
        _options = options.Value;
    }

    public bool IsAllowed(string? fileName, string? contentType, long fileSizeBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType) || fileSizeBytes <= 0)
        {
            return false;
        }

        if (fileSizeBytes > _options.MaxUploadBytes)
        {
            return false;
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        var isExtensionAllowed = _options.AllowedExtensions.Any(x => string.Equals(x, extension, StringComparison.OrdinalIgnoreCase));
        var isTypeAllowed = _options.AllowedContentTypes.Any(x => string.Equals(x, contentType, StringComparison.OrdinalIgnoreCase));

        return isExtensionAllowed && isTypeAllowed;
    }

    public string SanitizeFileName(string? fileName)
    {
        var safeName = Path.GetFileName(fileName ?? string.Empty);
        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(invalid, '_');
        }

        return string.IsNullOrWhiteSpace(safeName)
            ? $"upload-{Guid.NewGuid():N}"
            : safeName;
    }
}
