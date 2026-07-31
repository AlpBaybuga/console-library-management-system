using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Extensions;

public static class BookQueryExtensions
{
    public static IEnumerable<Book> ByCategory(this IEnumerable<Book> books, BookCategory category) =>
        books.Where(b => b.Category == category);

    public static IEnumerable<Book> ByBorrowStatus(this IEnumerable<Book> books, bool isBorrowed) =>
        books.Where(b => b.IsBorrowed == isBorrowed);

    public static IEnumerable<Book> SearchByTitleOrAuthor(this IEnumerable<Book> books, string keyword) =>
        books.Where(b =>
            b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase));
}
