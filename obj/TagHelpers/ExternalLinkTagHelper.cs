using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MamiaSeedsOil.Web.TagHelpers;

[HtmlTargetElement("a", Attributes = "asp-external")]
public sealed class ExternalLinkTagHelper : TagHelper
{
    [HtmlAttributeName("asp-external")]
    public bool IsExternal { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!IsExternal)
        {
            return;
        }

        output.Attributes.SetAttribute("target", "_blank");
        output.Attributes.SetAttribute("rel", "noopener noreferrer");
        output.Attributes.RemoveAll("asp-external");
    }
}
