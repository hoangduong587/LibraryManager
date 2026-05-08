public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string? Genre { get; set; }
    public string? Author { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
}
