using Hangfire;
using MediClaim.API.Middleware;
using MediClaim.API.Swagger;
using MediClaim.Application;
using MediClaim.Infrastructure;
using MediClaim.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "ClaimsPolicy",
        config =>
        {
            config.PermitLimit = 1000;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 100;
        });
});
//Hangfire configuration
builder.Services
    .AddHangfire(configuration => configuration
    .UseSqlServerStorage(builder.Configuration
    .GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSwaggerGen(options =>
{
    options.AddServer(new OpenApiServer
    {
        Url = "https://localhost:44312",
        Description = "Local"
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MediClaim API",
        Version = "v1",
        Description = "Enterprise Claims API",

        Contact = new OpenApiContact
        {
            Name = "MediClaim Team",
            Email = "support@mediclaim.com"
        }
    });
    //options.SwaggerDoc(
    //    "v1",
    //    new OpenApiInfo
    //    {
    //        Title = "MediClaim API",
    //        Version = "v1"
    //    });
    var xmlFile =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath =
        Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter JWT token"
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = new List<string>()
        });
});
builder.Services.AddSwaggerDocumentation();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseRateLimiter();
using (var scope =
    app.Services.CreateScope())
{
    var recurringJobManager =
        scope.ServiceProvider
            .GetRequiredService<
                IRecurringJobManager>();

    recurringJobManager
        .AddOrUpdate<
            StaleDocumentAutoRejectJob>(
                "stale-document-auto-reject-job",
                x => x.ExecuteAsync(
                    CancellationToken.None),
                "0 6 * * *",
                new RecurringJobOptions
                {
                    TimeZone =
                        TimeZoneInfo.Utc
                });

    recurringJobManager
        .AddOrUpdate<
            FraudScoreRecalculationJob>(
                "fraud-score-recalculation-job",
                x => x.ExecuteAsync(
                    CancellationToken.None),
                "0 2 * * *",
                new RecurringJobOptions
                {
                    TimeZone =
                        TimeZoneInfo.Utc
                });
    RecurringJob.AddOrUpdate<WeeklyClaimsSummaryJob>(
        "weekly-claims-summary-job",
        x => x.ExecuteAsync(
            CancellationToken.None),
        "0 7 * * 0",
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "MediClaim API v1");

        options.RoutePrefix = string.Empty;
    });
}
app.UseHttpsRedirection();
//app.UseMiddleware<CorrelationIdMiddleware>();
//app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseMiddleware<AuditRequestMiddleware>();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
//app.UseMiddleware<RateLimitingMiddleware>();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");
app.MapControllers();
app.Run();
