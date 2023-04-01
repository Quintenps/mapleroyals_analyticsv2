using System.Text.Json;

namespace MapleRoyalsPlayerCount;

public class MapleRoyalsService
{
    private readonly AppConfig _appConfig;
    private readonly HttpClient _httpClient;

    public MapleRoyalsService(AppConfig appConfig, HttpClient httpClient)
    {
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<MapleStats?> GetPlayerCount()
    {
        var response = await _httpClient.GetAsync(_appConfig.Endpoint);

        return await JsonSerializer.DeserializeAsync<MapleStats>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

public class MapleStats
{
    public int OnlineCount { get; set; }
    public bool Online { get; set; }
}