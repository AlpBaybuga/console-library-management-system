using System.Text;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Extensions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Statistics;

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
                    case "9": FilterAndSearch(); break;
                    case "10": ShowStatistics(); break;
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
        Console.WriteLine("9. Filtrele ve Ara");
        Console.WriteLine("10. İstatistikleri Görüntüle");
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

    private void ListBooks() => PrintBooks(_bookService.GetAllBooks());

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

    private void FilterAndSearch()
    {
        Console.WriteLine("--- Filtrele ve Ara ---");
        Console.WriteLine("1. Kategoriye göre filtrele");
        Console.WriteLine("2. Duruma göre filtrele (rafta / ödünçte)");
        Console.WriteLine("3. Başlık veya yazara göre ara");
        Console.WriteLine("4. Gecikmiş ödünç kayıtlarını listele");
        Console.WriteLine("5. Bir üyenin ödünç aldığı kitapları listele");
        Console.Write("Seçiminiz: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                var category = ReadCategory();
                PrintBooks(_bookService.GetAllBooks().ByCategory(category));
                break;

            case "2":
                Console.Write("Durum (1: Rafta, 2: Ödünçte): ");
                var isBorrowed = Console.ReadLine() == "2";
                PrintBooks(_bookService.GetAllBooks().ByBorrowStatus(isBorrowed));
                break;

            case "3":
                Console.Write("Aranacak kelime (başlık veya yazar): ");
                var keyword = Console.ReadLine() ?? string.Empty;
                PrintBooks(_bookService.GetAllBooks().SearchByTitleOrAuthor(keyword));
                break;

            case "4":
                PrintLoans(_loanService.GetAllLoans().Overdue());
                break;

            case "5":
                Console.Write("Üyenin Id'si: ");
                var memberId = Guid.Parse(Console.ReadLine() ?? string.Empty);
                PrintLoans(_loanService.GetAllLoans().ByMember(memberId));
                break;

            default:
                Console.WriteLine("Geçersiz seçim.");
                break;
        }
    }

    private void ShowStatistics()
    {
        var books = _bookService.GetAllBooks();
        var members = _memberService.GetAllMembers();
        var loans = _loanService.GetAllLoans();

        var sb = new StringBuilder();
        sb.AppendLine("=== Kütüphane İstatistikleri ===");
        sb.AppendLine($"Toplam kitap sayısı        : {LibraryStatistics.TotalBooks(books)}");
        sb.AppendLine($"Rafta olan kitap sayısı    : {LibraryStatistics.OnShelfBooksCount(books)}");
        sb.AppendLine($"Ödünçte olan kitap sayısı  : {LibraryStatistics.BorrowedBooksCount(books)}");
        sb.AppendLine($"Toplam üye sayısı          : {LibraryStatistics.TotalMembers(members)}");
        sb.AppendLine($"Gecikmiş ödünç kaydı       : {LibraryStatistics.OverdueLoanCount(loans)}");
        sb.AppendLine();

        sb.AppendLine("Kategoriye göre kitap dağılımı:");
        foreach (var (bookCategory, count) in LibraryStatistics.BooksByCategory(books))
            sb.AppendLine($"  {bookCategory,-10}: {count}");
        sb.AppendLine();

        var mostBorrowedBook = LibraryStatistics.MostBorrowedBook(loans, books);
        sb.AppendLine($"En çok ödünç alınan kitap    : {(mostBorrowedBook is not null ? mostBorrowedBook.Title : "Henüz veri yok")}");

        var mostBorrowedCategory = LibraryStatistics.MostBorrowedCategory(loans, books);
        sb.AppendLine($"En çok ödünç alınan kategori : {(mostBorrowedCategory is not null ? mostBorrowedCategory.ToString() : "Henüz veri yok")}");

        Console.WriteLine(sb.ToString());
    }

    private static void PrintBooks(IEnumerable<Book> books)
    {
        var list = books.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("Sonuç bulunamadı.");
            return;
        }

        var sb = new StringBuilder();
        foreach (var book in list)
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

    private static void PrintLoans(IEnumerable<Loan> loans)
    {
        var list = loans.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("Sonuç bulunamadı.");
            return;
        }

        var sb = new StringBuilder();
        foreach (var loan in list)
        {
            var status = loan.ReturnDate is not null
                ? $"İade edildi ({loan.ReturnDate:dd.MM.yyyy})"
                : loan.IsOverdue ? "GECİKMİŞ" : "Devam ediyor";

            sb.AppendLine($"Loan Id       : {loan.Id}");
            sb.AppendLine($"  Kitap Id    : {loan.BookId}");
            sb.AppendLine($"  Üye Id      : {loan.MemberId}");
            sb.AppendLine($"  Alış Tarihi : {loan.BorrowDate:dd.MM.yyyy}");
            sb.AppendLine($"  İade Tarihi : {loan.DueDate:dd.MM.yyyy}");
            sb.AppendLine($"  Durum       : {status}");
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    private static BookCategory ReadCategory()
    {
        Console.WriteLine("Kategori seçin: 0-Novel 1-Science 2-History 3-Children 4-Other");
        Console.Write("Kategori: ");
        var input = Console.ReadLine() ?? string.Empty;
        var category = (BookCategory)int.Parse(input);

        if (!Enum.IsDefined(category))
            throw new LibraryException("Geçersiz kategori seçimi. 0-4 arasında bir değer giriniz.");

        return category;
    }
}
