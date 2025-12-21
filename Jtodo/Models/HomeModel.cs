using Jtodo.Domains;
using Jtodo.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Jtodo.Models
{
    public class HomeModel : INotifyPropertyChanged
    {
        private readonly TodoListService _todo_list;
        public ObservableCollection<TodoItem> TodoList { get; set; } = new();
        public ObservableCollection<TodoList> TodoLists { get; set; } = new();

        public HomeModel()
        {
            _todo_list = new TodoListService();
        }

        public void Load_All_Todo_List()
        {
            var todo_list = _todo_list.Get_All_Todo_list();
            TodoList.Clear();
            foreach (var list in todo_list)
            {
                foreach (var item in list.Todo_Items)
                {
                    TodoList.Add(item);
                }
            }

            // populate lists collection
            TodoLists.Clear();
            foreach (var list in todo_list)
            {
                TodoLists.Add(list);
            }
        }

        public void Load_Todo_List_ById(System.UInt64 id)
        {
            var list = _todo_list.Get_Todo_List(id);
            TodoList.Clear();
            if (list != null)
            {
                foreach (var item in list.Todo_Items)
                {
                    TodoList.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
