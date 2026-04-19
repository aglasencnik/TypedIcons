using Spectre.Console.Cli;
using TypedIcons.Cli.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("typedicons");
    config.SetApplicationVersion("0.1.0-alpha.1");
    
    config.AddCommand<PlaceholderCommand>("placeholder");
});

return await app.RunAsync(args);