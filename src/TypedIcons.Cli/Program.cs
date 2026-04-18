using Spectre.Console.Cli;
using TypedIcons.Cli.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<PlaceholderCommand>("placeholder");
});

return await app.RunAsync(args);