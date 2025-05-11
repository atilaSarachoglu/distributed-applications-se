using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();   // <– in-memory store
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // tweak as you like
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("FrontEnd", p => p
        .WithOrigins("https://localhost:64705")   // ← портът на Razor Pages
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());                    // 🔑
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"] ?? "CodeSpaceApi",
            ValidAudience = builder.Configuration["JwtAudience"] ?? "CodeSpaceWeb",
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"] ??
                                           "dev-sample-key-change-me"))
        };

        /* NEW – look in the “jwt” cookie if no Authorization header */
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var cookie = ctx.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(cookie))
                    ctx.Token = cookie;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        p => p.RequireClaim("isAdmin", "true"));   // ← this was missing
});

/* secure every Razor Page except Login / Register */
builder.Services.AddRazorPages(o =>
{
    o.Conventions.AuthorizeFolder("/");      // everything
    o.Conventions.AllowAnonymousToPage("/Users/Login");
    o.Conventions.AllowAnonymousToPage("/Users/Register");
    o.Conventions.AllowAnonymousToPage("/Contact/Index");
    o.Conventions.AllowAnonymousToPage("/About/Index");
    o.Conventions.AllowAnonymousToPage("/Index");
});

builder.Logging.AddConsole();

var app = builder.Build();

Console.WriteLine($"JwtKey seen by {builder.Environment.ApplicationName} = "
                  + builder.Configuration["JwtKey"]);

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseCors("FrontEnd");
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
app.Run();
