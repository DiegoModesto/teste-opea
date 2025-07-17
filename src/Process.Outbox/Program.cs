using Infra;
using Process.Outbox;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder
    .Services
    .AddDatabase(builder.Configuration)
    .AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
