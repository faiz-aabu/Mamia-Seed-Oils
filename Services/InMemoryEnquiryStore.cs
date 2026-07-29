using System.Collections.Concurrent;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Services;

public sealed class InMemoryEnquiryStore : IEnquiryStore
{
    private readonly ConcurrentQueue<ContactEnquiry> _contactEnquiries = new();
    private readonly ConcurrentQueue<DistributorEnquiry> _distributorEnquiries = new();
    private readonly ConcurrentQueue<PartnershipApplication> _partnershipApplications = new();

    public Task StoreContactEnquiryAsync(ContactEnquiry enquiry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _contactEnquiries.Enqueue(enquiry);
        return Task.CompletedTask;
    }

    public Task StoreDistributorEnquiryAsync(DistributorEnquiry enquiry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _distributorEnquiries.Enqueue(enquiry);
        return Task.CompletedTask;
    }

    public Task StorePartnershipApplicationAsync(PartnershipApplication application, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _partnershipApplications.Enqueue(application);
        return Task.CompletedTask;
    }
}
