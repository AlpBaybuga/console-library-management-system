using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services;

public class BookService
{
    private readonly IRepository<Book> _books;

    public BookService(IRepository<Book> books)
    {
        _books = books;
    }

    public Book AddBook(string title, string author, string isbn, int publicationYear, BookCategory category)
    {
        if (_books.GetAll().Any(b => b.ISBN == isbn))
            throw new InvalidOperationException($"'{isbn}' ISBN numarasına sahip bir kitap zaten kayıtlı.");

        var book = new Book
        {
            Title = title,
            Author = author,
            ISBN = isbn,
            PublicationYear = publicationYear,
            Category = category
        };

        _books.Add(book);
        return book;
    }

    public IReadOnlyList<Book> GetAllBooks() => _books.GetAll();

    public Book? GetBookById(Guid id) => _books.GetById(id);

    public void UpdateBook(Guid id, string title, string author, int publicationYear, BookCategory category)
    {
        var book = _books.GetById(id)
            ?? throw new InvalidOperationException("Belirtilen Id ile bir kitap bulunamadı.");

        book.Title = title;
        book.Author = author;
        book.PublicationYear = publicationYear;
        book.Category = category;
    }

    public void DeleteBook(Guid id)
    {
        var book = _books.GetById(id)
            ?? throw new InvalidOperationException("Belirtilen Id ile bir kitap bulunamadı.");

        if (book.IsBorrowed)
            throw new InvalidOperationException("Ödünçte olan bir kitap silinemez.");

        _books.Remove(id);
    }
}
