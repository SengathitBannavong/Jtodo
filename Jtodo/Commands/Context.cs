using System.Text;

namespace Jtodo.Commands
{
    class Context
    {
        private string Topic;
        private string Description;
        private DateTime DueDate;
        private int Priority;

        public Context(string topic, string description, DateTime dueDate, int priority)
        {
            Topic = topic;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
        }

        public Context()
        {
            Topic = string.Empty;
            Description = string.Empty;
            DueDate = DateTime.MinValue;
            Priority = 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Topic: {Topic}");
            sb.AppendLine($"Description: {Description}");
            sb.AppendLine($"Due Date: {DueDate.ToShortDateString()}");
            sb.AppendLine($"Priority: {Priority}");
            return sb.ToString();
        }

        // get and set Topic
        public string GetTopic() => Topic;
        public void SetTopic(string topic) => Topic = topic;

        // get and set Description
        public string GetDescription() => Description;
        public void SetDescription(string description) => Description = description;

        // get and set DueDate
        public DateTime GetDueDate() => DueDate;
        public void SetDueDate(DateTime dueDate) => DueDate = dueDate;

        // get and set Priority
        public int GetPriority() => Priority;
        public void SetPriority(int priority) => Priority = priority;

    }
}
