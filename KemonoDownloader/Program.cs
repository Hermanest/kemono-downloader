using KemonoDownloader.Models;

namespace KemonoDownloader;

class Program {
    private static readonly DownloadHelper _helper = new(new());
    private static readonly KemonoAPI _api = new(_helper);

    public static void Main(string[] args) {
        while (true) {
            var res = Start();
            if (res == null) {
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine(res);
        }
    }

    public static string? Start() {
        var artists = default(KemonoArtist[]);
        AskForCorrectInput("Enter the artist name: ", x => {
            PrintWait();
            artists = _api.GetArtists();
            artists = KemonoUtils.FindArtists(artists!, x!);
            return artists.Length == 0 ? "Artist not found!" : null;
        });

        var artist = artists!.FirstOrDefault();
        if (artists!.Length > 1) {
            Console.WriteLine($"Available {artists.Length} artists:");
            var idx = 1;
            foreach (var item in artists) {
                Console.WriteLine($"{idx} - {item.Name} on {item.Service}");
                idx++;
            }
            idx = int.Parse(AskForCorrectInput("Select the artist. [idx]: ",
                x => int.TryParse(x, out var i) && i <= artists.Length && i > 0));
            artist = artists[idx - 1];
        }

        if (!AskForConfirmation($"Selected artist is {artist!.Name}. ")) return string.Empty;
        Console.WriteLine("Parsing posts...");
        PrintWait();

        var offset = 0;
        var postCount = 0;
        var postsList = new List<KemonoPost>();
        while (true) {
            var posts = _api.GetPosts(artist,
                offset, x => Console.WriteLine($"{x.Id} - {x.Name} done'd"));
            if (posts.Length == 0) break;
            postsList.AddRange(posts);
            postCount += posts.Length;
            offset += 50;
        }

        Console.WriteLine($"Parsed {postCount} posts. Downloading...");
        var path = $"{Path.GetDirectoryName(Environment.ProcessPath)}\\{artist.Name} posts";
        Directory.CreateDirectory(path);
        foreach (var item in postsList) {
            Console.Write($"{item.Id} - {item.Name} ");
            //create files and attachments list
            var files = new List<string>();
            if (item.Files is not null)
                files.AddRange(item.Files!);
            if (item.Attachments is not null)
                files.AddRange(item.Attachments!);
            //check is post contains anything
            if (files.Count == 0) {
                Console.WriteLine($"has no attachments");
                continue;
            }
            var postPath = $"{path}\\{item.Id}";
            if (!Directory.Exists(postPath)) {
                Directory.CreateDirectory(postPath);
            }
            Console.WriteLine("downloading:");
            foreach (var att in files) {
                var attPostfix = att.Remove(0, att.LastIndexOf('/') + 1);
                Console.Write($"  {attPostfix} ");
                var attPath = $"{postPath}\\{attPostfix}";
                if (File.Exists(attPath)) {
                    Console.WriteLine($"is already exists");
                    continue;
                }
                Console.Write("downloading ");
                var bytes = _helper.GetBytes(att);
                File.WriteAllBytes(attPath, bytes);
                Console.WriteLine("done'd");
            }
        }

        return null;
    }

    private static string AskForCorrectInput(string? text, Func<string, bool> comparer, bool line = false) {
        return AskForCorrectInput(text, x => comparer(x) ? null : "Invalid input!", line);
    }

    private static string AskForCorrectInput(string? text, Func<string, string?> comparer, bool line = false) {
        while (true) {
            if (text != null) {
                if (line) Console.WriteLine(text);
                else Console.Write(text);
            }
            var input = Console.ReadLine();
            if (comparer(input!) is string str and not null) {
                Console.WriteLine(str);
                continue;
            }
            return input!;
        }
    }

    private static bool AskForConfirmation(string? text) {
        return AskForCorrectInput(text + "Do you want to continue? [Y/N]: ",
            x => x.ToLower() is "y" or "n").ToLower() is "y";
    }

    private static void PrintWait() {
        Console.WriteLine("Please wait...");
    }
}