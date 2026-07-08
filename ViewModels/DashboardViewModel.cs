using System.Collections.ObjectModel;
using ProductivityApp.Data.Models;
using ProductivityApp.MVVM;
using ProductivityApp.Services;

namespace ProductivityApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;
        private readonly IPomodoroService _pomodoroService;
        private readonly IAnalyticsService _analyticsService;

        private int _tasksCompleted;
        private int _pomodorosCompleted;
        private int _focusTimeMinutes;
        private ObservableCollection<TaskItem> _upcomingTasks = new();
        private DailyStats? _todayStats;

        public int TasksCompleted
        {
            get => _tasksCompleted;
            set => SetProperty(ref _tasksCompleted, value);
        }

        public int PomodorosCompleted
        {
            get => _pomodorosCompleted;
            set => SetProperty(ref _pomodorosCompleted, value);
        }

        public int FocusTimeMinutes
        {
            get => _focusTimeMinutes;
            set => SetProperty(ref _focusTimeMinutes, value);
        }

        public ObservableCollection<TaskItem> UpcomingTasks
        {
            get => _upcomingTasks;
            set => SetProperty(ref _upcomingTasks, value);
        }

        public DailyStats? TodayStats
        {
            get => _todayStats;
            set => SetProperty(ref _todayStats, value);
        }

        public DashboardViewModel(ITaskService taskService, IPomodoroService pomodoroService, IAnalyticsService analyticsService)
        {
            _taskService = taskService;
            _pomodoroService = pomodoroService;
            _analyticsService = analyticsService;
            _ = LoadDashboardAsync();
        }

        public async Task LoadDashboardAsync()
        {
            try
            {
                // Load today's stats
                TodayStats = await _analyticsService.GetDailyStatsAsync(DateTime.Today) ?? new DailyStats();
                TasksCompleted = await _taskService.GetTodaysCompletedCountAsync();
                PomodorosCompleted = await _pomodoroService.GetTodaysCompletedCountAsync();
                FocusTimeMinutes = await _pomodoroService.GetTotalFocusTimeAsync();

                // Load upcoming tasks
                var upcomingTasks = await _taskService.GetUpcomingTasksAsync(7);
                UpcomingTasks.Clear();
                foreach (var task in upcomingTasks.Take(5))
                {
                    UpcomingTasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard: {ex.Message}");
            }
        }
    }
}
