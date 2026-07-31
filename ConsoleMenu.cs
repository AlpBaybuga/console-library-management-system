using System.Text;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem;

public class ConsoleMenu
{
    private readonly BookService _bookService;
    private readonly MemberService _memberService;
    private readonly LoanService _loanService;

    public ConsoleMenu(BookService bookService, MemberService memberService, LoanService loanService)
    {
        _bookService = bookService;
        _memberService = memberService;
        _loanService = loanService;
    }

    public void Run()
    {
        var running = true;
        while (running)
        {
            PrintMainMenu();
            var choice = Console.ReadLine();

            if (choice is null)
            {
                running = false;
                break;
            }

            try
            {
                switch (choice)
                {
                    case "1": AddBook(); break;
                    case "2": ListBooks(); break;
                    case "3": UpdateBook(); break;
                    case "4": DeleteBook(); break;
                    case "5": AddMember(); break;
                    case "6": ListMembers(); break;
                    case "7": BorrowBook(); break;
                    case "8": ReturnBook(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Geçersiz seçim, tekrar deneyin."); break;
                }
            }
            catch (LibraryException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Hata: Girdiğiniz değer beklenen formatta değil (Id veya sayı hatalı olabilir).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Beklenmeyen bir hata oluştu: {ex.Message}");
            }

            if (running)
            {
                Console.WriteLine("\nDevam etmek için Enter'a basın...");
                Console.ReadLine();
            }
        }
    }

    private static void PrintMainMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Kütüphane Yönetim Sistemi ===");
        Console.WriteLine("1. Kitap Ekle");
        Console.WriteLine("2. Kitapları Listele");
        Console.WriteLine("3. Kitap Güncelle");
        Console.WriteLine("4. Kitap Sil");
        Console.WriteLine("5. Üye Ekle");
        Console.WriteLine("6. Üyeleri Listele");
        Console.WriteLine("7. Kitap Ödünç Ver");
        Console.WriteLine("8. Kitap İade Al");
        Console.WriteLine("0. Çıkış");
        Console.Write("Seçiminiz: ");
    }

    private void AddBook()
    {
        Console.Write("Başlık: ");
        var title = Console.ReadLine() ?? string.Empty;

        Console.Write("Yazar: ");
        var author = Console.ReadLine() ?? string.Empty;

        Console.Write("ISBN: ");
        var isbn = Console.ReadLine() ?? string.Empty;

        Console.Write("Yayın Yılı: ");
        var year = int.Parse(Console.ReadLine() ?? "0");

        var category = ReadCategory();

        var book = _bookService.AddBook(title, author, isbn, year, category);
        Console.WriteLine($"'{book.Title}' adlı kitap eklendi. Id: {book.Id}");
    }

    private void ListBooks()
    {
        var books = _bookService.GetAllBooks();

        if (books.Count == 0)
        {
            Console.WriteLine("Kayıtlı kitap bulunmuyor.");
            return;
        }

        var sb = new StringBuilder();
        foreach (var book in books)
        {
            sb.AppendLine($"Id: {book.Id}");
            sb.AppendLine($"  Başlık       : {book.Title}");
            sb.AppendLine($"  Yazar        : {book.Author}");
            sb.AppendLine($"  ISBN         : {book.ISBN}");
            sb.AppendLine($"  Yayın Yılı   : {book.PublicationYear}");
            sb.AppendLine($"  Kategori     : {book.Category}");
            sb.AppendLine($"  Durum        : {(book.IsBorrowed ? "Ödünçte" : "Rafta")}");
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    private void UpdateBook()
    {
        Console.Write("Güncellenecek kitabın Id'si: ");
        var id = Guid.Parse(Console.ReadLine() ?? string.Empty);

        Console.Write("Yeni Başlık: ");
        var title = Console.ReadLine() ?? string.Empty;

        Console.Write("Yeni Yazar: ");
        var author = Console.ReadLine() ?? string.Empty;

        Console.Write("Yeni Yayın Yılı: ");
        var year = int.Parse(Console.ReadLine() ?? "0");

        var category = ReadCategory();

        _bookService.UpdateBook(id, title, author, year, category);
        Console.WriteLine("Kitap güncellendi.");
    }

    private void DeleteBook()
    {
        Console.Write("Silinecek kitabın Id'si: ");
        var id = Guid.Parse(Console.ReadLine() ?? string.Empty);

        _bookService.DeleteBook(id);
        Console.WriteLine("Kitap silindi.");
    }

    private void AddMember()
    {
        Console.Write("Ad Soyad: ");
        var fullName = Console.ReadLine() ?? string.Empty;

        Console.Write("E-posta: ");
        var email = Console.ReadLine() ?? string.Empty;

        var member = _memberService.AddMember(fullName, email);
        Console.WriteLine($"'{member.FullName}' adlı üye eklendi. Id: {member.Id}");
    }

    private void ListMembers()
    {
        var members = _memberService.GetAllMembers();

        if (members.Count == 0)
        {
            Console.WriteLine("Kayıtlı üye bulunmuyor.");
            return;
        }

        var sb = new StringBuilder();
        foreach (var member in members)
        {
            sb.AppendLine($"Id: {member.Id}");
            sb.AppendLine($"  Ad Soyad      : {member.FullName}");
            sb.AppendLine($"  E-posta       : {member.Email}");
            sb.AppendLine($"  Kayıt Tarihi  : {member.RegisteredAt:dd.MM.yyyy}");
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    private void BorrowBook()
    {
        Console.Write("Ödünç verilecek kitabın Id'si: ");
        var bookId = Guid.Parse(Console.ReadLine() ?? string.Empty);

        Console.Write("Üyenin Id'si: ");
        var memberId = Guid.Parse(Console.ReadLine() ?? string.Empty);

        var loan = _loanService.BorrowBook(bookId, memberId);
        Console.WriteLine($"Kitap ödünç verildi. İade tarihi: {loan.DueDate:dd.MM.yyyy}");
    }

    private void ReturnBook()
    {
        Console.Write("İade edilecek kitabın Id'si: ");
        var bookId = Guid.Parse(Console.ReadLine() ?? string.Empty);

        _loanService.ReturnBook(bookId);
        Console.WriteLine("Kitap iade alındı.");
    }

    private static BookCategory ReadCategory()
    {
        Console.WriteLine("Kategori seçin: 0-Novel 1-Science 2-History 3-Children 4-Other");
        Console.Write("Kategori: ");
        var input = Console.ReadLine() ?? string.Empty;
        return (BookCategory)int.Parse(input);
    }
}
