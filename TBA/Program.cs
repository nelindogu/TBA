using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using TBA.Data; // Replace with your namespace for AppDbContext
using TBA.Models; // Replace with your namespace for User model

using System.Text;

// ✅ Ensure Global UTF-8 Encoding
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure SQLite Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// ✅ Configure Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? "";
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? "";
    options.CallbackPath = "/signin-google";
});



builder.Services.AddAuthorization();

var app = builder.Build();

// ✅ Ensure HTTP Response Encoding is UTF-8
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Type", "text/html; charset=utf-8");
    await next();
});

// ✅ Apply Database Migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// ✅ Middleware
app.UseAuthentication();
app.UseAuthorization();

// ✅ Define Routes

// 🔑 Login Route
app.MapGet("/login", () => Results.Challenge(
    new AuthenticationProperties { RedirectUri = "/" },
    new[] { "Google" }));

// 🚪 Logout Route
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("Cookies");
    return Results.Redirect("/");
});

// 📝 Home Route
app.MapGet("/", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var email = context.User.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = context.User.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        return Results.Content($@"
            <h1>Welcome, {name}!</h1>
            <p>Email: {email}</p>
            <p><a href='/profile'>Profile</a></p>
            <p><a href='/logout'>Log out</a></p>",
            "text/html; charset=utf-8");
    }

    return Results.Content($@"
        <h1>Welcome!</h1>
        <p><a href='/login'>Login with Google</a></p>",
        "text/html; charset=utf-8");
});

// 📝 Profile Route
app.MapGet("/profile", async (AppDbContext db, HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var email = context.User.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = context.User.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        if (!string.IsNullOrEmpty(email))
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                db.Users.Add(new User
                {
                    Name = name ?? "Unknown",
                    Email = email
                });

                await db.SaveChangesAsync();
            }

            return Results.Content($@"
                <h1>Profile</h1>
                <p>Ad: {name}</p>
                <p>Email: {email}</p>
                <p><a href='/users'> See all Users</a></p>
                <p><a href='/logout'>Çıkış Yap</a></p>",
                "text/html; charset=utf-8");
        }
    }

    return Results.Unauthorized();
});

// 📝 Users Route
app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    var userList = string.Join("<br>", users.Select(u => $"{u.Name} ({u.Email})"));

    return Results.Content($@"
        <h1> All Users:</h1>
        <p>{userList}</p>
        <p><a href='/'>Back to MainPage</a></p>",
        "text/html; charset=utf-8");
});

// 🏁 Start the Application
app.Run();
