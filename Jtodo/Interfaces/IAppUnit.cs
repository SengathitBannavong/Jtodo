using Jtodo.Repositories;

namespace Jtodo.Interfaces
{
    public interface IAppUnit : IDisposable
    {
        TodoListRespository TodoListRepository { get; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
