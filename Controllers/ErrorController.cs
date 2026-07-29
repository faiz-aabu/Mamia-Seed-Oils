using Microsoft.AspNetCore.Mvc;

namespace MamiaSeedsOil.Web.Controllers;

[Route("error")]
public sealed class ErrorController : Controller
{
    [Route("404")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NotFound");
    }

    [Route("500")]
    public IActionResult ServerError()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View("ServerError");
    }

    [Route("403")]
    public IActionResult Forbidden()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View("Forbidden");
    }

    [Route("maintenance")]
    public IActionResult Maintenance()
    {
        Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        return View("Maintenance");
    }

    [Route("{statusCode:int}")]
    public IActionResult HandleStatusCode(int statusCode)
    {
        return statusCode switch
        {
            404 => RedirectToAction(nameof(NotFoundPage)),
            403 => RedirectToAction(nameof(Forbidden)),
            _ => RedirectToAction(nameof(ServerError))
        };
    }
}
