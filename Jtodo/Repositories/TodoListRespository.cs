using Jtodo.Domains;
using Jtodo.Values;
using System;
using System.Collections.Generic;

namespace Jtodo.Repositories
{
    public class TodoListRespository
    {
        // This should be database connection
        // But i will use in-memory list for demo
        private readonly List<TodoList> _chunk_todo_list;

        public void Mock_Data()
        {
            UInt64 id = (UInt64)_chunk_todo_list.Count() + 1;
            TodoList todo_list = new TodoList(id,$"Mock_{id}", $"Mock_{id}");
            for (int i = 0; i < 20; i++)
            {
                var item = new TodoItem(
                    (UInt64)i,
                    $"TodoList_Title_{i}",
                    $"TodoList_Description_{i}",
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    Priority.Low,
                    Status.Pending,
                    1
                );
                todo_list.Add_Todo_Item(item);
            }
            _chunk_todo_list.Add(todo_list);
        }

        public TodoListRespository()
        {
            _chunk_todo_list = new List<TodoList>();
            for(int i = 0; i < 5; i++)
            {
                Mock_Data();
            }
        }

        public TodoList Get_Todo_List(UInt64 id)
        {
            // this func query
            //    SELECT *
            //    FROM TodoListItem tli
            //    INNER JOIN TodoList tl ON tli.todo_list_id = tl.id
            //    INNER JOIN TodoItem ti ON tli.todo_item_id = ti.id
            //    LEFT JOIN Type ty ON ti.type_id = ty.id
            //    WHERE tl.id = id;
            return _chunk_todo_list.FirstOrDefault(tl => tl.Id == id);
        }

        public List<TodoList> Get_All_Todo_list()
        {
            return _chunk_todo_list;
        }
    }
}
