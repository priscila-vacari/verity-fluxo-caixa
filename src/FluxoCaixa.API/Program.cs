using FluxoCaixa.API.Middlewares;
using System.Reflection;
using FluxoCaixa.Application;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;
using Asp.Versioning;
using Serilog;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using FluentValidation;
using FluxoCaixa.API.Filters;
using FluxoCaixa.Infra;
using FluxoCaixa.API.Swagger;
using FluxoCaixa.Infra.Context;
using FluxoCaixa.API.Validators;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

#region Log
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});
#endregion

try
{
    #region Services DI
    builder.Services.AddHttpContextAccessor();
    ApplicationDependencyRegister.RegisterServices(builder.Services);
    InfraDependencyRegister.RegisterServices(builder.Services, builder.Configuration);
    #endregion

    #region Swagger Configs
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.ResolveConflictingActions(opt => opt.First());
        options.DescribeAllParametersInCamelCase();
        options.CustomSchemaIds(opt => opt.ToString());

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    #endregion

    #region AutoMapper
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    #endregion

    #region HealthCheck
    builder.Services.AddHealthChecks();
    #endregion

    #region Filters
    builder.Services.AddControllers(opt => opt.Filters.Add(typeof(ExceptionFilter)));
    #endregion

    #region Validators
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<LaunchRequestModelValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<ConsolidationRequestModelValidator>();

    #endregion

    #region RateLimit
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    #endregion

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.UseHealthChecks("/health",
        new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
        {
            ResponseWriter = async (context, report) =>
            {
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    healthChecks = report.Entries.Select(entry => new
                    {
                        check = entry.Key,
                        ErrorMessage = entry.Value.Exception?.Message,
                        status = Enum.GetName(typeof(HealthStatus), entry.Value.Status),
                        description = entry.Value.Description
                    })
                });
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            }
        });

    app.UseCorrelationId();
    app.UseBruteForceProtection();
    app.UseSerilogRequestLogging();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal error when starting app.");
}
finally
{
    Log.CloseAndFlush();
}