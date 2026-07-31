using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Validation;

namespace LibraryManagementSystem.Services;

public class MemberService
{
    private readonly IRepository<Member> _members;

    public MemberService(IRepository<Member> members)
    {
        _members = members;
    }

    public Member AddMember(string fullName, string email)
    {
        Validator.ValidateFullName(fullName);
        Validator.ValidateEmail(email);

        var member = new Member
        {
            FullName = fullName,
            Email = email
        };

        _members.Add(member);
        return member;
    }

    public IReadOnlyList<Member> GetAllMembers() => _members.GetAll();

    public Member? GetMemberById(Guid id) => _members.GetById(id);
}
