using System.Text.Json;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Data;

// Ties an anonymous test-flow session (see Pages/Test/*.cshtml.cs) to a real account —
// used on both register and login, so a returning user's in-session result isn't lost either.
public static class SessionTestResultHelper
{
    public static async Task PersistAsync(ApplicationDbContext db, ISession session, int userId)
    {
        var subFieldId = session.GetInt32("ResultSubFieldId");
        if (subFieldId is null)
        {
            return;
        }

        var careerPath = await db.CareerPaths.FirstOrDefaultAsync(cp => cp.SubFieldId == subFieldId);
        var levelRaw = session.GetInt32("ResultLevel");
        var level = levelRaw.HasValue ? (ProficiencyLevel)levelRaw.Value : ProficiencyLevel.Beginner;

        var tallyJson = session.GetString("InterestTally");
        var matchPercent = 87;
        if (!string.IsNullOrEmpty(tallyJson))
        {
            var tally = JsonSerializer.Deserialize<Dictionary<int, int>>(tallyJson) ?? new Dictionary<int, int>();
            var total = tally.Values.Sum();
            if (total > 0)
            {
                matchPercent = (int)Math.Round((double)tally.GetValueOrDefault(subFieldId.Value) / total * 100);
            }
        }

        db.TestResults.Add(new TestResult
        {
            UserId = userId,
            SubFieldId = subFieldId.Value,
            CareerPathId = careerPath?.Id,
            Level = level,
            CareerMatchPercent = matchPercent,
        });
        await db.SaveChangesAsync();
    }
}
