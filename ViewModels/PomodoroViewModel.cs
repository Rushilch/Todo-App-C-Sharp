using System.Windows.Input;
using ProductivityApp.MVVM;
using ProductivityApp.Services;
using System.Windows.Threading;

namespace ProductivityApp.ViewModels
{
    public class PomodoroViewModel : BaseViewModel
    {
        private readonly IPomodoroService _pomodoroService;
        private readonly ISettingsService _settingsService;
        private DispatcherTimer? _timer;
        private int _secondsRemaining;
        private int _totalSeconds;
        private bool _isRunning;
        private bool _isFocusSession = true;
        private int _sessionsCompleted;
        private int _focusMinutes = 25;
        private int _breakMinutes = 5;
        private int _focusTimeMinutes;
        private int _breakTimeMinutes;

        public int SecondsRemaining
        {
            get => _secondsRemaining;
            set
            {
                if (SetProperty(ref _secondsRemaining, value))
                    OnPropertyChanged(nameof(TimerDisplay));
            }
        }

        public int TotalSeconds
        {
            get => _totalSeconds;
            set => SetProperty(ref _totalSeconds, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsFocusSession
        {
            get => _isFocusSession;
            set
            {
                if (SetProperty(ref _isFocusSession, value))
                    OnPropertyChanged(nameof(SessionStatus));
            }
        }

        public int SessionsCompleted
        {
            get => _sessionsCompleted;
            set => SetProperty(ref _sessionsCompleted, value);
        }

        public int FocusMinutes
        {
            get => _focusMinutes;
            set
            {
                if (SetProperty(ref _focusMinutes, value))
                    ResetTimer();
            }
        }

        public int BreakMinutes
        {
            get => _breakMinutes;
            set
            {
                if (SetProperty(ref _breakMinutes, value))
                    ResetTimer();
            }
        }

        public int FocusTimeMinutes
        {
            get => _focusTimeMinutes;
            set => SetProperty(ref _focusTimeMinutes, value);
        }

        public int BreakTimeMinutes
        {
            get => _breakTimeMinutes;
            set => SetProperty(ref _breakTimeMinutes, value);
        }

        public int FocusDuration
        {
            get => _focusMinutes;
            set
            {
                FocusMinutes = value;
                OnPropertyChanged();
            }
        }

        public int BreakDuration
        {
            get => _breakMinutes;
            set
            {
                BreakMinutes = value;
                OnPropertyChanged();
            }
        }

        public string TimerDisplay => $"{SecondsRemaining / 60:D2}:{SecondsRemaining % 60:D2}";
        public string SessionStatus => IsFocusSession ? "Focus Time" : "Break Time";

        public ICommand StartSessionCommand { get; }
        public ICommand PauseSessionCommand { get; }
        public ICommand ResetSessionCommand { get; }

        public PomodoroViewModel(IPomodoroService pomodoroService, ISettingsService settingsService)
        {
            _pomodoroService = pomodoroService;
            _settingsService = settingsService;

            // Initialize commands
            StartSessionCommand = new RelayCommand(_ => StartTimer());
            PauseSessionCommand = new RelayCommand(_ => PauseTimer());
            ResetSessionCommand = new RelayCommand(_ => ResetTimer());

            InitializeTimer();
            LoadSettingsAsync();
        }

        private async void LoadSettingsAsync()
        {
            try
            {
                var settings = await _settingsService.GetSettingsAsync();
                FocusMinutes = settings.PomodoroFocusMinutes;
                BreakMinutes = settings.PomodoroBreakMinutes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private void InitializeTimer()
        {
            ResetTimer();
        }

        public void StartTimer()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            _timer?.Start();
        }

        public void PauseTimer()
        {
            IsRunning = false;
            _timer?.Stop();
        }

        public void ResetTimer()
        {
            _timer?.Stop();
            IsRunning = false;
            TotalSeconds = (IsFocusSession ? FocusMinutes : BreakMinutes) * 60;
            SecondsRemaining = TotalSeconds;
            OnPropertyChanged(nameof(SessionStatus));
            
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            SecondsRemaining--;
            OnPropertyChanged(nameof(TimerDisplay));

            if (SecondsRemaining <= 0)
            {
                _timer?.Stop();
                IsRunning = false;
                CompleteSession();
            }
        }

        private async void CompleteSession()
        {
            if (IsFocusSession)
            {
                await _pomodoroService.StartSessionAsync();
                SessionsCompleted++;
                FocusTimeMinutes += FocusMinutes;
            }
            else
            {
                BreakTimeMinutes += BreakMinutes;
            }

            IsFocusSession = !IsFocusSession;
            ResetTimer();
        }
    }
}
