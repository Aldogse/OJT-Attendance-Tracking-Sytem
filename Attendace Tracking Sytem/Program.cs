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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddIdentity<LogInCredentials,IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();    
    
builder.Services.AddScoped<IRegistrationRepository,RegistrationRepository>();
builder.Services.AddScoped<IHrRepository, HrRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

//BACKGROUND SERVICES
//builder.Services.AddHostedService<LogCheckBackgroundService>();
//builder.Services.AddHostedService<AttendanceCheckerService>();

var app = builder.Build();

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
