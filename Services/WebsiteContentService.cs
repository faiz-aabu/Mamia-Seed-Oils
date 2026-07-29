using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Resources;
using MamiaSeedsOil.Web.ViewModels;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class WebsiteContentService : IWebsiteContentService
{
    private readonly WebsiteContentOptions _options;
    private readonly ICompanyProfileService _companyProfileService;
    private readonly IProductCatalogService _productCatalogService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public WebsiteContentService(
        IOptions<WebsiteContentOptions> options,
        ICompanyProfileService companyProfileService,
        IProductCatalogService productCatalogService,
        IStringLocalizer<SharedResource> localizer)
    {
        _options = options.Value;
        _companyProfileService = companyProfileService;
        _productCatalogService = productCatalogService;
        _localizer = localizer;
    }

    public async Task<HomePageViewModel> GetHomePageContentAsync(CancellationToken cancellationToken = default)
    {
        var home = _options.HomePage;
        var company = _options.CompanyInfo;
        var companyProfile = await _companyProfileService.GetCompanyProfileAsync(cancellationToken);
        var productCatalog = await _productCatalogService.GetActiveProductsAsync(cancellationToken);

        return new HomePageViewModel
        {
            Seo = new SeoViewModel
            {
                Title = L("SeoTitle", _options.Seo.SiteTitle),
                Description = L("SeoDescription", _options.Seo.Description),
                Keywords = L("SeoKeywords", _options.Seo.Keywords),
                CanonicalUrl = _options.Seo.CanonicalUrl,
                OgImageUrl = _options.Seo.OgImageUrl,
                TwitterImageUrl = _options.Seo.TwitterImageUrl,
                SchemaLogoUrl = _options.Seo.SchemaLogoUrl
            },
            Company = new CompanyInfoViewModel
            {
                Name = company.Name,
                Phone = company.Phone,
                Email = company.Email,
                AddressLine1 = company.AddressLine1,
                AddressLine2 = company.AddressLine2,
                City = company.City,
                Region = company.Region,
                Country = company.Country,
                BusinessHours = company.BusinessHours,
                WhatsAppUrl = company.WhatsAppUrl,
                GoogleMapsEmbedUrl = company.GoogleMapsEmbedUrl,
                SocialLinks = company.SocialLinks
            },
            CompanyProfile = new CompanyProfileViewModel
            {
                CompanyName = companyProfile.CompanyName,
                EstablishedDate = companyProfile.EstablishedDate,
                AddressLines = companyProfile.AddressLines,
                BusinessDescription = companyProfile.BusinessDescription,
                AdditionalProductsDescription = companyProfile.AdditionalProductsDescription
            },
            Placeholders = _options.Placeholders.Assets.ToDictionary(
                entry => entry.Key,
                entry => new PlaceholderAssetViewModel
                {
                    Key = entry.Key,
                    Label = entry.Value.Label,
                    SubLabel = entry.Value.SubLabel,
                    AltText = entry.Value.AltText,
                    AspectRatio = entry.Value.AspectRatio
                },
                StringComparer.OrdinalIgnoreCase),
            NavigationItems =
            [
                new NavigationItemViewModel { Text = L("NavOurStory", "Our Story"), Anchor = "#history" },
                new NavigationItemViewModel { Text = L("NavProducts", "Products"), Anchor = "#products" },
                new NavigationItemViewModel { Text = L("NavProcess", "Process"), Anchor = "#process" },
                new NavigationItemViewModel { Text = L("NavGallery", "Gallery"), Anchor = "#gallery" },
                new NavigationItemViewModel { Text = L("NavContact", "Contact"), Anchor = "#contact" }
            ],
            Hero = new HeroViewModel
            {
                Eyebrow = home.Hero.Eyebrow,
                Heading = home.Hero.Heading,
                HighlightText = home.Hero.HighlightText,
                Description = home.Hero.Description,
                PrimaryButtonText = home.Hero.PrimaryButtonText,
                PrimaryButtonUrl = home.Hero.PrimaryButtonUrl,
                SecondaryButtonText = home.Hero.SecondaryButtonText,
                SecondaryButtonUrl = home.Hero.SecondaryButtonUrl,
                StatStrip = home.Hero.StatStrip.Select(MapStat).ToArray()
            },
            History = new HistoryViewModel
            {
                Eyebrow = home.History.Eyebrow,
                SubEyebrow = home.History.SubEyebrow,
                Intro = string.Format(
                    L("HistoryIntroTemplate", "{0} was incorporated on {1:dd MMMM yyyy} and has steadily built its operations around disciplined soybean processing."),
                    companyProfile.CompanyName,
                    companyProfile.EstablishedDate),
                Paragraphs =
                [
                    string.Format(
                        L("HistoryParagraphOperationsTemplate", "The company operates from {0}, where it produces premium soybean cooking oil for households, retailers, wholesalers and industrial customers."),
                        string.Join(", ", companyProfile.AddressLines)),
                    companyProfile.AdditionalProductsDescription,
                    companyProfile.BusinessDescription
                ],
                Timeline = home.History.Timeline.Select(MapSimpleCard).ToArray()
            },
            TrustBadges = home.TrustBadges.Select(MapSimpleCard).ToArray(),
            ProcessSteps = home.ProcessSteps.Select(MapSimpleCard).ToArray(),
            Products = productCatalog.Select(item => new ProductViewModel
            {
                Key = item.SeoUrl,
                Name = item.Name,
                Description = item.Description,
                Category = item.Category,
                PackagingSize = item.PackagingSize,
                Image = item.Image,
                ImageWebp = item.ImageWebp,
                IsPlaceholderImage = item.IsPlaceholderImage,
                Availability = item.Availability,
                SeoUrl = item.SeoUrl,
                DisplayOrder = item.DisplayOrder,
                Status = item.Status.ToString()
            }).ToArray(),
            DistributorCta = MapCta(home.DistributorCta),
            Gallery = new GalleryViewModel
            {
                Eyebrow = L("GalleryEyebrow", "Gallery"),
                Title = L("GalleryTitle", "Inside the factory."),
                Description = L("GalleryDescription", "Photos of the production floor, staff, and products - add real images here."),
                Items = home.GalleryItems.Select(MapSimpleCard).ToArray()
            },
            Contact = new ContactViewModel
            {
                Eyebrow = home.Contact.Eyebrow,
                Title = home.Contact.Title,
                FormSuccessMessage = L("ContactSuccessMessage", home.Contact.FormSuccessMessage)
            },
            WhyChooseUs = new WhyChooseUsViewModel
            {
                Eyebrow = L("WhyChooseEyebrow", "Why Choose Mamia"),
                Title = L("WhyChooseTitle", "Built for dependable quality and scale."),
                Description = L("WhyChooseDescription", "From sourcing to final delivery, Mamia combines local agricultural strength with disciplined manufacturing standards."),
                Items = home.WhyChooseUs.Select(MapSimpleCard).ToArray()
            },
            Industries = new IndustriesViewModel
            {
                Eyebrow = L("IndustriesEyebrow", "Industries We Serve"),
                Title = L("IndustriesTitle", "Supplying every major food and trade channel."),
                Description = L("IndustriesDescription", "Mamia supports multiple industries with pack sizes and delivery schedules tailored to operational needs."),
                Items = home.Industries.Select(MapSimpleCard).ToArray()
            },
            CompanyStats = new CompanyStatsSectionViewModel
            {
                Eyebrow = L("CompanyNumbersEyebrow", "Company Numbers"),
                Title = L("CompanyNumbersTitle", "Scale backed by measurable performance."),
                Description = L("CompanyNumbersDescription", "Our growth reflects sustained production strength, market trust, and operational discipline."),
                Items = home.CompanyStats.Select(MapStat).ToArray()
            },
            QualityAssurance = new QualityAssuranceViewModel
            {
                Eyebrow = L("QualityEyebrow", "Quality Assurance"),
                Title = L("QualityTitle", "Process control at every stage."),
                Description = L("QualityDescription", "Our quality team verifies each production stage to ensure the final oil remains clean, safe, and consistent."),
                Steps = home.QualityTimeline.Select(MapSimpleCard).ToArray()
            },
            Distribution = new DistributionViewModel
            {
                Eyebrow = home.Distribution.Eyebrow,
                Title = home.Distribution.Title,
                Description = home.Distribution.Description,
                MapPlaceholderTitle = home.Distribution.MapPlaceholderTitle,
                MapPlaceholderDescription = home.Distribution.MapPlaceholderDescription,
                Regions = home.Distribution.Regions.Select(MapSimpleCard).ToArray()
            },
            PartnershipCentre = new PartnershipCentreViewModel
            {
                Eyebrow = home.PartnershipCentre.Eyebrow,
                Title = home.PartnershipCentre.Title,
                Description = home.PartnershipCentre.Description,
                Pillars = home.PartnershipCentre.Pillars.Select(MapSimpleCard).ToArray(),
                PartnerTypes = home.PartnershipCentre.PartnerTypes,
                BusinessTypeOptions = home.PartnershipCentre.BusinessTypeOptions
                    .Select(option => new PartnershipOptionViewModel { Value = option.Value, Label = option.Label })
                    .ToArray(),
                YearsInOperationOptions = home.PartnershipCentre.YearsInOperationOptions
                    .Select(option => new PartnershipOptionViewModel { Value = option.Value, Label = option.Label })
                    .ToArray(),
                PreferredProductOptions = home.PartnershipCentre.PreferredProductOptions
                    .Select(option => new PartnershipOptionViewModel { Value = option.Value, Label = option.Label })
                    .ToArray(),
                PreferredPackagingOptions = home.PartnershipCentre.PreferredPackagingOptions
                    .Select(option => new PartnershipOptionViewModel { Value = option.Value, Label = option.Label })
                    .ToArray(),
                DocumentRequirements = home.PartnershipCentre.FutureDocumentRequirements
                    .Select(option => new PartnershipDocumentRequirementViewModel
                    {
                        DocumentType = option.DocumentType,
                        Description = option.Description,
                        IsRequired = option.IsRequired
                    })
                    .ToArray(),
                TermsUrl = home.PartnershipCentre.TermsUrl,
                WhatsAppConversationUrl = BuildWhatsAppUrl(home.PartnershipCentre.WhatsAppPhoneNumber, home.PartnershipCentre.WhatsAppPrefilledMessage),
                FormSuccessMessage = home.PartnershipCentre.FormSuccessMessage,
                FormSubmittingMessage = home.PartnershipCentre.FormSubmittingMessage,
                FormErrorMessage = home.PartnershipCentre.FormErrorMessage
            },
            Faq = new FaqSectionViewModel
            {
                Eyebrow = L("FaqEyebrow", "Frequently Asked Questions"),
                Title = L("FaqTitle", "Answers for partners and bulk buyers."),
                Items = home.Faqs.Select(x => new FaqViewModel { Question = x.Question, Answer = x.Answer }).ToArray()
            },
            Testimonials = new TestimonialSectionViewModel
            {
                Eyebrow = L("TestimonialsEyebrow", "Testimonials"),
                Title = L("TestimonialsTitle", "What partners say about working with Mamia."),
                Items = home.Testimonials.Select(x => new TestimonialViewModel
                {
                    Company = x.Company,
                    Position = x.Position,
                    Quote = x.Quote
                }).ToArray()
            },
            Insights = new InsightSectionViewModel
            {
                Eyebrow = L("InsightsEyebrow", "News and Insights"),
                Title = L("InsightsTitle", "Knowledge from our production and sourcing teams."),
                Description = L("InsightsDescription", "These articles are placeholders ready to be connected to your future blog or updates system."),
                Items = home.Insights.Select(x => new InsightViewModel
                {
                    Category = x.Category,
                    Title = x.Title,
                    Description = x.Description,
                    Url = x.Url,
                    LinkText = x.LinkText
                }).ToArray()
            },
            CorporateCta = MapCta(home.CorporateCta),
            Footer = new FooterViewModel
            {
                Description = home.Footer.Description,
                Copyright = home.Footer.Copyright,
                NewsletterPlaceholder = home.Footer.NewsletterPlaceholder,
                NewsletterButtonText = home.Footer.NewsletterButtonText,
                NewsletterSuccessMessage = home.Footer.NewsletterSuccessMessage,
                Columns = home.Footer.Columns.Select(column => new FooterColumnViewModel
                {
                    Heading = column.Key,
                    Links = column.Value.Select(link => new FooterLinkViewModel
                    {
                        Text = link.Text,
                        Url = link.Url
                    }).ToArray()
                }).ToArray()
            }
        };
    }

    private static SimpleCardViewModel MapSimpleCard(SimpleCardOptions option) => new()
    {
        Key = option.Key,
        Title = option.Title,
        Description = option.Description
    };

    private static CompanyStatsViewModel MapStat(StatOptions option) => new()
    {
        Value = option.Value,
        Label = option.Label
    };

    private static CtaViewModel MapCta(CtaOptions option) => new()
    {
        Eyebrow = option.Eyebrow,
        Title = option.Title,
        Description = option.Description,
        PrimaryButtonText = option.PrimaryButtonText,
        PrimaryButtonUrl = option.PrimaryButtonUrl,
        SecondaryButtonText = option.SecondaryButtonText,
        SecondaryButtonUrl = option.SecondaryButtonUrl
    };

    private string L(string key, string fallback)
    {
        var value = _localizer[key];
        return value.ResourceNotFound ? fallback : value.Value;
    }

    private static string BuildWhatsAppUrl(string phoneNumber, string message)
    {
        var trimmedPhone = (phoneNumber ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmedPhone))
        {
            return string.Empty;
        }

        var cleanPhone = new string(trimmedPhone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(cleanPhone))
        {
            return string.Empty;
        }

        var text = Uri.EscapeDataString(string.IsNullOrWhiteSpace(message) ? "[To Be Updated]" : message);
        return $"https://wa.me/{cleanPhone}?text={text}";
    }
}
