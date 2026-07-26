namespace LibraryManagementSystem.Models;

public class Book : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string ISBN { get; set; }
    public int PublicationYear { get; set; }
    public BookCategory Category { get; set; }
    public bool IsBorrowed { get; set; }
}
