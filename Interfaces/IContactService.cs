using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IContactService
{
    Task<ContactServiceResult> SubmitContactEnquiryAsync(ContactEnquiry enquiry, CancellationToken cancellationToken = default);
    Task<ContactServiceResult> SubmitDistributorEnquiryAsync(DistributorEnquiry enquiry, CancellationToken cancellationToken = default);
}

public sealed class ContactServiceResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? EnquiryId { get; init; }
}
