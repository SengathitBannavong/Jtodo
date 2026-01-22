using Jtodo.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jtodo.Interfaces
{
    public interface ITodoItemRepository
    {
        Task<TodoItem?> GetTodoItemAsync(ulong id);
        Task<List<TodoItem>> GetAllTodoItemsAsync();
        Task<ulong> AddTodoItemAsync(TodoItem todoItem);
        Task UpdateTodoItemAsync(TodoItem todoItem);
        Task DeleteTodoItemAsync(ulong id);
    }
}
