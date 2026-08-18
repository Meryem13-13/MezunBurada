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

public class KayitModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public KayitModel(ApplicationDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Ad Soyad gerekli.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta gir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gerekli.")]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalı.")]
        public string Password { get; set; } = string.Empty;

        public bool AcceptTerms { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Input.AcceptTerms)
        {
            ModelState.AddModelError(string.Empty, "Devam etmek için Kullanım Şartları ve Gizlilik Politikası'nı kabul etmelisin.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var emailExists = await _db.Users.AnyAsync(u => u.Email == Input.Email);
        if (emailExists)
        {
            ErrorMessage = "Bu e-posta ile zaten bir hesap var.";
            return Page();
        }

        var user = new User
        {
            FullName = Input.FullName,
            Email = Input.Email,
        };
        user.PasswordHash = _hasher.HashPassword(user, Input.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // If this browser session already has an in-progress/completed test result,
        // tie it to the new account instead of losing it — matches the "yol haritanı
        // kaybetme" promise shown on the roadmap page.
        await SessionTestResultHelper.PersistAsync(_db, HttpContext.Session, user.Id);

        await SignInAsync(user);

        return RedirectToPage("/Account/HosGeldin");
    }

    private async Task SignInAsync(User user)
    {
        var identity = AuthClaimsHelper.BuildIdentity(user, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
