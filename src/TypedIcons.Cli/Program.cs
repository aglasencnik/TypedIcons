using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TypedIcons.Cli.Commands;
using TypedIcons.Cli.Infrastructure;
using TypedIcons.Cli.Services;

var services = new ServiceCollection();
services.AddSingleton<InitializationService>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(config =>
{
    config.SetApplicationName("typedicons");
    config.SetApplicationVersion("0.1.0-alpha.1");
    
    config.AddCommand<InitCommand>("init");
});

return await app.RunAsync(args);