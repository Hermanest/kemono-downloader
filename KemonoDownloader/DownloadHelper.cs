using System.Text;

namespace KemonoDownloader;

class DownloadHelper {
    public DownloadHelper(HttpClient client) {
        _client = client;
    }

    private readonly HttpClient _client;

    public byte[] GetBytes(string url) {
        var result = Get(url);
        var stream = result.Content.ReadAsStream();
        return ReadUntilEnd(stream);
    }

    public string GetString(string url) {
        var result = Get(url);
        return Encoding.UTF8.GetString(result
            .Content.ReadAsByteArrayAsync().Result);
    }

    private HttpResponseMessage Get(string url) {
        return _client.Send(new() {
            Method = HttpMethod.Get,
            RequestUri = new UriBuilder(url).Uri
        });
    } 

    private static byte[] ReadUntilEnd(Stream stream) {
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        stream.Close();
        return buffer;
    }
}
