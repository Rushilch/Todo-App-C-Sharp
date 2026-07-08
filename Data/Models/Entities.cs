namespace ProductivityApp.Data.Models
{
    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum TaskCategory
    {
        Work = 0,
        Personal = 1,
        Study = 2,
        Health = 3,
        Other = 4
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskCategory Category { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class PomodoroSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Duration { get; set; } // in minutes
        public bool IsCompleted { get; set; }
        public int SessionNumber { get; set; } // track which pomodoro of the day
        public string? TaskName { get; set; } // optional task name
    }

    public class TimerSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; } // in seconds
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan? EventTime { get; set; }
        public int? TaskId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasReminder { get; set; }
        public int ReminderMinutesBefore { get; set; } = 15;
    }

    public class AppSettings
    {
        public int Id { get; set; } = 1; // Only one settings record
        public int PomodoroFocusMinutes { get; set; } = 25;
        public int PomodoroBreakMinutes { get; set; } = 5;
        public int PomodoroLongBreakMinutes { get; set; } = 15;
        public int SessionsBeforeLongBreak { get; set; } = 4;
        public bool DarkModeEnabled { get; set; } = false;
        public bool NotificationsEnabled { get; set; } = true;
        public bool Use24HourFormat { get; set; } = false;
        public string Theme { get; set; } = "Light";
    }

    public class DailyStats
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TasksCompleted { get; set; }
        public int PomodorosCompleted { get; set; }
        public int TotalFocusTimeMinutes { get; set; }
    }
}
