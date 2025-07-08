using Application;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Infra;
using Serilog;
using Web.API;
using Web.API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .Host
    .UseSerilog(configureLogger: (context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder
    .Services
    .AddSwaggerGenWithAuth();

builder
    .Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();

await app.RunAsync();

namespace Web.API
{
    public partial class Program;
}
