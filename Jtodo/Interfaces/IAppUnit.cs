using Jtodo.Repositories;

namespace Jtodo.Interfaces
{
    public interface IAppUnit : IDisposable
    {
        TodoListRepository TodoListRepository { get; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
