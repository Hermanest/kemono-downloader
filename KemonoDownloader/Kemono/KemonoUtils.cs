using KemonoDownloader.Models;

namespace KemonoDownloader;

static class KemonoUtils {
    public static KemonoArtist[] FindArtists(KemonoArtist[] artists, string name) {
        return artists.Where(x => x.Name!.Contains(name)).ToArray();
    }
}

