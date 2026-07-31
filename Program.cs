using LibraryManagementSystem;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Services;

IRepository<Book> bookRepository = new InMemoryRepository<Book>();
IRepository<Member> memberRepository = new InMemoryRepository<Member>();
IRepository<Loan> loanRepository = new InMemoryRepository<Loan>();

var bookService = new BookService(bookRepository);
var memberService = new MemberService(memberRepository);
var loanService = new LoanService(loanRepository, bookService, memberService);

var menu = new ConsoleMenu(bookService, memberService, loanService);
menu.Run();
