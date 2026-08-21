using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using ProductivityApp.Data.Models;
using ProductivityApp.MVVM;
using ProductivityApp.Services;

namespace ProductivityApp.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        private readonly ICalendarService _calendarService;
        private readonly ITaskService _taskService;
        private DateTime _selectedMonth;
        private ObservableCollection<CalendarEvent> _monthEvents = new();
        private ObservableCollection<CalendarEvent> _selectedDayEvents = new();
        private DateTime _selectedDate = DateTime.Today;
        private string _currentMonthYear = DateTime.Now.ToString("MMMM yyyy");

        public DateTime SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    CurrentMonthYear = _selectedMonth.ToString("MMMM yyyy");
                    _ = LoadMonthEventsAsync();
                }
            }
        }

        public string CurrentMonthYear
        {
            get => _currentMonthYear;
            set => SetProperty(ref _currentMonthYear, value);
        }

        public ObservableCollection<CalendarEvent> MonthEvents
        {
            get => _monthEvents;
            set => SetProperty(ref _monthEvents, value);
        }

        public ObservableCollection<CalendarEvent> SelectedDayEvents
        {
            get => _selectedDayEvents;
            set => SetProperty(ref _selectedDayEvents, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                    _ = LoadSelectedDayEventsAsync();
            }
        }

        public ObservableCollection<DateTime> CalendarDays { get; } = new();

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand SelectDateCommand { get; }

        public CalendarViewModel(ICalendarService calendarService, ITaskService taskService)
        {
            _calendarService = calendarService;
            _taskService = taskService;
            _selectedMonth = DateTime.Now;

            // Initialize commands
            PreviousMonthCommand = new RelayCommand(_ => NavigatePreviousMonth());
            NextMonthCommand = new RelayCommand(_ => NavigateNextMonth());
            AddEventCommand = new RelayCommand(_ => AddNewEvent());
            SelectDateCommand = new RelayCommand<DateTime>(d => { SelectedDate = d; });

            _ = LoadMonthEventsAsync();
        }

        public async Task LoadMonthEventsAsync()
        {
            try
            {
                BuildCalendarDays();
                var events = await _calendarService.GetEventsForMonthAsync(SelectedMonth);
                MonthEvents.Clear();
                foreach (var @event in events)
                    MonthEvents.Add(@event);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading month events: {ex.Message}");
            }
        }

        public async Task LoadSelectedDayEventsAsync()
        {
            try
            {
                var events = await _calendarService.GetEventsForDayAsync(SelectedDate);
                SelectedDayEvents.Clear();
                foreach (var @event in events)
                    SelectedDayEvents.Add(@event);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading day events: {ex.Message}");
            }
        }

        public async Task AddEventAsync(CalendarEvent @event)
        {
            try
            {
                await _calendarService.AddEventAsync(@event);
                await LoadMonthEventsAsync();
                if (@event.EventDate == SelectedDate)
                    await LoadSelectedDayEventsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding event: {ex.Message}");
            }
        }

        private void AddNewEvent()
        {
            var newEvent = new CalendarEvent
            {
                Title = "New Event",
                Description = "Click to edit",
                EventDate = SelectedDate,
                CreatedAt = DateTime.Now
            };
            _ = AddEventAsync(newEvent);
        }

        public void NavigatePreviousMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(-1);
        }

        public void NavigateNextMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(1);
        }

        public void NavigateToToday()
        {
            SelectedMonth = DateTime.Now;
            SelectedDate = DateTime.Today;
        }

        private void BuildCalendarDays()
        {
            CalendarDays.Clear();

            var firstDay = new DateTime(SelectedMonth.Year, SelectedMonth.Month, 1);
            var start = firstDay.AddDays(-(int)firstDay.DayOfWeek);

            for (var i = 0; i < 42; i++)
                CalendarDays.Add(start.AddDays(i));
        }
    }

    public class ClockViewModel : BaseViewModel
    {
        private DateTime _currentTime;
        private string _dayOfWeek = "";
        private int _weekNumber = 0;
        private int _daysLeftInYear = 0;
        private DispatcherTimer? _timer;

        public DateTime CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public string DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }

        public int WeekNumber
        {
            get => _weekNumber;
            set => SetProperty(ref _weekNumber, value);
        }

        public int DaysLeftInYear
        {
            get => _daysLeftInYear;
            set => SetProperty(ref _daysLeftInYear, value);
        }

        public string CurrentTime_Display => CurrentTime.ToString("HH:mm:ss");
        public string CurrentDate => CurrentTime.ToString("dddd, MMMM d, yyyy");

        public ClockViewModel()
        {
            CurrentTime = DateTime.Now;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) =>
            {
                CurrentTime = DateTime.Now;
                DayOfWeek = DateTime.Now.ToString("dddd");
                WeekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday);
                DaysLeftInYear = (DateTime.IsLeapYear(DateTime.Now.Year) ? 366 : 365) - DateTime.Now.DayOfYear;

                OnPropertyChanged(nameof(CurrentTime_Display));
                OnPropertyChanged(nameof(CurrentDate));
            };
            _timer.Start();
        }
    }
}
