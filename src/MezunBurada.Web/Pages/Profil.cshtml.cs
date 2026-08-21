using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MezunBurada.Web.Pages;

[Authorize]
public class ProfilModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public ProfilModel(ApplicationDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public AccountInputModel AccountInput { get; set; } = new();

    [BindProperty]
    public PasswordInputModel PasswordInput { get; set; } = new();

    public string Email { get; private set; } = string.Empty;
    public DateTime MemberSince { get; private set; }
    public List<TestResult> TestHistory { get; private set; } = new();

    public string? AccountSuccessMessage { get; set; }
    public string? PasswordSuccessMessage { get; set; }
    public string? PasswordErrorMessage { get; set; }

    public class AccountInputModel
    {
        [Required(ErrorMessage = "Ad Soyad gerekli.")]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;
    }

    public class PasswordInputModel
    {
        public string? CurrentPassword { get; set; }

        [MinLength(8, ErrorMessage = "Yeni şifre en az 8 karakter olmalı.")]
        public string? NewPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        AccountInput.FullName = user.FullName;
        await LoadDisplayDataAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateAccountAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        if (!TryValidateModel(AccountInput, nameof(AccountInput)))
        {
            await LoadDisplayDataAsync(user);
            return Page();
        }

        user.FullName = AccountInput.FullName;
        await _db.SaveChangesAsync();

        AccountSuccessMessage = "Bilgilerin güncellendi.";
        await LoadDisplayDataAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        AccountInput.FullName = user.FullName;

        if (string.IsNullOrWhiteSpace(PasswordInput.CurrentPassword) || string.IsNullOrWhiteSpace(PasswordInput.NewPassword))
        {
            PasswordErrorMessage = "Mevcut ve yeni şifreni gir.";
            await LoadDisplayDataAsync(user);
            return Page();
        }

        if (PasswordInput.NewPassword.Length < 8)
        {
            PasswordErrorMessage = "Yeni şifre en az 8 karakter olmalı.";
            await LoadDisplayDataAsync(user);
            return Page();
        }

        var verifyResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, PasswordInput.CurrentPassword);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            PasswordErrorMessage = "Mevcut şifre yanlış.";
            await LoadDisplayDataAsync(user);
            return Page();
        }

        user.PasswordHash = _hasher.HashPassword(user, PasswordInput.NewPassword);
        await _db.SaveChangesAsync();

        PasswordSuccessMessage = "Şifren güncellendi.";
        await LoadDisplayDataAsync(user);
        return Page();
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _db.Users.FindAsync(userId);
    }

    private async Task LoadDisplayDataAsync(User user)
    {
        Email = user.Email;
        MemberSince = user.CreatedAt;
        TestHistory = await _db.TestResults
            .Include(t => t.SubField).ThenInclude(sf => sf!.Department)
            .Where(t => t.UserId == user.Id)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync();
    }
}
