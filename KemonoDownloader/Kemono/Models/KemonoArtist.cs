using Newtonsoft.Json;

namespace KemonoDownloader.Models;

class KemonoArtist
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("service")]
    public string? Service { get; set; }

    [JsonProperty("indexed")]
    public string? Indexed { get; set; }

    [JsonProperty("updated")]
    public string? Updated { get; set; }
}
