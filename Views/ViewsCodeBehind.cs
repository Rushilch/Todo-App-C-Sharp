using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ProductivityApp.Data.Models;
using ProductivityApp.ViewModels;

namespace ProductivityApp.Views
{
    public partial class DashboardView : UserControl 
    {
        public DashboardView()
        {
            InitializeComponent();
        }
    }
    public partial class TasksView : UserControl 
    {
        private Point _dragStartPoint;

        public TasksView()
        {
            InitializeComponent();
        }

        private void TaskCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void TaskCard_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed ||
                sender is not FrameworkElement { DataContext: TaskItem task } ||
                IsInteractiveControl(e.OriginalSource as DependencyObject))
            {
                return;
            }

            var currentPosition = e.GetPosition(null);
            if (Math.Abs(currentPosition.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(currentPosition.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            DragDrop.DoDragDrop((DependencyObject)sender, task, DragDropEffects.Move);
        }

        private static bool IsInteractiveControl(DependencyObject? source)
        {
            while (source != null)
            {
                if (source is ButtonBase || source is TextBox || source is ComboBox)
                    return true;

                source = VisualTreeHelper.GetParent(source);
            }

            return false;
        }

        private void TaskColumn_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(typeof(TaskItem))
                ? DragDropEffects.Move
                : DragDropEffects.None;
            e.Handled = true;
        }

        private void TaskColumn_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is not TasksViewModel viewModel ||
                sender is not FrameworkElement { Tag: string targetColumn } ||
                e.Data.GetData(typeof(TaskItem)) is not TaskItem task)
            {
                return;
            }

            _ = viewModel.MoveTaskAsync(task, targetColumn);
            e.Handled = true;
        }
    }
    public partial class PomodoroView : UserControl 
    {
        public PomodoroView()
        {
            InitializeComponent();
        }
    }
    public partial class CalendarView : UserControl 
    {
        public CalendarView()
        {
            InitializeComponent();
        }

        private async void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ViewModels.CalendarViewModel vm)
                return;

            var dialog = new AddEventWindow { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true && dialog.ResultEvent != null)
            {
                var ev = dialog.ResultEvent;
                // Set date to currently selected date
                ev.EventDate = vm.SelectedDate.Date;
                ev.CreatedAt = System.DateTime.Now;

                await vm.AddEventAsync(ev);
            }
        }
    }
    public partial class ClockView : UserControl 
    {
        public ClockView()
        {
            InitializeComponent();
        }
    }
    public partial class AnalyticsView : UserControl
    {
        public AnalyticsView()
        {
            InitializeComponent();
        }
    }
    public partial class SettingsView : UserControl 
    {
        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
