using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IKnowledgeValidator
{
    KnowledgeValidationResult Validate(CompanyKnowledgeModel model);
}
