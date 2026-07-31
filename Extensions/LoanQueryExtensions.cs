using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Extensions;

public static class LoanQueryExtensions
{
    public static IEnumerable<Loan> Overdue(this IEnumerable<Loan> loans) =>
        loans.Where(l => l.IsOverdue);

    public static IEnumerable<Loan> ByMember(this IEnumerable<Loan> loans, Guid memberId) =>
        loans.Where(l => l.MemberId == memberId);
}
