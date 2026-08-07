using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IEnquiryStore
{
    Task StoreContactEnquiryAsync(ContactEnquiry enquiry, CancellationToken cancellationToken = default);
    Task StoreDistributorEnquiryAsync(DistributorEnquiry enquiry, CancellationToken cancellationToken = default);
    Task StorePartnershipApplicationAsync(PartnershipApplication application, CancellationToken cancellationToken = default);
    Task StoreDistributorApplicationAsync(DistributorApplication application, CancellationToken cancellationToken = default);
}
