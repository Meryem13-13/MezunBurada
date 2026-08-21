using System.ComponentModel.DataAnnotations;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages;

public class KurumlarIcinModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public KurumlarIcinModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Submitted { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Kurum adı gerekli.")]
        public string OrganizationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adın gerekli.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta gir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesajını yazman gerekli.")]
        [MinLength(10, ErrorMessage = "Mesajın biraz daha detaylı olsun (en az 10 karakter).")]
        public string Message { get; set; } = string.Empty;
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

        _db.ContactMessages.Add(new ContactMessage
        {
            Name = Input.Name,
            Email = Input.Email,
            OrganizationName = Input.OrganizationName,
            Message = Input.Message,
            Category = ContactCategory.Institutional,
        });
        await _db.SaveChangesAsync();

        Submitted = true;
        return Page();
    }
}
