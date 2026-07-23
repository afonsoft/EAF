using Eaf.Gateways.API;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

const string serviceName = "Eaf.Gateways.API";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "Routes", $"ocelot.{builder.Environment.EnvironmentName}.json"))
    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "Routes", $"ocelot.SwaggerEndPoints.{builder.Environment.EnvironmentName}.json"))
    .AddOcelotWithSwaggerSupport(builder.Environment, "Routes")
    .AddEnvironmentVariables();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHsts(o => o.IncludeSubDomains = true);

//Trace OpenTelemetry
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));

// Configure important OpenTelemetry settings, the console exporter, and instrumentation library
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
    .AddService(serviceName: serviceName))
    .WithTracing(tracing =>
    {
        tracing.AddSource("Eaf.Gateways.API")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: "1.0.0"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(serviceName);
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
        .AddMeter(serviceName)
        // Metrics provides by ASP.NET Core in .NET 8
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel");
    });

// Swagger for ocelot
builder.Services.AddSwaggerGen();
// Add Ocelot with plugin
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly()
    .AddCacheManager(x => x.WithDictionaryHandle())
    .AddConfigPlaceholders();

builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddHostedService<ConfigurationNotifyingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("EafGateWayCorsPolicy", policyBuilder =>
    {
        var corsOrigins = builder.Configuration["App:CorsOrigins"];
        var isDevelopment = builder.Environment.IsDevelopment();

        if (!isDevelopment && (string.IsNullOrWhiteSpace(corsOrigins) || corsOrigins == "*"))
        {
            throw new InvalidOperationException("App:CorsOrigins must be configured with explicit origins in production.");
        }

        if (isDevelopment && corsOrigins == "*")
        {
            policyBuilder.SetIsOriginAllowed((host) => true);
        }
        else
        {
            policyBuilder.WithOrigins(
                    corsOrigins
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.TrimEnd('/'))
                        .ToArray()
                )
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        }

        policyBuilder.AllowAnyMethod()
            .AllowCredentials()
            .WithHeaders("Authorization", "Content-Type", "X-Requested-With", "Accept", "X-XSRF-TOKEN");
    });
});

var app = builder.Build();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerForOcelotUI();
}
app.UseCors("EafGateWayCorsPolicy"); //Enable CORS!
app.MapControllers();
app.UseWebSockets();
app.UseOcelot().Wait();
await app.RunAsync();