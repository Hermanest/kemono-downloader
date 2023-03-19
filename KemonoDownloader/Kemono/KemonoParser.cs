using KemonoDownloader.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static KemonoDownloader.KemonoConstants;

namespace KemonoDownloader;

static class KemonoParser {
    public static IEnumerable<string> ParsePostLinks(string page) {
        var a = new Regex(@"\w*" + KEMONO_USER_ENDPOINT + @"/\d*" + KEMONO_POST_ENDPOINT + @"/\d*").Matches(page);
        return a.Select(x => x.Value);
    }

    public static KemonoPost ParsePost(string source) {
        source = source.Replace("\n", "");
        var post = new KemonoPost();

        var postMeta = new Regex(
            @"<meta name=""id"" content=""\d*"">")
            .Match(source).Value;
        int.TryParse(new Regex(
            @".*content=""|"">")
            .Replace(postMeta, ""), out var id);
        post.Id = id;

        var postTitle = new Regex(
            @"class=""post__title"">\s*?<span>.*?</span>")
            .Match(source).Value;
        post.Name = new Regex(
            @".*?[^/]span>.*?|</span>")
            .Replace(postTitle, "");

        var fileThumbs = string.Join("", new Regex(
            @"class=""fileThumb""\s* href="".*?""\s*")
            .Matches(source));
        post.Files = ParseDataServerLinks(fileThumbs);

        var attachments = string.Join("", new Regex(
           @"class=""post__attachment-link""\s* href="".*?""\s*")
           .Matches(source));
        post.Attachments = ParseDataServerLinks(attachments);

        return post;
    }

    public static KemonoArtist[]? ParseArtists(string source) {
        return JsonConvert.DeserializeObject<KemonoArtist[]>(source);
    }

    public static string CreateArtistUrl(KemonoArtist artist) {
        return $"{KEMONO_HOST}/{artist.Service}{KEMONO_USER_ENDPOINT}/{artist.Id}";
    }

    private static string[] ParseDataServerLinks(string str) {
        return new Regex(
            @"\w*\." + KEMONO_HOST
            .Replace(".", "\\.") + @"/data/.*?/.*?/.*?\.\w*")
            .Matches(str).Select(x => x.Value).ToArray();
    }
}
