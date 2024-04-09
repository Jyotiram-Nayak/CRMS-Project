using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO.Email;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Core.Services;
using CRMS_Project.Infrastructure.DbContext;
using CRMS_Project.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option => option.SignIn.RequireConfirmedEmail = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<IEmailServices, EmailServices>();

//configure services with SMTPConfiguration
builder.Services.Configure<SMTPConfiguration>(builder.Configuration.GetSection("EmailSettings"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    try
    {
        var servicesProvider = scope.ServiceProvider;
        var userManager = servicesProvider.GetRequiredService<UserManager<ApplicationUser>>();
        AppDbInitializer.InitializerAsync(servicesProvider, userManager).Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred during database initialization: " + ex.Message);
        throw;
    }
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
