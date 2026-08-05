using System.ComponentModel.DataAnnotations;

namespace MamiaSeedsOil.Web.Configuration;

public sealed class WebsiteContentOptions
{
    public const string SectionName = "WebsiteContent";

    [Required]
    public CompanyInfoOptions CompanyInfo { get; set; } = new();

    [Required]
    public SeoOptions Seo { get; set; } = new();

    [Required]
    public HomePageContentOptions HomePage { get; set; } = new();

    [Required]
    public PlaceholderRegistryOptions Placeholders { get; set; } = new();
}

public sealed class PlaceholderRegistryOptions
{
    [Required]
    public Dictionary<string, PlaceholderAssetOptions> Assets { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class PlaceholderAssetOptions
{
    [Required]
    public string Label { get; set; } = string.Empty;

    [Required]
    public string SubLabel { get; set; } = string.Empty;

    [Required]
    public string AltText { get; set; } = string.Empty;

    [Required]
    public string AspectRatio { get; set; } = "4 / 3";
}

public sealed class CompanyInfoOptions
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string AddressLine1 { get; set; } = string.Empty;

    [Required]
    public string AddressLine2 { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Region { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string BusinessHours { get; set; } = string.Empty;

    [Required]
    public string WhatsAppUrl { get; set; } = string.Empty;

    [Required]
    public string GoogleMapsEmbedUrl { get; set; } = string.Empty;

    public Dictionary<string, string> SocialLinks { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class SeoOptions
{
    [Required]
    public string SiteTitle { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Keywords { get; set; } = string.Empty;

    [Required]
    public string CanonicalUrl { get; set; } = string.Empty;

    [Required]
    public string OgImageUrl { get; set; } = string.Empty;

    [Required]
    public string TwitterImageUrl { get; set; } = string.Empty;

    [Required]
    public string SchemaLogoUrl { get; set; } = string.Empty;
}

public sealed class HomePageContentOptions
{
    [Required]
    public HeroOptions Hero { get; set; } = new();

    [Required]
    public HistoryOptions History { get; set; } = new();

    [Required]
    public List<SimpleCardOptions> TrustBadges { get; set; } = [];

    [Required]
    public List<SimpleCardOptions> ProcessSteps { get; set; } = [];

    [Required]
    public List<ProductOptions> Products { get; set; } = [];

    [Required]
    public CtaOptions DistributorCta { get; set; } = new();

    [Required]
    public List<SimpleCardOptions> GalleryItems { get; set; } = [];

    [Required]
    public ContactOptions Contact { get; set; } = new();

    [Required]
    public List<SimpleCardOptions> WhyChooseUs { get; set; } = [];

    [Required]
    public List<SimpleCardOptions> Industries { get; set; } = [];

    [Required]
    public List<StatOptions> CompanyStats { get; set; } = [];

    [Required]
    public List<SimpleCardOptions> QualityTimeline { get; set; } = [];

    [Required]
    public DistributionOptions Distribution { get; set; } = new();

    [Required]
    public PartnershipCentreOptions PartnershipCentre { get; set; } = new();

    [Required]
    public List<FaqOptions> Faqs { get; set; } = [];

    [Required]
    public List<TestimonialOptions> Testimonials { get; set; } = [];

    [Required]
    public List<InsightOptions> Insights { get; set; } = [];

    [Required]
    public CtaOptions CorporateCta { get; set; } = new();

    [Required]
    public FooterOptions Footer { get; set; } = new();
}

public sealed class PartnershipCentreOptions
{
    [Required]
    public string Eyebrow { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public List<SimpleCardOptions> Pillars { get; set; } = [];

    [Required]
    public List<string> PartnerTypes { get; set; } = [];

    [Required]
    public List<SelectOption> BusinessTypeOptions { get; set; } = [];

    [Required]
    public List<SelectOption> YearsInOperationOptions { get; set; } = [];

    [Required]
    public List<SelectOption> PreferredProductOptions { get; set; } = [];

    [Required]
    public List<SelectOption> PreferredPackagingOptions { get; set; } = [];

    [Required]
    public List<DocumentRequirementOption> FutureDocumentRequirements { get; set; } = [];

    [Required]
    public string TermsUrl { get; set; } = "#";

    public string WhatsAppPhoneNumber { get; set; } = string.Empty;

    [Required]
    public string WhatsAppPrefilledMessage { get; set; } = string.Empty;

    [Required]
    public string FormSuccessMessage { get; set; } = string.Empty;

    [Required]
    public string FormSubmittingMessage { get; set; } = string.Empty;

    [Required]
    public string FormErrorMessage { get; set; } = string.Empty;
}

public sealed class SelectOption
{
    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public string Label { get; set; } = string.Empty;
}

public sealed class DocumentRequirementOption
{
    [Required]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
}

public sealed class HeroOptions
{
    [Required]
    public string Eyebrow { get; set; } = string.Empty;

    [Required]
    public string Heading { get; set; } = string.Empty;

    [Required]
    public string HighlightText { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string PrimaryButtonText { get; set; } = string.Empty;

    [Required]
    public string PrimaryButtonUrl { get; set; } = string.Empty;

    [Required]
    public string SecondaryButtonText { get; set; } = string.Empty;

    [Required]
    public string SecondaryButtonUrl { get; set; } = string.Empty;

    [Required]
    public List<StatOptions> StatStrip { get; set; } = [];
}

public sealed class HistoryOptions
{
    [Required]
    public string Eyebrow { get; set; } = string.Empty;

    [Required]
    public string SubEyebrow { get; set; } = string.Empty;

    [Required]
    public string Intro { get; set; } = string.Empty;

    [Required]
    public List<string> Paragraphs { get; set; } = [];

    [Required]
    public List<SimpleCardOptions> Timeline { get; set; } = [];
}

public class SimpleCardOptions
{
    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}

public sealed class ProductOptions : SimpleCardOptions
{
    [Required]
    public string Tag { get; set; } = string.Empty;
}

public sealed class ContactOptions
{
    [Required]
    public string Eyebrow { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string FormSuccessMessage { get; set; } = string.Empty;
}

public sealed class DistributionOptions
{
    [Required]
    public string Eyebrow { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string MapPlaceholderTitle { get; set; } = string.Empty;

    [Required]
    public string MapPlaceholderDescription { get; set; } = string.Empty;

    [Required]
    public List<SimpleCardOptions> Regions { get; set; } = [];
}

public sealed class StatOptions
{
    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public string Label { get; set; } = string.Empty;
}

public sealed class FaqOptions
{
    [Required]
    public string Question { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;
}

public sealed class TestimonialOptions
{
    [Required]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string Position { get; set; } = string.Empty;

    [Required]
    public string Quote { get; set; } = string.Empty;
}

public sealed class InsightOptions
{
    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    public string LinkText { get; set; } = string.Empty;
}

public sealed class CtaOptions
{
    [Required]
    public string Eyebrow { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string PrimaryButtonText { get; set; } = string.Empty;

    [Required]
    public string PrimaryButtonUrl { get; set; } = string.Empty;

    [Required]
    public string SecondaryButtonText { get; set; } = string.Empty;

    [Required]
    public string SecondaryButtonUrl { get; set; } = string.Empty;
}

public sealed class FooterOptions
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Copyright { get; set; } = string.Empty;

    [Required]
    public string NewsletterPlaceholder { get; set; } = string.Empty;

    [Required]
    public string NewsletterButtonText { get; set; } = string.Empty;

    [Required]
    public string NewsletterSuccessMessage { get; set; } = string.Empty;

    [Required]
    public Dictionary<string, List<LinkOptions>> Columns { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class LinkOptions
{
    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string Url { get; set; } = string.Empty;
}
