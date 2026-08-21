using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MezunBurada.Web.Data;
using MezunBurada.Web.Models;

var builder = WebApplication.CreateBuilder(args);

var supportedCultures = new[]
{
    new CultureInfo("tr"),
    new CultureInfo("en"),
    new CultureInfo("ar"),
};

// Add services to the container.
builder.Services.AddRazorPages()
    .AddViewLocalization();

// Every page under /Admin requires the AdminOnly policy (see below) — added here in one place
// rather than an [Authorize] attribute on each of the ~15 admin page folders.
builder.Services.Configure<Microsoft.AspNetCore.Mvc.RazorPages.RazorPagesOptions>(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("tr");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    // Cookie-based selection: URL stays the same for every language,
    // the chosen culture is remembered in a cookie (see Pages/SetLanguage).
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Anonymous test-flow progress (selected department, answers-so-far, computed result) lives in
// session until real auth exists — see Pages/Test/*.cshtml.cs and Pages/Roadmap/Index.cshtml.cs.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.IsEssential = true;
});

// Cookie auth (not JWT) — this is a server-rendered Razor Pages app, not an API consumed by a
// separate client, so a login cookie is the simpler, idiomatic fit.
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/giris";
        options.AccessDeniedPath = "/giris";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "true"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Sets the language cookie and sends the user back where they came from.
// Used by the language switcher in _Layout.cshtml.
app.MapGet("/set-language", (string culture, string returnUrl, HttpContext context) =>
{
    context.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true });

    return Results.LocalRedirect(returnUrl);
});

// Signs the user out and sends them back to the home page. Used by the "Çıkış Yap" link in
// _PanelLayout.cshtml (a POST form, not a plain link, since this changes server-side state).
app.MapPost("/cikis-yap", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/");
});

// robots.txt and sitemap.xml build their absolute URLs from the current request's host rather
// than a hardcoded domain — the real production domain isn't chosen yet, so this way neither
// file needs to be remembered and edited once it is.
app.MapGet("/robots.txt", (HttpContext context) =>
{
    var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
    var content = $"""
        User-agent: *
        Allow: /
        Disallow: /admin/
        Disallow: /panel
        Disallow: /profil
        Disallow: /kayit
        Disallow: /giris
        Disallow: /sifremi-unuttum
        Disallow: /sifre-sifirla

        Sitemap: {baseUrl}/sitemap.xml
        """;
    return Results.Text(content, "text/plain", Encoding.UTF8);
});

app.MapGet("/sitemap.xml", async (HttpContext context, ApplicationDbContext db) =>
{
    var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

    var staticPaths = new[]
    {
        "/", "/test/bolum", "/deneyimler", "/deneyim-paylas", "/hakkimizda", "/sss",
        "/gizlilik", "/kullanim-sartlari", "/iletisim", "/kurumlar-icin",
    };

    var departmentSlugs = await db.Departments
        .Where(d => d.IsActive)
        .Select(d => d.Slug)
        .ToListAsync();

    var sb = new StringBuilder();
    sb.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
    sb.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

    foreach (var path in staticPaths)
    {
        sb.AppendLine($"  <url><loc>{baseUrl}{path}</loc></url>");
    }

    foreach (var slug in departmentSlugs)
    {
        sb.AppendLine($"  <url><loc>{baseUrl}/bolum/{slug}</loc></url>");
    }

    sb.AppendLine("</urlset>");

    return Results.Text(sb.ToString(), "application/xml", Encoding.UTF8);
});

app.Run();
