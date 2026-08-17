using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Account;

public class SifremiUnuttumModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SifremiUnuttumModel> _logger;

    public SifremiUnuttumModel(ApplicationDbContext db, ILogger<SifremiUnuttumModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Submitted { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "E-posta gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta gir.")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
        if (user is not null)
        {
            var (rawToken, hash) = PasswordResetTokenHelper.Generate();
            user.PasswordResetTokenHash = hash;
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            await _db.SaveChangesAsync();

            var resetLink = Url.Page("/Account/SifreSifirla", null,
                new { email = user.Email, token = rawToken }, Request.Scheme);

            // No email provider is wired up yet — logging is the only delivery channel right
            // now. Replace this with a real email send once one is configured.
            _logger.LogInformation("Password reset link for {Email}: {ResetLink}", user.Email, resetLink);
        }

        // Always show the same confirmation regardless of whether the email matched an
        // account, so this page can't be used to check which emails are registered.
        Submitted = true;
        return Page();
    }
}
