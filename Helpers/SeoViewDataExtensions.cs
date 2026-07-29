namespace MamiaSeedsOil.Web.Helpers;

public static class SeoViewDataExtensions
{
    public static string GetString(this IDictionary<string, object?> viewData, string key, string fallback = "")
    {
        return viewData.TryGetValue(key, out var value) && value is not null
            ? value.ToString() ?? fallback
            : fallback;
    }
}
