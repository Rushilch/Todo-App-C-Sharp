using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ProductivityApp.ViewModels;
using ProductivityApp.Views;

namespace ProductivityApp
{
    public partial class MainWindow : Window
    {
        private readonly MainViewViewModel _viewModel;
        private Random _random = new Random();
        private DispatcherTimer? _shootingStarTimer;

        public MainWindow(MainViewViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

       private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
            CreateStaticStars();
            CreateShootingStars();
        }

        private void CreateStaticStars()
        {
            // Create random static stars
            int canvasWidth = Math.Max((int)StarCanvas.ActualWidth, 1400);
            int canvasHeight = Math.Max((int)StarCanvas.ActualHeight, 800);

            for (int i = 0; i < 150; i++)
            {
                var star = new Ellipse
                {
                    Width = _random.Next(1, 4),
                    Height = _random.Next(1, 4),
                    Fill = new SolidColorBrush(Colors.White),
                    Opacity = _random.Next(30, 100) / 100.0
                };

                Canvas.SetLeft(star, _random.Next(0, canvasWidth));
                Canvas.SetTop(star, _random.Next(0, canvasHeight));
                StarCanvas.Children.Add(star);
            }
        }

        private void CreateShootingStars()
        {
            // Timer to create shooting stars periodically
            _shootingStarTimer = new DispatcherTimer();
            _shootingStarTimer.Interval = TimeSpan.FromSeconds(_random.Next(3, 8));
            _shootingStarTimer.Tick += (s, e) =>
            {
                CreateShootingStar();
                _shootingStarTimer.Interval = TimeSpan.FromSeconds(_random.Next(3, 8));
            };
            _shootingStarTimer.Start();
        }

        private void CreateShootingStar()
        {
            // Create a shooting star line
            var line = new Line
            {
                Stroke = new LinearGradientBrush(
                    new GradientStopCollection(new[] 
                    {
                        new GradientStop(Colors.Transparent, 0),
                        new GradientStop(Colors.Cyan, 0.5),
                        new GradientStop(Colors.White, 1)
                    })),
                StrokeThickness = 2,
                X1 = _random.Next(0, (int)StarCanvas.ActualWidth),
                Y1 = _random.Next(0, (int)StarCanvas.ActualHeight / 2)
            };

            StarCanvas.Children.Add(line);

            // Animate the shooting star
            var storyboard = new Storyboard();

            var x2Animation = new DoubleAnimation
            {
                From = line.X1,
                To = line.X1 - 300,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var y2Animation = new DoubleAnimation
            {
                From = line.Y1,
                To = line.Y1 + 200,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var opacityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1.5)
            };

            Storyboard.SetTarget(x2Animation, line);
            Storyboard.SetTargetProperty(x2Animation, new PropertyPath(Line.X2Property));
            
            Storyboard.SetTarget(y2Animation, line);
            Storyboard.SetTargetProperty(y2Animation, new PropertyPath(Line.Y2Property));
            
            Storyboard.SetTarget(opacityAnimation, line);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Line.OpacityProperty));

            storyboard.Children.Add(x2Animation);
            storyboard.Children.Add(y2Animation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Completed += (s, e) =>
            {
                StarCanvas.Children.Remove(line);
            };

            storyboard.Begin();
        }



        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn)
            {
                switch (btn.Name)
                {
                    case "BtnDashboard":
                        ShowDashboard();
                        break;
                    case "BtnTasks":
                        ShowTasks();
                        break;
                    case "BtnPomodoro":
                        ShowPomodoro();
                        break;
                    case "BtnCalendar":
                        ShowCalendar();
                        break;
                    case "BtnClock":
                        ShowClock();
                        break;
                    case "BtnAnalytics":
                        ShowAnalytics();
                        break;
                    case "BtnSettings":
                        ShowSettings();
                        break;
                }
                HighlightButton(btn.Name);
            }
        }

        private void ShowDashboard()
        {
            _viewModel.NavigateToPage("Dashboard");
            if (_viewModel.CurrentViewModel is DashboardViewModel dashboardViewModel)
                _ = dashboardViewModel.LoadDashboardAsync();

            ContentControl.Content = new DashboardView { DataContext = _viewModel.CurrentViewModel };
        }

        private void ShowTasks()
        {
            try
            {
                _viewModel.NavigateToPage("Tasks");
                ContentControl.Content = new TasksView { DataContext = _viewModel.CurrentViewModel };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open tasks: {ex.Message}", "Tasks Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPomodoro()
        {
            _viewModel.NavigateToPage("Pomodoro");
            ContentControl.Content = new PomodoroView { DataContext = _viewModel.CurrentViewModel };
        }

        private void ShowCalendar()
        {
            _viewModel.NavigateToPage("Calendar");
            ContentControl.Content = new CalendarView { DataContext = _viewModel.CurrentViewModel };
        }

        private void ShowClock()
        {
            _viewModel.NavigateToPage("Clock");
            ContentControl.Content = new ClockView { DataContext = _viewModel.CurrentViewModel };
        }

        private void ShowAnalytics()
        {
            _viewModel.NavigateToPage("Analytics");
            if (_viewModel.CurrentViewModel is AnalyticsViewModel analyticsViewModel)
                _ = analyticsViewModel.LoadAnalyticsAsync();

            ContentControl.Content = new AnalyticsView { DataContext = _viewModel.CurrentViewModel };
        }

        private void ShowSettings()
        {
            try
            {
                _viewModel.NavigateToPage("Settings");
                ContentControl.Content = new SettingsView { DataContext = _viewModel.CurrentViewModel };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open settings: {ex.Message}", "Settings Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HighlightButton(string buttonName)
        {
            // Buttons now handle their own glow effect via the style
            // This method can be extended for additional visual feedback if needed
        }
    }
}
