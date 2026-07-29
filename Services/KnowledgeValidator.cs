using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Services;

public sealed class KnowledgeValidator : IKnowledgeValidator
{
    public KnowledgeValidationResult Validate(CompanyKnowledgeModel model)
    {
        var warnings = new List<string>();

        if (string.IsNullOrWhiteSpace(model.CompanyInformation.CompanyName) || model.CompanyInformation.CompanyName == "[To Be Updated]")
        {
            warnings.Add("Company name is missing or placeholder.");
        }

        if (string.IsNullOrWhiteSpace(model.CompanyInformation.BusinessDescription) || model.CompanyInformation.BusinessDescription == "[To Be Updated]")
        {
            warnings.Add("Business description is missing or placeholder.");
        }

        if (!model.Categories.Any())
        {
            warnings.Add("Knowledge categories are empty.");
        }

        return new KnowledgeValidationResult
        {
            IsValid = warnings.Count == 0,
            Warnings = warnings
        };
    }
}
