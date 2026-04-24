using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TypedIcons.Cli.Commands;
using TypedIcons.Cli.Infrastructure;
using TypedIcons.Cli.Services;

var services = new ServiceCollection();

services.AddHttpClient("iconify", (serviceProvider, client) =>
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
    config.SetApplicationVersion("0.1.0-alpha.5");
    
    config.AddCommand<InitCommand>("init");
    config.AddCommand<AddCommand>("add");
    config.AddCommand<RestoreCommand>("restore");
});

return await app.RunAsync(args);