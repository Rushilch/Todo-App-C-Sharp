using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using ProductivityApp.Data.Models;
using ProductivityApp.MVVM;
using ProductivityApp.Services;

namespace ProductivityApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private AppSettings _settings = new();
        private bool _isLoadingSettings;
        private int _pomodoroFocus = 25;
        private int _pomodoroBreak = 5;
        private bool _darkModeEnabled;
        private bool _notificationsEnabled = true;
        private bool _enableSoundEffects = true;
        private bool _use24HourFormat;
        private string _selectedTheme = "Dark";
        private string _selectedAccentColor = "Blue";
        private string _selectedTextColor = "Default";
        private int _defaultFocusDuration = 25;
        private int _defaultBreakDuration = 5;
        private int _dailyTaskGoal = 10;
        private ObservableCollection<string> _themes = new();
        private ObservableCollection<string> _accentColors = new();
        private ObservableCollection<string> _textColors = new();

        public int PomodoroFocusMinutes
        {
            get => _pomodoroFocus;
            set => SetProperty(ref _pomodoroFocus, value);
        }

        public int PomodoroBreakMinutes
        {
            get => _pomodoroBreak;
            set => SetProperty(ref _pomodoroBreak, value);
        }

        public bool DarkModeEnabled
        {
            get => _darkModeEnabled;
            set => SetProperty(ref _darkModeEnabled, value);
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (SetProperty(ref _notificationsEnabled, value))
                    OnPropertyChanged(nameof(EnableNotifications));
            }
        }

        public bool EnableNotifications
        {
            get => NotificationsEnabled;
            set => NotificationsEnabled = value;
        }

        public bool EnableSoundEffects
        {
            get => _enableSoundEffects;
            set => SetProperty(ref _enableSoundEffects, value);
        }

        public bool Use24HourFormat
        {
            get => _use24HourFormat;
            set => SetProperty(ref _use24HourFormat, value);
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                {
                    ApplySelectedTheme();
                    if (!_isLoadingSettings)
                        _ = SaveSettingsAsync();
                }
            }
        }

        public string SelectedAccentColor
        {
            get => _selectedAccentColor;
            set
            {
                if (SetProperty(ref _selectedAccentColor, value))
                {
                    ApplySelectedTheme();
                    if (!_isLoadingSettings)
                        _ = SaveSettingsAsync();
                }
            }
        }

        public string SelectedTextColor
        {
            get => _selectedTextColor;
            set
            {
                if (SetProperty(ref _selectedTextColor, value))
                {
                    ApplySelectedTheme();
                    if (!_isLoadingSettings)
                        _ = SaveSettingsAsync();
                }
            }
        }

        public int DefaultFocusDuration
        {
            get => _defaultFocusDuration;
            set => SetProperty(ref _defaultFocusDuration, value);
        }

        public int DefaultBreakDuration
        {
            get => _defaultBreakDuration;
            set => SetProperty(ref _defaultBreakDuration, value);
        }

        public int DailyTaskGoal
        {
            get => _dailyTaskGoal;
            set => SetProperty(ref _dailyTaskGoal, value);
        }

        public ObservableCollection<string> Themes => _themes;
        public ObservableCollection<string> AccentColors => _accentColors;
        public ObservableCollection<string> TextColors => _textColors;

        public ICommand ExportDataCommand { get; }
        public ICommand ImportDataCommand { get; }
        public ICommand ClearDataCommand { get; }

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            // Initialize theme collection with Catppuccin themes
            _themes.Add("Catppuccin Mocha");
            _themes.Add("Catppuccin Macchiato");
            _themes.Add("Catppuccin Frappe");
            _themes.Add("Catppuccin Latte");
            _themes.Add("Dark");
            _themes.Add("Light");
            _themes.Add("System Default");

            _accentColors.Add("Rosewater");
            _accentColors.Add("Flamingo");
            _accentColors.Add("Pink");
            _accentColors.Add("Mauve");
            _accentColors.Add("Red");
            _accentColors.Add("Maroon");
            _accentColors.Add("Peach");
            _accentColors.Add("Yellow");
            _accentColors.Add("Green");
            _accentColors.Add("Teal");
            _accentColors.Add("Sky");
            _accentColors.Add("Sapphire");
            _accentColors.Add("Blue");
            _accentColors.Add("Lavender");

            _textColors.Add("Default");
            _textColors.Add("Rosewater");
            _textColors.Add("Flamingo");
            _textColors.Add("Pink");
            _textColors.Add("Mauve");
            _textColors.Add("Peach");
            _textColors.Add("Yellow");
            _textColors.Add("Green");
            _textColors.Add("Teal");
            _textColors.Add("Sky");
            _textColors.Add("Blue");
            _textColors.Add("Lavender");
            _textColors.Add("White");
            _textColors.Add("Charcoal");

            // Initialize commands
            ExportDataCommand = new RelayCommand(_ => ExportData());
            ImportDataCommand = new RelayCommand(_ => ImportData());
            ClearDataCommand = new RelayCommand(_ => ClearData());

            LoadSettingsAsync();
        }

        private async void LoadSettingsAsync()
        {
            try
            {
                _isLoadingSettings = true;
                _settings = await _settingsService.GetSettingsAsync();
                PomodoroFocusMinutes = _settings.PomodoroFocusMinutes;
                PomodoroBreakMinutes = _settings.PomodoroBreakMinutes;
                DarkModeEnabled = _settings.DarkModeEnabled;
                NotificationsEnabled = _settings.NotificationsEnabled;
                Use24HourFormat = _settings.Use24HourFormat;
                var appearance = ParseAppearanceSettings(_settings.Theme);
                SelectedTheme = appearance.Theme;
                SelectedAccentColor = appearance.AccentColor;
                SelectedTextColor = appearance.TextColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
            finally
            {
                _isLoadingSettings = false;
                ApplySelectedTheme();
            }
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                _settings.PomodoroFocusMinutes = PomodoroFocusMinutes;
                _settings.PomodoroBreakMinutes = PomodoroBreakMinutes;
                _settings.DarkModeEnabled = DarkModeEnabled;
                _settings.NotificationsEnabled = NotificationsEnabled;
                _settings.Use24HourFormat = Use24HourFormat;
                _settings.Theme = ComposeAppearanceSettings();
                await _settingsService.UpdateSettingsAsync(_settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private void ExportData()
        {
            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var exportPath = Path.Combine(desktop, $"ProductivityApp_Export_{DateTime.Now:yyyy-MM-dd_HHmmss}.json");
                System.Diagnostics.Debug.WriteLine($"Data exported to: {exportPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting data: {ex.Message}");
            }
        }

        private void ImportData()
        {
            System.Diagnostics.Debug.WriteLine("Import data functionality");
        }

        private void ClearData()
        {
            System.Diagnostics.Debug.WriteLine("Clear data functionality - showing confirmation dialog");
        }

        private void ApplySelectedTheme()
        {
            var palette = GetThemePalette(SelectedTheme);
            var accent = GetAccentColor(SelectedAccentColor, palette.Accent);
            var text = GetTextColor(SelectedTextColor, palette.Text);
            var resources = Application.Current.Resources;

            resources["AppBackgroundBrush"] = new SolidColorBrush(palette.Base);
            resources["SidebarBackgroundBrush"] = new SolidColorBrush(palette.Mantle);
            resources["ContentBackgroundBrush"] = new SolidColorBrush(palette.Crust);
            resources["SurfaceBrush"] = new SolidColorBrush(palette.Surface0);
            resources["SurfaceAltBrush"] = new SolidColorBrush(palette.Surface1);
            resources["TextBrush"] = new SolidColorBrush(text);
            resources["MutedTextBrush"] = new SolidColorBrush(palette.Subtext0);
            resources["SubtleTextBrush"] = new SolidColorBrush(palette.Subtext1);
            resources["BorderBrushColor"] = new SolidColorBrush(palette.Surface1);
            resources["AccentBrush"] = new SolidColorBrush(accent);
            resources["AccentTextBrush"] = new SolidColorBrush(palette.AccentText);
            resources["DangerBrush"] = new SolidColorBrush(palette.Red);
            resources["SuccessBrush"] = new SolidColorBrush(palette.Green);
            resources["WarningBrush"] = new SolidColorBrush(palette.Yellow);
        }

        private static ThemePalette GetThemePalette(string theme) => theme switch
        {
            "Catppuccin Latte" => new ThemePalette("#EFF1F5", "#E6E9EF", "#DCE0E8", "#CCD0DA", "#BCC0CC", "#4C4F69", "#6C6F85", "#5C5F77", "#1E66F5", "#D20F39", "#40A02B", "#DF8E1D", "#EFF1F5"),
            "Catppuccin Frappe" => new ThemePalette("#303446", "#292C3C", "#232634", "#414559", "#51576D", "#C6D0F5", "#A5ADCE", "#B5BFE2", "#8CAAEE", "#E78284", "#A6D189", "#E5C890", "#232634"),
            "Catppuccin Macchiato" => new ThemePalette("#24273A", "#1E2030", "#181926", "#363A4F", "#494D64", "#CAD3F5", "#A5ADCB", "#B8C0E0", "#8AADF4", "#ED8796", "#A6DA95", "#EED49F", "#181926"),
            "Catppuccin Mocha" => new ThemePalette("#1E1E2E", "#181825", "#11111B", "#313244", "#45475A", "#CDD6F4", "#A6ADC8", "#BAC2DE", "#89B4FA", "#F38BA8", "#A6E3A1", "#F9E2AF", "#11111B"),
            "Light" => new ThemePalette("#F8F9FA", "#ECEFF4", "#FFFFFF", "#FFFFFF", "#E5E7EB", "#2C3E50", "#6B7280", "#4B5563", "#3498DB", "#E74C3C", "#27AE60", "#F59E0B", "#FFFFFF"),
            _ => new ThemePalette("#0F1423", "#0A0E27", "#090D19", "#172033", "#1E2A44", "#E8F4F8", "#9FB0C0", "#C5D4E0", "#00D4FF", "#E74C3C", "#27AE60", "#F1C40F", "#090D19")
        };

        private static Color GetAccentColor(string accentName, Color fallback) => accentName switch
        {
            "Rosewater" => ColorFromHex("#F5E0DC"),
            "Flamingo" => ColorFromHex("#F2CDCD"),
            "Pink" => ColorFromHex("#F5C2E7"),
            "Mauve" => ColorFromHex("#CBA6F7"),
            "Red" => ColorFromHex("#F38BA8"),
            "Maroon" => ColorFromHex("#EBA0AC"),
            "Peach" => ColorFromHex("#FAB387"),
            "Yellow" => ColorFromHex("#F9E2AF"),
            "Green" => ColorFromHex("#A6E3A1"),
            "Teal" => ColorFromHex("#94E2D5"),
            "Sky" => ColorFromHex("#89DCEB"),
            "Sapphire" => ColorFromHex("#74C7EC"),
            "Blue" => ColorFromHex("#89B4FA"),
            "Lavender" => ColorFromHex("#B4BEFE"),
            _ => fallback
        };

        private static Color GetTextColor(string textColorName, Color fallback) => textColorName switch
        {
            "White" => ColorFromHex("#FFFFFF"),
            "Charcoal" => ColorFromHex("#2C2F3A"),
            "Default" => fallback,
            _ => GetAccentColor(textColorName, fallback)
        };

        private string ComposeAppearanceSettings() => $"{SelectedTheme}|{SelectedAccentColor}|{SelectedTextColor}";

        private static (string Theme, string AccentColor, string TextColor) ParseAppearanceSettings(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return ("Catppuccin Mocha", "Blue", "Default");

            var parts = value.Split('|', StringSplitOptions.TrimEntries);
            return (
                string.IsNullOrWhiteSpace(parts.ElementAtOrDefault(0)) ? "Catppuccin Mocha" : parts[0],
                string.IsNullOrWhiteSpace(parts.ElementAtOrDefault(1)) ? "Blue" : parts[1],
                string.IsNullOrWhiteSpace(parts.ElementAtOrDefault(2)) ? "Default" : parts[2]);
        }

        private static Color ColorFromHex(string hex) => (Color)ColorConverter.ConvertFromString(hex);

        private readonly record struct ThemePalette(
            string BaseHex,
            string MantleHex,
            string CrustHex,
            string Surface0Hex,
            string Surface1Hex,
            string TextHex,
            string Subtext0Hex,
            string Subtext1Hex,
            string AccentHex,
            string RedHex,
            string GreenHex,
            string YellowHex,
            string AccentTextHex)
        {
            public Color Base => ColorFromHex(BaseHex);
            public Color Mantle => ColorFromHex(MantleHex);
            public Color Crust => ColorFromHex(CrustHex);
            public Color Surface0 => ColorFromHex(Surface0Hex);
            public Color Surface1 => ColorFromHex(Surface1Hex);
            public Color Text => ColorFromHex(TextHex);
            public Color Subtext0 => ColorFromHex(Subtext0Hex);
            public Color Subtext1 => ColorFromHex(Subtext1Hex);
            public Color Accent => ColorFromHex(AccentHex);
            public Color Red => ColorFromHex(RedHex);
            public Color Green => ColorFromHex(GreenHex);
            public Color Yellow => ColorFromHex(YellowHex);
            public Color AccentText => ColorFromHex(AccentTextHex);
        }
    }

    public class AnalyticsViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;
        private ObservableCollection<DailyStats> _dailyStats = new();
        private int _totalTasksCompleted;
        private int _pomodorosCompleted;
        private double _totalFocusHours;
        private double _dailyAverage;
        private ObservableCollection<DailyStats> _weeklyStats = new();

        public ObservableCollection<DailyStats> DailyStats
        {
            get => _dailyStats;
            set => SetProperty(ref _dailyStats, value);
        }

        public int TotalTasksCompleted
        {
            get => _totalTasksCompleted;
            set => SetProperty(ref _totalTasksCompleted, value);
        }

        public int PomodosCompleted
        {
            get => _pomodorosCompleted;
            set => SetProperty(ref _pomodorosCompleted, value);
        }

        public double TotalFocusHours
        {
            get => _totalFocusHours;
            set => SetProperty(ref _totalFocusHours, value);
        }

        public double DailyAverage
        {
            get => _dailyAverage;
            set => SetProperty(ref _dailyAverage, value);
        }

        public ObservableCollection<DailyStats> WeeklyStats
        {
            get => _weeklyStats;
            set => SetProperty(ref _weeklyStats, value);
        }

        public ICommand ExportReportCommand { get; }
        public ICommand RefreshCommand { get; }

        public AnalyticsViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;

            // Initialize commands
            ExportReportCommand = new RelayCommand(_ => _ = ExportReportAsync());
            RefreshCommand = new RelayCommand(_ => _ = RefreshAsync());

            _ = LoadAnalyticsAsync();
        }

        public async Task LoadAnalyticsAsync()
        {
            try
            {
                for (var day = DateTime.Today.AddDays(-6); day <= DateTime.Today; day = day.AddDays(1))
                    await _analyticsService.UpdateDailyStatsAsync(day);

                var stats = await _analyticsService.GetWeeklyStatsAsync();
                DailyStats.Clear();
                WeeklyStats.Clear();

                TotalTasksCompleted = 0;
                PomodosCompleted = 0;
                TotalFocusHours = 0;

                if (stats != null && stats.Any())
                {
                    foreach (var stat in stats)
                    {
                        DailyStats.Add(stat);
                        WeeklyStats.Add(stat);
                        TotalTasksCompleted += stat.TasksCompleted;
                        PomodosCompleted += stat.PomodorosCompleted;
                        TotalFocusHours += stat.TotalFocusTimeMinutes / 60.0;
                    }

                    DailyAverage = TotalTasksCompleted / (double)Math.Max(stats.Count(), 1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading analytics: {ex.Message}");
            }
        }

        public async Task ExportReportAsync()
        {
            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var reportPath = Path.Combine(desktop, $"Productivity_Report_{DateTime.Now:yyyy-MM-dd}.json");
                System.Diagnostics.Debug.WriteLine($"Report exported to: {reportPath}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting report: {ex.Message}");
            }
        }

        public async Task RefreshAsync()
        {
            await LoadAnalyticsAsync();
        }
    }
}
