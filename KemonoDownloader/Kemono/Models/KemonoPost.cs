namespace KemonoDownloader.Models;

class KemonoPost {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string[]? Files { get; set; }
    public string[]? Attachments { get; set; }
}
