using LibraryManagementSystem;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Services;

IRepository<Book> bookRepository = new InMemoryRepository<Book>();
IRepository<Member> memberRepository = new InMemoryRepository<Member>();

var bookService = new BookService(bookRepository);
var memberService = new MemberService(memberRepository);

var menu = new ConsoleMenu(bookService, memberService);
menu.Run();
