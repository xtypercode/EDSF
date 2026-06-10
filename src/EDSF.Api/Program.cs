using EDSF.Api.Services;
using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using EDSF.Data;
using EDSF.Data.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new()
        {
            Title = "EDSF API",
            Version = "v1",
            Description = "API do sistema EDSF - Empresa de Desenvolvimento de Serviços e Facturação"
        };
        return Task.CompletedTask;
    });
});
builder.Services.AddDataLayer(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=edsf.db");
builder.Services.AddSingleton<ICsvExportService>(_ =>
    new CsvExportServiceWrapper());
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<LoginThrottle>();
builder.Services.AddScoped<SeriesManager>();

// JWT config
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EDSF";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EDSFApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Register JWT settings for DI
builder.Services.Configure<JwtSettings>(options =>
{
    options.Key = jwtKey;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbInitializer.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
