using Jtodo.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtodo.Interfaces
{
    public interface ITodoListRepository
    {
        TodoList? Get_Todo_List(ulong id);
        List<TodoList> Get_All_Todo_list();
        void Add_Todo_List(TodoList todoList);
        void Update_Todo_List(TodoList todoList);
        void Delete_Todo_List(ulong id);
        bool Exists(ulong id);
    }
}
