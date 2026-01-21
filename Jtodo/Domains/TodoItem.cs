using Jtodo.Values;
using System.Text;

namespace Jtodo.Domains
{
    public class TodoItem {
        private UInt64 _id;
        private string _title;
        private string _description;
        private DateTime? _startDate;
        private DateTime? _dueDate;
        private Priority _priority;
        private Status _status;
        private UInt64 _type_id;

        public string Description { get => _description; private set => _description = value; }
        public DateTime? StartDate { get => _startDate; private set => _startDate = value; }
        public DateTime? DueDate { get => _dueDate; private set => _dueDate = value; }
        public Priority Priority { get => _priority; private set => _priority = value; }
        public Status Status { get => _status; private set => _status = value; }
        public UInt64 TypeId { get => _type_id; private set => _type_id = value; }
        public UInt64 Id { get => _id; private set => _id = value; }
        public string Title { get => _title; private set => _title = value; }

        // Constructor for EF Core (private)
        private TodoItem()
        {
            _title = string.Empty;
            _description = string.Empty;
        }

        public TodoItem(
            UInt64 Id, string Title, string Description,
            DateTime StartDate,DateTime DueDate,
            Priority Priority, Status Status, UInt64 TypeId
        )
        {
            _id = Id;
            _title = Title;
            _description = Description;
            _startDate = StartDate;
            _dueDate = DueDate;
            _priority = Priority;
            _status = Status;
            _type_id = TypeId;
        }

    }
}
