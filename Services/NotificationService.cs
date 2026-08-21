using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProductivityApp.Services
{
    public interface INotificationService
    {
        Task ShowNotificationAsync(string title, string message);
    }

    // Simple WPF-based toast window notification (no external dependencies)
    public class NotificationService : INotificationService
    {
        public Task ShowNotificationAsync(string title, string message)
        {
            try
            {
                Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    var toast = new Window
                    {
                        Width = 320,
                        Height = 80,
                        WindowStyle = WindowStyle.None,
                        AllowsTransparency = true,
                        Background = new SolidColorBrush(Color.FromArgb(230, 44, 62, 80)),
                        Foreground = Brushes.White,
                        Topmost = true,
                        ShowInTaskbar = false,
                        ResizeMode = ResizeMode.NoResize,
                        Content = new Border
                        {
                            Padding = new Thickness(12),
                            Child = new StackPanel
                            {
                                Children =
                                {
                                    new TextBlock { Text = title, FontWeight = FontWeights.Bold, FontSize = 14, Margin = new Thickness(0,0,0,4) },
                                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap }
                                }
                            }
                        }
                    };

                    // Position top-right of primary screen working area
                    var workArea = SystemParameters.WorkArea;
                    toast.Left = workArea.Right - toast.Width - 16;
                    toast.Top = workArea.Top + 16;

                    toast.Loaded += (s, e) =>
                    {
                        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                        timer.Tick += (ts, te) =>
                        {
                            timer.Stop();
                            toast.Close();
                        };
                        timer.Start();
                    };

                    toast.Show();
                }));
            }
            catch
            {
                // Swallow errors - notifications are non-critical
            }

            return Task.CompletedTask;
        }
    }
}
