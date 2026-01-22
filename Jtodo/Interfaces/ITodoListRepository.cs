using Jtodo.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jtodo.Interfaces
{
    public interface ITodoListRepository
    {
        // Async methods
        Task<TodoList?> Get_Todo_List_Async(ulong id);
        Task<List<TodoList>> Get_All_Todo_list_Async();
        Task<ulong> Add_Todo_List_Async(TodoList todoList);
        Task Update_Todo_List_Async(TodoList todoList);
        Task Delete_Todo_List_Async(ulong id);
        Task<bool> Exists_Async(ulong id);
        Task Delete_Todo_List_With_Items_Async(ulong id);
    }
}
