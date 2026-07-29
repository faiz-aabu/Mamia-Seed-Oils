namespace MamiaSeedsOil.Web.Helpers;

public static class PrivacyMasker
{
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return "[empty]";
        }

        var parts = email.Split('@', 2);
        if (parts.Length != 2)
        {
            return "[invalid-email]";
        }

        var name = parts[0];
        var maskedName = name.Length switch
        {
            <= 1 => "*",
            2 => $"{name[0]}*",
            _ => $"{name[0]}***{name[^1]}"
        };

        return $"{maskedName}@{parts[1]}";
    }
}
