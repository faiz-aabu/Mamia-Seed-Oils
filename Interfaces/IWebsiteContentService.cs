using MamiaSeedsOil.Web.ViewModels;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IWebsiteContentService
{
    Task<HomePageViewModel> GetHomePageContentAsync(CancellationToken cancellationToken = default);
}
