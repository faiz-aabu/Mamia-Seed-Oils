using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.ViewModels;

public sealed class HomePageViewModel
{
    public SeoViewModel Seo { get; init; } = new();
    public CompanyInfoViewModel Company { get; init; } = new();
    public CompanyProfileViewModel CompanyProfile { get; init; } = new();
    public HeroViewModel Hero { get; init; } = new();
    public HistoryViewModel History { get; init; } = new();
    public IReadOnlyList<SimpleCardViewModel> TrustBadges { get; init; } = [];
    public IReadOnlyList<SimpleCardViewModel> ProcessSteps { get; init; } = [];
    public IReadOnlyList<ProductViewModel> Products { get; init; } = [];
    public CtaViewModel DistributorCta { get; init; } = new();
    public GalleryViewModel Gallery { get; init; } = new();
    public ContactViewModel Contact { get; init; } = new();
    public WhyChooseUsViewModel WhyChooseUs { get; init; } = new();
    public IndustriesViewModel Industries { get; init; } = new();
    public CompanyStatsSectionViewModel CompanyStats { get; init; } = new();
    public QualityAssuranceViewModel QualityAssurance { get; init; } = new();
    public DistributionViewModel Distribution { get; init; } = new();
    public PartnershipCentreViewModel PartnershipCentre { get; init; } = new();
    public FaqSectionViewModel Faq { get; init; } = new();
    public TestimonialSectionViewModel Testimonials { get; init; } = new();
    public InsightSectionViewModel Insights { get; init; } = new();
    public CtaViewModel CorporateCta { get; init; } = new();
    public FooterViewModel Footer { get; init; } = new();
    public IReadOnlyList<NavigationItemViewModel> NavigationItems { get; init; } = [];
    public IReadOnlyDictionary<string, PlaceholderAssetViewModel> Placeholders { get; init; } = new Dictionary<string, PlaceholderAssetViewModel>();
}

public sealed class PartnershipCentreViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<SimpleCardViewModel> Pillars { get; init; } = [];
    public IReadOnlyList<string> PartnerTypes { get; init; } = [];
    public IReadOnlyList<PartnershipOptionViewModel> BusinessTypeOptions { get; init; } = [];
    public IReadOnlyList<PartnershipOptionViewModel> YearsInOperationOptions { get; init; } = [];
    public IReadOnlyList<PartnershipOptionViewModel> PreferredProductOptions { get; init; } = [];
    public IReadOnlyList<PartnershipOptionViewModel> PreferredPackagingOptions { get; init; } = [];
    public IReadOnlyList<PartnershipDocumentRequirementViewModel> DocumentRequirements { get; init; } = [];
    public string TermsUrl { get; init; } = "#";
    public string WhatsAppConversationUrl { get; init; } = string.Empty;
    public string FormSuccessMessage { get; init; } = string.Empty;
    public string FormSubmittingMessage { get; init; } = string.Empty;
    public string FormErrorMessage { get; init; } = string.Empty;
}

public sealed class PartnershipOptionViewModel
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
}

public sealed class PartnershipDocumentRequirementViewModel
{
    public string DocumentType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
}

public sealed class PlaceholderAssetViewModel
{
    public string Key { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string SubLabel { get; init; } = string.Empty;
    public string AltText { get; init; } = string.Empty;
    public string AspectRatio { get; init; } = "4 / 3";
}

public sealed class ResponsiveImageViewModel
{
    public string? Src { get; init; }
    public string? WebpSrc { get; init; }
    public string Alt { get; init; } = string.Empty;
    public int Width { get; init; } = 1200;
    public int Height { get; init; } = 900;
    public string Sizes { get; init; } = "(max-width: 900px) 100vw, 33vw";
    public string CssClass { get; init; } = string.Empty;
    public string PlaceholderKey { get; init; } = string.Empty;
    public bool UsePlaceholder { get; init; } = true;
}

public sealed class SeoViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Keywords { get; init; } = string.Empty;
    public string CanonicalUrl { get; init; } = string.Empty;
    public string OgImageUrl { get; init; } = string.Empty;
    public string TwitterImageUrl { get; init; } = string.Empty;
    public string SchemaLogoUrl { get; init; } = string.Empty;
}

public sealed class CompanyInfoViewModel
{
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string BusinessHours { get; init; } = string.Empty;
    public string WhatsAppUrl { get; init; } = string.Empty;
    public string GoogleMapsEmbedUrl { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> SocialLinks { get; init; } = new Dictionary<string, string>();
}

public sealed class CompanyProfileViewModel
{
    public string CompanyName { get; init; } = string.Empty;
    public DateOnly EstablishedDate { get; init; }
    public IReadOnlyList<string> AddressLines { get; init; } = [];
    public string BusinessDescription { get; init; } = string.Empty;
    public string AdditionalProductsDescription { get; init; } = string.Empty;
}

public sealed class NavigationItemViewModel
{
    public string Text { get; init; } = string.Empty;
    public string Anchor { get; init; } = string.Empty;
}

public sealed class HeroViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Heading { get; init; } = string.Empty;
    public string HighlightText { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string PrimaryButtonText { get; init; } = string.Empty;
    public string PrimaryButtonUrl { get; init; } = string.Empty;
    public string SecondaryButtonText { get; init; } = string.Empty;
    public string SecondaryButtonUrl { get; init; } = string.Empty;
    public IReadOnlyList<CompanyStatsViewModel> StatStrip { get; init; } = [];
}

public sealed class HistoryViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string SubEyebrow { get; init; } = string.Empty;
    public string Intro { get; init; } = string.Empty;
    public IReadOnlyList<string> Paragraphs { get; init; } = [];
    public IReadOnlyList<SimpleCardViewModel> Timeline { get; init; } = [];
}

public sealed class SimpleCardViewModel
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public sealed class ProductViewModel
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string PackagingSize { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public string? ImageWebp { get; init; }
    public bool IsPlaceholderImage { get; init; } = true;
    public string Availability { get; init; } = string.Empty;
    public string SeoUrl { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed class GalleryViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<SimpleCardViewModel> Items { get; init; } = [];
}

public sealed class ContactViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string FormSuccessMessage { get; init; } = string.Empty;

    [Required, StringLength(120), NoHtml]
    public string FullName { get; init; } = string.Empty;

    [StringLength(120), NoHtml]
    public string? Company { get; init; }

    [Required, EmailAddress, StringLength(160), NoHtml]
    public string EmailAddress { get; init; } = string.Empty;

    [Phone, StringLength(60), NoHtml]
    public string? PhoneNumber { get; init; }

    [StringLength(1500), NoHtml]
    public string? Requirements { get; init; }
}

public sealed class CompanyStatsViewModel
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
}

public sealed class CompanyStatsSectionViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<CompanyStatsViewModel> Items { get; init; } = [];
}

public sealed class WhyChooseUsViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<SimpleCardViewModel> Items { get; init; } = [];
}

public sealed class IndustriesViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<SimpleCardViewModel> Items { get; init; } = [];
}

public sealed class QualityAssuranceViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<SimpleCardViewModel> Steps { get; init; } = [];
}

public sealed class DistributionViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string MapPlaceholderTitle { get; init; } = string.Empty;
    public string MapPlaceholderDescription { get; init; } = string.Empty;
    public IReadOnlyList<SimpleCardViewModel> Regions { get; init; } = [];
}

public sealed class FaqViewModel
{
    public string Question { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
}

public sealed class FaqSectionViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<FaqViewModel> Items { get; init; } = [];
}

public sealed class TestimonialViewModel
{
    public string Company { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string Quote { get; init; } = string.Empty;
}

public sealed class TestimonialSectionViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<TestimonialViewModel> Items { get; init; } = [];
}

public sealed class InsightViewModel
{
    public string Category { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string LinkText { get; init; } = string.Empty;
}

public sealed class InsightSectionViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<InsightViewModel> Items { get; init; } = [];
}

public sealed class CtaViewModel
{
    public string Eyebrow { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string PrimaryButtonText { get; init; } = string.Empty;
    public string PrimaryButtonUrl { get; init; } = string.Empty;
    public string SecondaryButtonText { get; init; } = string.Empty;
    public string SecondaryButtonUrl { get; init; } = string.Empty;
}

public sealed class FooterColumnViewModel
{
    public string Heading { get; init; } = string.Empty;
    public IReadOnlyList<FooterLinkViewModel> Links { get; init; } = [];
}

public sealed class FooterLinkViewModel
{
    public string Text { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}

public sealed class FooterViewModel
{
    public string Description { get; init; } = string.Empty;
    public string Copyright { get; init; } = string.Empty;
    public string NewsletterPlaceholder { get; init; } = string.Empty;
    public string NewsletterButtonText { get; init; } = string.Empty;
    public string NewsletterSuccessMessage { get; init; } = string.Empty;
    public IReadOnlyList<FooterColumnViewModel> Columns { get; init; } = [];
}
