using Attendace_Tracking_Sytem.ApiSettings;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Repository;
using Attendace_Tracking_Sytem.Seeders;
using Attendace_Tracking_Sytem.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//JWT CONFIGURATIONS
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuers"],
            ValidAudience = builder.Configuration["Jwt:Audiences"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Token"]))
        };
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddIdentity<LogInCredentials,IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton(typeof(IConverter),
    new SynchronizedConverter(new PdfTools()));
    
builder.Services.AddScoped<IRegistrationRepository,RegistrationRepository>();
builder.Services.AddScoped<IHrRepository, HrRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<EmailServices>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<IViewRenderService,ViewRenderService>();
builder.Services.AddScoped<IJwtService,JwtService>();


//SET UP EMAIL SERRVICE
builder.Services.Configure<BrevoSettings>(
  builder.Configuration.GetSection("BrevoSettings")    
);

//BACKGROUND SERVICES
//builder.Services.AddHostedService<LogCheckBackgroundService>();
//builder.Services.AddHostedService<AttendanceCheckerService>();
//builder.Services.AddHostedService<AttendanceStatusCheckerService>();

var app = builder.Build();


//ROLE SEEDER
using (var scope = app.Services.CreateScope())
{
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.seedRolesAsync(roles);
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseRouting();

app.UseExceptionHandler(error =>
{
    error.Run(async (context) =>
    {
        var exceptionFeatures = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeatures?.Error;

        Console.WriteLine($"Error: {exception?.Message}");

        context.Response.StatusCode = 500;
        context.Response.Redirect("/Home/Status500");
    });
});
app.UseAuthorization();

app.MapStaticAssets();

app.UseCors(cors =>
{
    cors.AllowAnyOrigin();
    cors.AllowAnyMethod();
    cors.AllowAnyHeader();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
