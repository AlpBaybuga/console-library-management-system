using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services;

public class LoanService
{
    private const int MaxActiveLoansPerMember = 3;
    private const int LoanPeriodDays = 14;

    private readonly IRepository<Loan> _loans;
    private readonly BookService _bookService;
    private readonly MemberService _memberService;

    public LoanService(IRepository<Loan> loans, BookService bookService, MemberService memberService)
    {
        _loans = loans;
        _bookService = bookService;
        _memberService = memberService;
    }

    public Loan BorrowBook(Guid bookId, Guid memberId)
    {
        var book = _bookService.GetBookById(bookId)
            ?? throw new LibraryException("Belirtilen Id ile bir kitap bulunamadı.");

        if (book.IsBorrowed)
            throw new LibraryException("Bu kitap zaten ödünçte, tekrar ödünç verilemez.");

        var member = _memberService.GetMemberById(memberId)
            ?? throw new LibraryException("Belirtilen Id ile bir üye bulunamadı.");

        var activeLoanCount = _loans.GetAll().Count(l => l.MemberId == memberId && l.ReturnDate is null);
        if (activeLoanCount >= MaxActiveLoansPerMember)
            throw new LibraryException(
                $"'{member.FullName}' zaten en fazla {MaxActiveLoansPerMember} kitap ödünç almış durumda.");

        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            DueDate = DateTime.Now.AddDays(LoanPeriodDays)
        };

        _loans.Add(loan);
        book.IsBorrowed = true;

        return loan;
    }

    public Loan ReturnBook(Guid bookId)
    {
        var loan = _loans.GetAll().FirstOrDefault(l => l.BookId == bookId && l.ReturnDate is null)
            ?? throw new LibraryException("Bu kitaba ait, iade edilmemiş bir ödünç kaydı bulunamadı.");

        loan.ReturnDate = DateTime.Now;

        var book = _bookService.GetBookById(bookId);
        if (book is not null)
            book.IsBorrowed = false;

        return loan;
    }

    public IReadOnlyList<Loan> GetAllLoans() => _loans.GetAll();
}
