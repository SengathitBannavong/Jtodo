using Jtodo.Repositories;
using Jtodo.Domains;
using System.Collections.Generic;
using System.Linq;

namespace Jtodo.Services
{
    public class TodoListService
    {
        private readonly TodoListRespository _todo_list_respository;

        public TodoListService()
        {
            _todo_list_respository = new TodoListRespository();
        }

        public List<TodoList> Get_All_Todo_list()
        {
           return _todo_list_respository.Get_All_Todo_list();
        }

        public TodoList? Get_Todo_List(System.UInt64 id)
        {
            return _todo_list_respository.Get_Todo_List(id);
        }

        // Return all list ids (and titles) for selection in UI
        public List<System.UInt64> Get_All_ListIds()
        {
            return _todo_list_respository.Get_All_Todo_list().Select(t => t.Id).ToList();
        }

        // Optionally return list summaries (id + title)
        public List<(System.UInt64 Id, string Title)> Get_All_ListSummaries()
        {
            return _todo_list_respository.Get_All_Todo_list().Select(t => (t.Id, t.Title)).ToList();
        }
    }
}
