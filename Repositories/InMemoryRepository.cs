using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly Dictionary<Guid, T> _items = new();

    public void Add(T item) => _items[item.Id] = item;

    public IReadOnlyList<T> GetAll() => _items.Values.ToList();

    public T? GetById(Guid id) => _items.TryGetValue(id, out var item) ? item : default;

    public bool Remove(Guid id) => _items.Remove(id);
}
