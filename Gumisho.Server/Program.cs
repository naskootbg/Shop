using Backend.Contracts;
using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// 1) Configure Services
// --------------------------------------------------

// Database + Identity
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    // identity options...
})
    .AddRoles<IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppDbContext>();

// HttpClient for your sitemap service
builder.Services.AddHttpClient<SitemapStaticGeneratorService>();
//builder.Services.AddTransient<PushService>();
builder.Services.AddTransient<EmailService>();

// Controllers, Swagger, CORS, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                "https://localhost:52565",
                "http://localhost:52565",
                "https://evtinoo.com",
                "http://evtinoo.com")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// --------------------------------------------------
// 2) Ensure DB is seeded (if needed)
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

// --------------------------------------------------
// 3) HTTP → HTTPS and HSTS (very first middleware)
// --------------------------------------------------
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// --------------------------------------------------
// 4) Serve static files (wwwroot) with correct MIME for .xml
// --------------------------------------------------
app.UseDefaultFiles();

var provider = new FileExtensionContentTypeProvider();
// Override .xml to application/xml
provider.Mappings[".xml"] = "application/xml";

app.UseStaticFiles(new StaticFileOptions
{ ServeUnknownFileTypes = true, // 🚨 Required for extensionless files
    DefaultContentType = "application/octet-stream",
    ContentTypeProvider = provider
});

// --------------------------------------------------
// 5) Routing, CORS, Authentication, Swagger, Controllers
// --------------------------------------------------
app.UseRouting();
app.MapControllers();

app.UseCors();             // use default CORS policy
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// --------------------------------------------------
// 6) SPA fallback (for Vue)
// --------------------------------------------------
app.MapFallbackToFile("/index.html");

app.Run();
