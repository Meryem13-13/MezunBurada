using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages.Test;

public class IlgiAlaniModel : PageModel
{
    public record InterestOption(string Id, string Icon, string TitleKey, string DescriptionKey);

    public int CurrentStep { get; } = 2;
    public int TotalSteps { get; } = 3;
    public int CurrentQuestion { get; } = 2;
    public int TotalQuestions { get; } = 6;
    public string? SelectedOptionId { get; private set; }

    public IReadOnlyList<InterestOption> Options { get; } = new List<InterestOption>
    {
        new("ai-ml", "🧠", "OptionAiTitle", "OptionAiDesc"),
        new("web-backend", "💻", "OptionWebBackendTitle", "OptionWebBackendDesc"),
        new("mobile", "📱", "OptionMobileTitle", "OptionMobileDesc"),
        new("security", "🛡️", "OptionSecurityTitle", "OptionSecurityDesc"),
        new("data", "📊", "OptionDataTitle", "OptionDataDesc"),
    };

    public void OnGet()
    {
        // Placeholder selection until real answer state (session/DB) is wired up.
        SelectedOptionId = Options[0].Id;
    }
}
