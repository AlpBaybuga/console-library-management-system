using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

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
