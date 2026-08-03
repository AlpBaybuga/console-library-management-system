using System.Text.RegularExpressions;
using LibraryManagementSystem.Exceptions;

namespace LibraryManagementSystem.Validation;

public static class Validator
{
    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex NamePattern = new(@"^[\p{L}][\p{L}\s'.-]*$", RegexOptions.Compiled);

    public static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length < 2 || title.Length > 150)
            throw new LibraryException("Başlık zorunludur ve 2-150 karakter arasında olmalıdır.");
    }

    public static void ValidateAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author) || author.Length < 3 || author.Length > 100)
            throw new LibraryException("Yazar adı zorunludur ve 3-100 karakter arasında olmalıdır.");

        if (!NamePattern.IsMatch(author))
            throw new LibraryException("Yazar adı yalnızca harf içermelidir, sayı veya özel karakter olamaz.");
    }

    public static void ValidateIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new LibraryException("ISBN boş bırakılamaz.");
    }

    public static void ValidatePublicationYear(int year)
    {
        if (year < 1450 || year > DateTime.Now.Year)
            throw new LibraryException($"Yayın yılı 1450 ile {DateTime.Now.Year} arasında olmalıdır.");
    }

    public static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3 || fullName.Length > 100)
            throw new LibraryException("Ad soyad zorunludur ve 3-100 karakter arasında olmalıdır.");

        if (!NamePattern.IsMatch(fullName))
            throw new LibraryException("Ad soyad yalnızca harf içermelidir, sayı veya özel karakter olamaz.");
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailPattern.IsMatch(email))
            throw new LibraryException("Geçerli bir e-posta adresi giriniz.");
    }
}
