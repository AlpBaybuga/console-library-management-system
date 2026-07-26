using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories;

public interface IRepository<T> where T : IEntity
{
    void Add(T item);
    IReadOnlyList<T> GetAll();
    T? GetById(Guid id);
    bool Remove(Guid id);
}
