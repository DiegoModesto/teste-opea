using Infra;
using Process.Queue;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder
    .Services
    .AddReadDatabase(builder.Configuration)
    .AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
