namespace MamiaSeedsOil.Web.Models.Api;

public sealed class ApiErrorResponse
{
    public string Message { get; set; } = "An unexpected error occurred.";
    public int StatusCode { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string? Detail { get; set; }
}
