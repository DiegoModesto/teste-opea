using Infra;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder
    .Services
    .AddInfrastructure(builder.Configuration)
    .AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
