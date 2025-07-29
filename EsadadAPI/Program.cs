using Esadad.Core.Models;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.InterfacesJson;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using Esadad.Infrastructure.Services;
using Esadad.Infrastructure.ServicesJson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

 // Set up configuration to read from appsettings.json
 builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


builder.Services.AddControllers().AddXmlSerializerFormatters().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}); 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddDbContext<EsadadIntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EsadadDBConnectionString")));

//builder.Services.AddDbContext<EsadadContext>(options => options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings").Value));

//builder.Services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

// Configure BillerInfo to be monitored for changes
builder.Services.Configure<Biller>(builder.Configuration.GetSection("BillerInfo"));
builder.Services.AddSingleton<IOptionsMonitor<Biller>, OptionsMonitor<Biller>>();

// Configure Certificates to be monitored for changes
builder.Services.Configure<Certificates>(builder.Configuration.GetSection("Certificates"));
builder.Services.AddSingleton<IOptionsMonitor<Certificates>, OptionsMonitor<Certificates>>();

//registering services in the dependency injection
builder.Services.AddTransient<IBillPullService, BillPullService>();
builder.Services.AddTransient<IPaymentNotificationService, PaymentNotificationService>();
builder.Services.AddTransient<ICommonService, CommonService>();
builder.Services.AddTransient<IPrepaidValidationService, PrepaidValidationService>();
builder.Services.AddTransient<IPrepaidValidationJsonService, PrepaidValidationJsonService>();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
// Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthorization();
app.MapControllers();

// Retrieve IOptionsMonitor<Biller> from the DI container

var optionsMonitor = app.Services.GetRequiredService<IOptionsMonitor<Biller>>();
optionsMonitor.OnChange(biller =>
{
    MemoryCache.Biller = biller;
});

// Initialize the MemoryCache.Biller with current configuration
MemoryCache.Biller = optionsMonitor.CurrentValue;

// Retrieve IOptionsMonitor<Certificates> from the DI container
var optionsMonitorCert = app.Services
                            .GetRequiredService<IOptionsMonitor<Certificates>>();

optionsMonitorCert.OnChange(certificate =>
{
    MemoryCache.Certificates = certificate;
});

// Initialize the MemoryCache.Certificates with current configuration
MemoryCache.Certificates = optionsMonitorCert.CurrentValue;


// Fill the Currencies dictionary in MemoryCache
MemoryCache.Currencies.Add("ILS", 2);
MemoryCache.Currencies.Add("JOD", 3);  // Example: Jordanian Dinar has 3 decimal places
MemoryCache.Currencies.Add("USD", 2);
MemoryCache.Currencies.Add("EURO", 2);
 

app.Run();