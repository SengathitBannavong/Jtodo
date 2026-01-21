namespace Jtodo.Domains
{
    public class TodoListItem
    {
        public UInt64 TodoListId { get; set; }
        public UInt64 TodoItemId { get; set; }

        public TodoList? TodoList { get; set; }
        public TodoItem? TodoItem { get; set; }
    }
}
