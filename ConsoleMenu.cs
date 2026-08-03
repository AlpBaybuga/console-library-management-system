using System.Text;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Extensions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Statistics;
using LibraryManagementSystem.Validation;

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

            var continuePrefix = "İşlem tamamlandı.";

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
                    case "0":
                        if (ConfirmExit())
                            running = false;
                        else
                            continuePrefix = "İşlem iptal edildi.";
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim, tekrar deneyin.");
                        continuePrefix = "Hata oluştu.";
                        break;
                }
            }
            catch (GoBackException)
            {
                Console.WriteLine("İşlem iptal edildi, ana menüye dönülüyor.");
                continuePrefix = "İşlem iptal edildi.";
            }
            catch (LibraryException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                continuePrefix = "Hata oluştu.";
            }
            catch (FormatException)
            {
                Console.WriteLine("Hata: Girdiğiniz değer beklenen formatta değil (Id veya sayı hatalı olabilir).");
                continuePrefix = "Hata oluştu.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Beklenmeyen bir hata oluştu: {ex.Message}");
                continuePrefix = "Hata oluştu.";
            }

            if (running)
            {
                Console.WriteLine($"\n{continuePrefix} Devam etmek için herhangi bir tuşa basın...");
                Console.ReadKey(true);
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

    private static bool ConfirmExit()
    {
        while (true)
        {
            Console.Write("Kapatmak istediğinize emin misiniz? (E/H): ");
            var input = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (input == "E" || input == "EVET")
                return true;
            if (input == "H" || input == "HAYIR")
                return false;

            Console.WriteLine("Hata: Lütfen 'E' veya 'H' giriniz.\n");
        }
    }

    private sealed class GoBackException : Exception
    {
    }

    private static string? ReadLineOrGoBack(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (string.Equals(input?.Trim(), "geri", StringComparison.OrdinalIgnoreCase))
            throw new GoBackException();

        return input;
    }

    private static string WithCancelHint(string prompt)
    {
        var trimmed = prompt.TrimEnd().TrimEnd(':');
        return $"{trimmed} (iptal: 'geri'): ";
    }

    private void AddBook()
    {
        var title = ReadValidated("Başlık: ", Validator.ValidateTitle);
        var author = ReadValidated("Yazar: ", Validator.ValidateAuthor);
        var isbn = ReadValidated("ISBN: ", isbn =>
        {
            Validator.ValidateIsbn(isbn);
            if (_bookService.IsIsbnRegistered(isbn))
                throw new LibraryException($"'{isbn}' ISBN numarasına sahip bir kitap zaten kayıtlı.");
        });
        var year = ReadInt("Yayın Yılı: ", Validator.ValidatePublicationYear);
        var category = ReadCategory();

        var book = _bookService.AddBook(title, author, isbn, year, category);
        Console.WriteLine($"'{book.Title}' adlı kitap eklendi. Id: {book.Id}");
    }

    private void ListBooks() => PrintBooks(_bookService.GetAllBooks());

    private void UpdateBook()
    {
        var id = ReadGuid("Güncellenecek kitabın Id'si: ");
        var title = ReadValidated("Yeni Başlık: ", Validator.ValidateTitle);
        var author = ReadValidated("Yeni Yazar: ", Validator.ValidateAuthor);
        var year = ReadInt("Yeni Yayın Yılı: ", Validator.ValidatePublicationYear);
        var category = ReadCategory();

        _bookService.UpdateBook(id, title, author, year, category);
        Console.WriteLine("Kitap güncellendi.");
    }

    private void DeleteBook()
    {
        var id = ReadGuid("Silinecek kitabın Id'si: ");

        _bookService.DeleteBook(id);
        Console.WriteLine("Kitap silindi.");
    }

    private void AddMember()
    {
        var fullName = ReadValidated("Ad Soyad: ", Validator.ValidateFullName);
        var email = ReadValidated("E-posta: ", Validator.ValidateEmail);

        var member = _memberService.AddMember(fullName, email);
        Console.WriteLine($"'{member.FullName}' adlı üye eklendi. Id: {member.Id}");
    }

    private void ListMembers()
    {
        var members = _memberService.GetAllMembers();

        if (members.Count == 0)
        {
            Console.WriteLine("Üye bulunamadı.");
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
        var bookId = ReadGuid("Ödünç verilecek kitabın Id'si: ");
        var memberId = ReadGuid("Üyenin Id'si: ");

        var loan = _loanService.BorrowBook(bookId, memberId);
        Console.WriteLine($"Kitap ödünç verildi. İade tarihi: {loan.DueDate:dd.MM.yyyy}");
    }

    private void ReturnBook()
    {
        var bookId = ReadGuid("İade edilecek kitabın Id'si: ");

        _loanService.ReturnBook(bookId);
        Console.WriteLine("Kitap iade alındı.");
    }

    private void FilterAndSearch()
    {
        while (true)
        {
            Console.WriteLine("--- Filtrele ve Ara ---");
            Console.WriteLine("1. Kategoriye göre filtrele");
            Console.WriteLine("2. Duruma göre filtrele (rafta / ödünçte)");
            Console.WriteLine("3. Başlık veya yazara göre ara");
            Console.WriteLine("4. Gecikmiş ödünç kayıtlarını listele");
            Console.WriteLine("5. Bir üyenin ödünç aldığı kitapları listele");
            Console.WriteLine("6. Geri");
            var choice = ReadLineOrGoBack("Seçiminiz: ");

            switch (choice)
            {
                case "1":
                    var category = ReadCategory();
                    PrintBooks(_bookService.GetAllBooks().ByCategory(category));
                    return;

                case "2":
                    var isBorrowed = ReadInt("Durum (1: Rafta, 2: Ödünçte): ", value =>
                    {
                        if (value != 1 && value != 2)
                            throw new LibraryException("Geçersiz durum seçimi. 1 veya 2 giriniz.");
                    }) == 2;
                    PrintBooks(_bookService.GetAllBooks().ByBorrowStatus(isBorrowed));
                    return;

                case "3":
                    var keyword = ReadLineOrGoBack(WithCancelHint("Aranacak kelime (başlık veya yazar): ")) ?? string.Empty;
                    PrintBooks(_bookService.GetAllBooks().SearchByTitleOrAuthor(keyword));
                    return;

                case "4":
                    PrintLoans(_loanService.GetAllLoans().Overdue());
                    return;

                case "5":
                    var memberId = ReadGuid("Üyenin Id'si: ");
                    PrintLoans(_loanService.GetAllLoans().ByMember(memberId));
                    return;

                case "6":
                    throw new GoBackException();

                default:
                    Console.WriteLine("Hata: Geçersiz seçim. 1-6 arasında bir değer giriniz.");
                    break;
            }
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
            Console.WriteLine("Kitap bulunamadı.");
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
            Console.WriteLine("Ödünç kaydı bulunamadı.");
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
        while (true)
        {
            Console.WriteLine("Kategori seçin: 0-Novel 1-Science 2-History 3-Children 4-Other");
            var input = ReadLineOrGoBack("Kategori (iptal: 'geri'): ");

            if (int.TryParse(input, out var value) && Enum.IsDefined(typeof(BookCategory), value))
                return (BookCategory)value;

            Console.WriteLine("Hata: Geçersiz kategori seçimi. 0-4 arasında bir değer giriniz.\n");
        }
    }

    private static string ReadValidated(string prompt, Action<string> validate)
    {
        while (true)
        {
            var input = ReadLineOrGoBack(WithCancelHint(prompt)) ?? string.Empty;

            try
            {
                validate(input);
                return input;
            }
            catch (LibraryException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}\n");
            }
        }
    }

    private static int ReadInt(string prompt, Action<int>? validate = null)
    {
        while (true)
        {
            var input = ReadLineOrGoBack(WithCancelHint(prompt));

            if (!int.TryParse(input, out var value))
            {
                Console.WriteLine("Hata: Geçerli bir sayı giriniz.\n");
                continue;
            }

            try
            {
                validate?.Invoke(value);
                return value;
            }
            catch (LibraryException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}\n");
            }
        }
    }

    private static Guid ReadGuid(string prompt)
    {
        while (true)
        {
            var input = ReadLineOrGoBack(WithCancelHint(prompt));

            if (Guid.TryParse(input, out var value))
                return value;

            Console.WriteLine("Hata: Geçerli bir Id (Guid) giriniz.\n");
        }
    }
}
