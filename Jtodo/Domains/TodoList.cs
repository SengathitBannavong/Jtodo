
namespace Jtodo.Domains
{
    public class TodoList
    {
        private readonly List<TodoItem> _todo_items;
        private readonly UInt64 _id;
        private readonly string _title;
        private readonly string _description;

        public List<TodoItem> Todo_Items => _todo_items;
        public UInt64 Id => _id;
        public string Title => _title;
        public string Description => _description;

        public TodoList(UInt64 Id, string Title,string Description)
        {
            _todo_items = new List<TodoItem>();
            _id = Id;
            _title = Title;
            _description = Description;
        }

        public void Add_Todo_Item(TodoItem item)
        {
            _todo_items.Add(item);
        }

        public void Remove_Todo_Item(TodoItem item)
        {
            _todo_items.Remove(item);
        }

        public void Update_Todo_Item(TodoItem old_item, TodoItem new_item)
        {
            int index = _todo_items.IndexOf(old_item);
            if (index != -1)
            {
                _todo_items[index] = new_item;
            }
        }

    }
}
