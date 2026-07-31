using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Statistics;

public static class LibraryStatistics
{
    public static int TotalBooks(IEnumerable<Book> books) => books.Count();

    public static int TotalMembers(IEnumerable<Member> members) => members.Count();

    public static int BorrowedBooksCount(IEnumerable<Book> books) => books.Count(b => b.IsBorrowed);

    public static int OnShelfBooksCount(IEnumerable<Book> books) => books.Count(b => !b.IsBorrowed);

    public static int OverdueLoanCount(IEnumerable<Loan> loans) => loans.Count(l => l.IsOverdue);

    public static Dictionary<BookCategory, int> BooksByCategory(IEnumerable<Book> books) =>
        books.GroupBy(b => b.Category).ToDictionary(g => g.Key, g => g.Count());

    public static Book? MostBorrowedBook(IEnumerable<Loan> loans, IEnumerable<Book> books)
    {
        var bestGroup = loans
            .GroupBy(l => l.BookId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return bestGroup is null ? null : books.FirstOrDefault(b => b.Id == bestGroup.Key);
    }

    public static BookCategory? MostBorrowedCategory(IEnumerable<Loan> loans, IEnumerable<Book> books)
    {
        var bookLookup = books.ToDictionary(b => b.Id);

        var bestGroup = loans
            .Where(l => bookLookup.ContainsKey(l.BookId))
            .GroupBy(l => bookLookup[l.BookId].Category)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return bestGroup?.Key;
    }
}
