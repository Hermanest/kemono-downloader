using KemonoDownloader.Models;
using static KemonoDownloader.KemonoConstants;

namespace KemonoDownloader;

class KemonoAPI {
    public KemonoAPI(DownloadHelper helper) {
        _downloadHelper = helper;
    }

    private readonly DownloadHelper _downloadHelper;

    public KemonoArtist[]? GetArtists() {
        return KemonoParser.ParseArtists(_downloadHelper
            .GetString(KEMONO_HOST + KEMONO_API_ENDPOINT + KEMONO_ARTISTS_ENDPOINT));
    }

    public KemonoPost[] GetPosts(KemonoArtist artist, int offset = 0, Action<KemonoPost>? parseCallback = null) {
        var url = KemonoParser.CreateArtistUrl(artist) + "?o=" + offset;
        var page = _downloadHelper.GetString(url);
        var links = KemonoParser.ParsePostLinks(page);
        var posts = new KemonoPost[links.Count()];
        var index = 0;
        foreach (var link in links) {
            page = _downloadHelper.GetString(KEMONO_HOST + "/" + link);
            var post = KemonoParser.ParsePost(page);
            parseCallback?.Invoke(post);
            posts[index++] = post;
        }
        return posts;
    }
}
