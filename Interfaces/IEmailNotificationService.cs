using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IEmailNotificationService
{
    Task SendContactEnquiryNotificationAsync(ContactEnquiry enquiry, CancellationToken cancellationToken = default);
    Task SendDistributorEnquiryNotificationAsync(DistributorEnquiry enquiry, CancellationToken cancellationToken = default);
    Task SendPartnershipApplicationNotificationAsync(PartnershipApplication application, CancellationToken cancellationToken = default);
    Task SendDistributorApplicationNotificationAsync(DistributorApplication application, CancellationToken cancellationToken = default);
}
