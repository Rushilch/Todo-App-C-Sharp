using ProductivityApp.MVVM;

namespace ProductivityApp.ViewModels
{
    public class MainViewViewModel : BaseViewModel
    {
        private BaseViewModel? _currentViewModel;
        private string _currentPage = "Dashboard";

        private readonly DashboardViewModel _dashboardViewModel;
        private readonly TasksViewModel _tasksViewModel;
        private readonly PomodoroViewModel _pomodoroViewModel;
        private readonly CalendarViewModel _calendarViewModel;
        private readonly ClockViewModel _clockViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly AnalyticsViewModel _analyticsViewModel;

        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                    NavigateToPage(value);
            }
        }

        public MainViewViewModel(
            DashboardViewModel dashboardViewModel,
            TasksViewModel tasksViewModel,
            PomodoroViewModel pomodoroViewModel,
            CalendarViewModel calendarViewModel,
            ClockViewModel clockViewModel,
            SettingsViewModel settingsViewModel,
            AnalyticsViewModel analyticsViewModel)
        {
            _dashboardViewModel = dashboardViewModel;
            _tasksViewModel = tasksViewModel;
            _pomodoroViewModel = pomodoroViewModel;
            _calendarViewModel = calendarViewModel;
            _clockViewModel = clockViewModel;
            _settingsViewModel = settingsViewModel;
            _analyticsViewModel = analyticsViewModel;

            CurrentViewModel = _dashboardViewModel;
        }

        public void NavigateToPage(string pageName)
        {
            CurrentViewModel = pageName switch
            {
                "Dashboard" => _dashboardViewModel,
                "Tasks" => _tasksViewModel,
                "Pomodoro" => _pomodoroViewModel,
                "Calendar" => _calendarViewModel,
                "Clock" => _clockViewModel,
                "Analytics" => _analyticsViewModel,
                "Settings" => _settingsViewModel,
                _ => _dashboardViewModel
            };
        }
    }
}
