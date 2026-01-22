using Jtodo.Domains;
using Jtodo.Repositories;

namespace Jtodo.Interfaces
{
    public interface IAppUnit : IDisposable
    {
        TodoListRepository TodoListRepository { get; }
        TypeRepository TypeRepository { get; }
        TodoItemRepository TodoItemRepository { get; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
        SQLite GetDbContext();
    }
}

