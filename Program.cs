using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

// This doesn't work inside a container, since SystemdHelpers.IsSystemdService()
// works by checking if the parent process is systemd, but we have a container
// runtime in the middle.
// builder.Host.UseSystemd();

// See https://source.dot.net/#Microsoft.Extensions.Hosting.Systemd/SystemdHostBuilderExtensions.cs
// This still isn't working, needs more debugging
builder.WebHost.ConfigureServices((hostContext, services) =>
{
    services.Configure<ConsoleLoggerOptions>(options =>
    {
        options.FormatterName = ConsoleFormatterNames.Systemd;
    });

    // This resolves to /run/notify/notify.sock
    Console.WriteLine($"NOTIFY_SOCKET: {Environment.GetEnvironmentVariable("NOTIFY_SOCKET")}");

    // See https://source.dot.net/#Microsoft.Extensions.Hosting.Systemd/SystemdNotifier.cs
    services.AddSingleton<ISystemdNotifier, SystemdNotifier>();
    // See https://source.dot.net/#Microsoft.Extensions.Hosting.Systemd/SystemdLifetime.cs
    services.AddSingleton<IHostLifetime, SystemdLifetime>();
});

builder.WebHost.ConfigureKestrel(ko => {
    ko.UseSystemd();
});

var app = builder.Build();

app.MapGet("/", () => "Yo");

await app.RunAsync();