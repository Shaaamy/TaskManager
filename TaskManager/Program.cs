using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.APIs.Helpers;
using TaskManager.Application.Repositories;
using TaskManager.Application.Services;
using TaskManager.Core.Entities.Identity;
using TaskManager.Core.Services;
using TaskManager.Repository.Data;
using TaskManager.Repository.Identity;
using TaskManager.Repository.Repositories;
using TaskManager.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaskManagerDbContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();
builder.Services.AddAuthentication(Options =>
{
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})  //UserManager , SignInManager , RoleManager
                .AddJwtBearer(Options =>
                {
                    Options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                        RoleClaimType = "role"
                    };
                }); 

builder.Services.AddScoped<ITaskRepository,TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITokenService ,TokenService>();
builder.Services.AddAuthorization(Options =>
{
    Options.AddPolicy("DepartmentPolicy", policy => policy.RequireClaim("Department , Sales"));
});
builder.Services.AddAutoMapper(typeof(MappingProfiles));
var app = builder.Build();
#region Update Database and data seeding

using var Scope = app.Services.CreateScope();
var Services = Scope.ServiceProvider;
var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
try
{
    var dbContext = Services.GetRequiredService<TaskManagerDbContext>();
    await dbContext.Database.MigrateAsync();
    var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
    await IdentityDbContext.Database.MigrateAsync();
    var UserManager = Services.GetRequiredService<UserManager<AppUser>>();
    await AppIdentityDbContextSeed.SeedUserAsync(UserManager);
}
catch (Exception ex)
{
    var Logger = LoggerFactory.CreateLogger<Program>();
    Logger.LogError(ex, "An Error Occured While Applying The Migration");
}

#endregion
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
