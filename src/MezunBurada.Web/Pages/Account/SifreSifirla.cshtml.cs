using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Account;

public class SifreSifirlaModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<Models.User> _hasher;

    public SifreSifirlaModel(ApplicationDbContext db, IPasswordHasher<Models.User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool TokenValid { get; set; }
    public bool PasswordUpdated { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre gerekli.")]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalı.")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(string? email, string? token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            TokenValid = false;
            return Page();
        }

        Input.Email = email;
        Input.Token = token;
        TokenValid = await IsTokenValidAsync(email, token);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        TokenValid = await IsTokenValidAsync(Input.Email, Input.Token);
        if (!TokenValid)
        {
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
        if (user is null)
        {
            TokenValid = false;
            return Page();
        }

        user.PasswordHash = _hasher.HashPassword(user, Input.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAt = null;
        await _db.SaveChangesAsync();

        PasswordUpdated = true;
        return Page();
    }

    private async Task<bool> IsTokenValidAsync(string email, string rawToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || user.PasswordResetTokenHash is null || user.PasswordResetTokenExpiresAt is null)
        {
            return false;
        }

        if (user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        return PasswordResetTokenHelper.Matches(rawToken, user.PasswordResetTokenHash);
    }
}
