using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

void PrintStatus() {
    var notifierEnabled = builder.Services.Any(s => s.ServiceType == typeof(ISystemdNotifier));
    Console.WriteLine($"{nameof(ISystemdNotifier)} is {(notifierEnabled ? "" : "not ")}enabled");
    var lifetimeEnabled = builder.Services.Any(s => s.ImplementationType == typeof(SystemdLifetime));
    Console.WriteLine($"{nameof(SystemdLifetime)} is {(lifetimeEnabled ? "" : "not ")}enabled");
}

PrintStatus();

Console.WriteLine($"Calling nameof(SystemHostBuilderExtensions.UseSystemd)");
builder.Host.UseSystemd();

PrintStatus();

builder.WebHost.ConfigureKestrel(ko => {
    ko.UseSystemd();
});

var app = builder.Build();

app.MapGet("/", () => "Yo");

await app.RunAsync();