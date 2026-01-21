namespace Jtodo.Repositories
{
    public interface IAppUnit : IDisposable
    {
        TodoListRespository TodoListRepository { get; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
