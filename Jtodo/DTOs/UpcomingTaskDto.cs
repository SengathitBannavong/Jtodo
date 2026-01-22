using System;

namespace Jtodo.DTOs
{
    public class UpcomingTaskDto
    {
        public string TaskTitle { get; set; } = string.Empty;
        public string ListName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int DaysRemaining { get; set; }
        public string DueDateText { get; set; } = string.Empty;
        public string UrgencyColor { get; set; } = "#2196F3";
    }
}
