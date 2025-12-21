using Jtodo.Values;
using System.Text;

namespace Jtodo.Domains
{
    public class TodoItem {
        private readonly UInt64 _id;
        private readonly string _title;
        private readonly string _description;
        private readonly DateTime _startDate;
        private readonly DateTime _dueDate;
        private readonly Priority _priority;
        private readonly Status _status;
        private readonly UInt64 _type_id;

        public string Description => _description;
        public DateTime StartDate => _startDate;
        public DateTime DueDate => _dueDate;
        public Priority Priority => _priority;
        public Status Status => _status;
        public UInt64 TypeId => _type_id;
        public UInt64 Id => _id;
        public string Title => _title;


        public TodoItem(
            UInt64 Id, string Title, string Description,
            DateTime Starttime,DateTime Duetime,
            Priority Priority, Status Status, UInt64 TypeId
        )
        {
            _id = Id;
            _title = Title;
            _description = Description;
            _startDate = Starttime;
            _dueDate = Duetime;
            _priority = Priority;
            _status = Status;
            _type_id = TypeId;
        }

    }
}
