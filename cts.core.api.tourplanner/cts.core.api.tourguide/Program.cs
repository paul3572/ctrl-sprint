using System.Text;
using cts.core.svc.application;
using cts.core.svc.application.Interfaces;
using cts.core.svc.application.Services;
using cts.core.svc.infrastructure;
using cts.core.svc.infrastructure.Authentication;
using cts.core.svc.infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

JwtOptions jwtOptions = builder.Configuration
                            .GetSection(JwtOptions.SectionName)
                            .Get<JwtOptions>()
                        ?? throw new InvalidOperationException("JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
{
    throw new InvalidOperationException("JWT secret is missing.");
}

if (Encoding.UTF8.GetByteCount(jwtOptions.Secret) < 32)
{
    throw new InvalidOperationException("JWT secret must be at least 32 bytes long.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Secret)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHttpClient<IRouteService, OpenRouteService>(client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json, application/geo+json, application/gpx+xml, img/png");
    client.BaseAddress = new Uri("https://api.openrouteservice.org/");
    client.DefaultRequestHeaders.Add("Authorization",
        builder.Configuration["OpenRouteService:ApiKey"] ??
        throw new InvalidOperationException("OpenRouteService API key is missing."));
});

builder.Services.AddHttpClient<IWeatherService, OpenMeteoService>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using (var db = scope.ServiceProvider.GetRequiredService<TourPlannerDbContext>())
    {
        db.Initialize(deleteDatabase: true);
        db.Seed();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();