using MamiaSeedsOil.Web.DTOs.Contact;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MamiaSeedsOil.Web.Controllers;

[ApiController]
[EnableRateLimiting("ContactPolicy")]
[Route("api/contact")]
public sealed class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost("enquiry")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<ContactResponseDto>> SubmitEnquiry([FromBody] ContactEnquiryRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var model = new ContactEnquiry
        {
            FullName = request.FullName,
            CompanyName = request.CompanyName,
            Email = request.Email,
            Phone = request.Phone,
            Message = request.Message
        };

        var result = await _contactService.SubmitContactEnquiryAsync(model, cancellationToken);
        var response = new ContactResponseDto
        {
            Success = result.Success,
            Message = result.Message,
            EnquiryId = result.EnquiryId
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("distributor-enquiry")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<ContactResponseDto>> SubmitDistributorEnquiry([FromBody] DistributorEnquiryRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var model = new DistributorEnquiry
        {
            CompanyName = request.CompanyName,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Country = request.Country,
            State = request.State,
            BusinessType = request.BusinessType,
            ExpectedMonthlyVolume = request.ExpectedMonthlyVolume,
            Message = request.Message
        };

        var result = await _contactService.SubmitDistributorEnquiryAsync(model, cancellationToken);
        var response = new ContactResponseDto
        {
            Success = result.Success,
            Message = result.Message,
            EnquiryId = result.EnquiryId
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }
}
