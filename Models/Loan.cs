namespace LibraryManagementSystem.Models;

public class Loan : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid BookId { get; set; }
    public required Guid MemberId { get; set; }
    public DateTime BorrowDate { get; init; } = DateTime.Now;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public bool IsOverdue => ReturnDate is null && DateTime.Now > DueDate;
}
