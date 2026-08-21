using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Admin.Messages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IList<ContactMessage> UnhandledMessages { get; private set; } = new List<ContactMessage>();
    public IList<ContactMessage> HandledMessages { get; private set; } = new List<ContactMessage>();

    public async Task OnGetAsync()
    {
        var messages = await _db.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        UnhandledMessages = messages.Where(m => !m.IsHandled).ToList();
        HandledMessages = messages.Where(m => m.IsHandled).ToList();
    }

    public async Task<IActionResult> OnPostToggleHandledAsync(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is not null)
        {
            message.IsHandled = !message.IsHandled;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is not null)
        {
            _db.ContactMessages.Remove(message);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
