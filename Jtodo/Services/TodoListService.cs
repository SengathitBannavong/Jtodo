using Jtodo.Domains;
using System.Collections.Generic;
using System.Linq;
using Jtodo.Interfaces;

namespace Jtodo.Services
{
    public class TodoListService
    {
        private readonly IAppUnit _unitOfWork;

        public TodoListService(IAppUnit unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<TodoList> Get_All_Todo_list()
        {
           return _unitOfWork.TodoListRepository.Get_All_Todo_list();
        }

        public TodoList? Get_Todo_List(System.UInt64 id)
        {
            return _unitOfWork.TodoListRepository.Get_Todo_List(id);
        }

        // Return all list ids (and titles) for selection in UI
        public List<System.UInt64> Get_All_ListIds()
        {
            return _unitOfWork.TodoListRepository.Get_All_Todo_list().Select(t => t.Id).ToList();
        }

        // Optionally return list summaries (id + title)
        public List<(System.UInt64 Id, string Title)> Get_All_ListSummaries()
        {
            return _unitOfWork.TodoListRepository.Get_All_Todo_list().Select(t => (t.Id, t.Title)).ToList();
        }
    }
}
