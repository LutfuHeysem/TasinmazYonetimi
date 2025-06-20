using Api.Data;
using Api.Interfaces;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IIlService, IlService>();
builder.Services.AddScoped<IIlceService, IlceService>();
builder.Services.AddScoped<IMahalleService, MahalleService>();
builder.Services.AddScoped<ITasinmazService, TasinmazService>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/api/Kullanici/Login";
        options.LogoutPath = "/api/Kullanici/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:4200", "http://localhost:3000", "https://web.postman.co", "chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
});

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