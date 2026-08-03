using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MezunBurada.Web.Data;

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

app.Run();
