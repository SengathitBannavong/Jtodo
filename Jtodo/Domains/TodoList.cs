
namespace Jtodo.Domains
{
    public class TodoList
    {
        private List<TodoItem> _todo_items;
        private UInt64 _id;
        private string _title;
        private string _description;
        private DateTime _createDate;

        public List<TodoItem> Todo_Items => _todo_items;
        public UInt64 Id { get => _id; private set => _id = value; }
        public string Title { get => _title; private set => _title = value; }
        public string Description { get => _description; private set => _description = value; }
        public DateTime CreateDate { get => _createDate; private set => _createDate = value; }

        // Constructor for EF Core (private)
        private TodoList()
        {
            _todo_items = new List<TodoItem>();
            _title = string.Empty;
            _description = string.Empty;
        }

        public TodoList(UInt64 Id, string Title, string Description, DateTime CreateDate)
        {
            _todo_items = new List<TodoItem>();
            _id = Id;
            _title = Title;
            _description = Description;
            _createDate = CreateDate;
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
