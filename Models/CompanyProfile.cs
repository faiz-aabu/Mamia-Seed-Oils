namespace MamiaSeedsOil.Web.Models;

public sealed class CompanyProfile
{
    public string CompanyName { get; set; } = string.Empty;
    public DateOnly EstablishedDate { get; set; }
    public IReadOnlyList<string> AddressLines { get; set; } = [];
    public string BusinessDescription { get; set; } = string.Empty;
    public string AdditionalProductsDescription { get; set; } = string.Empty;
}
