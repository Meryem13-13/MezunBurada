using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages.Account;

public class GirisModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public GirisModel(ApplicationDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "E-posta gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta gir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gerekli.")]
        public string Password { get; set; } = string.Empty;
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
        if (user is null)
        {
            ErrorMessage = "E-posta veya şifre hatalı.";
            return Page();
        }

        var verifyResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "E-posta veya şifre hatalı.";
            return Page();
        }

        await SessionTestResultHelper.PersistAsync(_db, HttpContext.Session, user.Id);

        var identity = AuthClaimsHelper.BuildIdentity(user, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToPage("/Panel/Index");
    }
}
