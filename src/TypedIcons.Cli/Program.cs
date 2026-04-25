using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TypedIcons.Cli.Commands;
using TypedIcons.Cli.Infrastructure;
using TypedIcons.Cli.Services;

var services = new ServiceCollection();

services.AddHttpClient("iconify", (_, client) =>
{
    client.BaseAddress = new Uri("https://api.iconify.design");
});

services.AddSingleton<IconifyService>();
services.AddSingleton<InitializationService>();
services.AddSingleton<IconService>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(config =>
{
    config.SetApplicationName("typedicons");
    config.SetApplicationVersion("0.1.0-alpha.10");
    
    config.AddCommand<InitCommand>("init")
        .WithDescription("Initialize TypedIcons in the current project");
    
    config.AddCommand<AddCommand>("add")
        .WithDescription("Add an icon by name (<set>:<icon>)");
    
    config.AddCommand<RestoreCommand>("restore")
        .WithDescription("Restore icons in the local cache");
});

return await app.RunAsync(args);