using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MamiaSeedsOil.Web.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NoHtmlAttribute : ValidationAttribute
{
    private static readonly Regex HtmlPattern = new("<[^>]+>", RegexOptions.Compiled);

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is string text && !HtmlPattern.IsMatch(text);
    }
}
