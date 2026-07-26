namespace LibraryManagementSystem.Models;

public class Member : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime RegisteredAt { get; init; } = DateTime.Now;
}
