namespace TypedIcons.Cli.Services;

public class IconifyService(IHttpClientFactory httpClientFactory)
{
    public async Task<string?> GetIconSvgAsync(string set, string name, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient("iconify");
        var result = await httpClient.GetAsync($"{set}/{name}.svg", cancellationToken);
        return result.IsSuccessStatusCode ? await result.Content.ReadAsStringAsync(cancellationToken) : null;
    }
}