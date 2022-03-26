using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSystemd();

builder.WebHost.ConfigureKestrel(ko => {
    ko.UseSystemd();
});

var app = builder.Build();

app.MapGet("/", () => "Yo");

await app.RunAsync();