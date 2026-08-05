using MamiaSeedsOil.Web.Models.AiAssistant;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAIResponseBuilder
{
    AiAnswerResult Build(AiQueryContext context, IReadOnlyList<string> suggestedQuestions);
}
