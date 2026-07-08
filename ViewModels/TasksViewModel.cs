using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ProductivityApp.Data.Models;
using ProductivityApp.MVVM;
using ProductivityApp.Services;

namespace ProductivityApp.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;
        private ObservableCollection<TaskItem> _tasks = new();
        private ObservableCollection<TaskItem> _highPriorityTasks = new();
        private ObservableCollection<TaskItem> _mediumPriorityTasks = new();
        private ObservableCollection<TaskItem> _lowPriorityTasks = new();
        private ObservableCollection<TaskItem> _doneTasks = new();
        private ObservableCollection<string> _categories = new();
        private ObservableCollection<string> _priorities = new();
        private TaskCategory _selectedCategory = TaskCategory.Work;
        private string _selectedCategoryName = "Work";
        private TaskPriority _selectedPriority = TaskPriority.Medium;
        private string _selectedPriorityName = "Medium";
        private string _searchText = string.Empty;
        private string _newTaskTitle = string.Empty;
        private TaskItem? _selectedTask;
        private int _totalTasks;
        private int _completedTasks;
        private double _completionRate;

        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        public ObservableCollection<TaskItem> HighPriorityTasks
        {
            get => _highPriorityTasks;
            set => SetProperty(ref _highPriorityTasks, value);
        }

        public ObservableCollection<TaskItem> MediumPriorityTasks
        {
            get => _mediumPriorityTasks;
            set => SetProperty(ref _mediumPriorityTasks, value);
        }

        public ObservableCollection<TaskItem> LowPriorityTasks
        {
            get => _lowPriorityTasks;
            set => SetProperty(ref _lowPriorityTasks, value);
        }

        public ObservableCollection<TaskItem> DoneTasks
        {
            get => _doneTasks;
            set => SetProperty(ref _doneTasks, value);
        }

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<string> Priorities
        {
            get => _priorities;
            set => SetProperty(ref _priorities, value);
        }

        public TaskCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                    _ = LoadTasksAsync();
            }
        }

        public string SelectedCategoryName
        {
            get => _selectedCategoryName;
            set
            {
                if (SetProperty(ref _selectedCategoryName, value))
                {
                    if (Enum.TryParse<TaskCategory>(value, out var category))
                    {
                        SelectedCategory = category;
                    }
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    _ = SearchTasksAsync();
            }
        }

        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set => SetProperty(ref _newTaskTitle, value);
        }

        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public int TotalTasks
        {
            get => _totalTasks;
            set => SetProperty(ref _totalTasks, value);
        }

        public int CompletedTasks
        {
            get => _completedTasks;
            set => SetProperty(ref _completedTasks, value);
        }

        public double CompletionRate
        {
            get => _completionRate;
            set => SetProperty(ref _completionRate, value);
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand ToggleTaskCompletedCommand { get; }

        public TasksViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            // Initialize categories
            Categories.Add("Work");
            Categories.Add("Personal");
            Categories.Add("Study");
            Categories.Add("Health");
            Categories.Add("Other");

            Priorities.Add("High");
            Priorities.Add("Medium");
            Priorities.Add("Low");

            // Initialize commands
            AddTaskCommand = new RelayCommand(_ => AddNewTask());
            DeleteTaskCommand = new RelayCommand<TaskItem>(task => _ = DeleteTaskAsync(task));
            ToggleTaskCompletedCommand = new RelayCommand<TaskItem>(task => _ = ToggleTaskCompletedAsync(task));

            _ = LoadTasksAsync();
        }

        private void AddNewTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
                return;

            var newTask = new TaskItem
            {
                Title = NewTaskTitle,
                Description = "",
                Category = SelectedCategory,
                Priority = SelectedPriority,
                DueDate = DateTime.Now.AddDays(1),
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };
            _ = AddTaskAsync(newTask);
            NewTaskTitle = string.Empty;
        }

        public async Task LoadTasksAsync()
        {
            try
            {
                var tasks = (await _taskService.GetAllTasksAsync())
                    .Where(t => t.Category == SelectedCategory)
                    .OrderBy(t => t.IsCompleted)
                    .ThenByDescending(t => t.Priority)
                    .ThenBy(t => t.DueDate)
                    .ToList();
                Tasks.Clear();
                foreach (var task in tasks)
                    Tasks.Add(task);

                UpdateBoardColumns();
                UpdateStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
                MessageBox.Show($"Could not load tasks: {ex.Message}", "Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task SearchTasksAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadTasksAsync();
                    return;
                }

                var tasks = await _taskService.SearchTasksAsync(SearchText);
                Tasks.Clear();
                foreach (var task in tasks)
                    Tasks.Add(task);

                UpdateBoardColumns();
                UpdateStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching tasks: {ex.Message}");
                MessageBox.Show($"Could not search tasks: {ex.Message}", "Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            try
            {
                var newTask = await _taskService.AddTaskAsync(task);
                Tasks.Add(newTask);
                UpdateBoardColumns();
                UpdateStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding task: {ex.Message}");
                MessageBox.Show($"Could not add task: {ex.Message}", "Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            try
            {
                await _taskService.UpdateTaskAsync(task);
                int index = Tasks.IndexOf(task);
                if (index >= 0)
                {
                    Tasks[index] = task;
                }
                UpdateBoardColumns();
                UpdateStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating task: {ex.Message}");
                MessageBox.Show($"Could not update task: {ex.Message}", "Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task DeleteTaskAsync(TaskItem? task)
        {
            if (task == null) return;
            try
            {
                await _taskService.DeleteTaskAsync(task.Id);
                Tasks.Remove(task);
                UpdateBoardColumns();
                UpdateStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting task: {ex.Message}");
                MessageBox.Show($"Could not delete task: {ex.Message}", "Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task MoveTaskAsync(TaskItem? task, string? targetColumn)
        {
            if (task == null || string.IsNullOrWhiteSpace(targetColumn))
                return;

            task.IsCompleted = targetColumn == "Done";
            task.CompletedAt = task.IsCompleted ? DateTime.Now : null;

            if (!task.IsCompleted && Enum.TryParse<TaskPriority>(targetColumn, out var priority))
                task.Priority = priority;

            await UpdateTaskAsync(task);
        }

        private void UpdateStats()
        {
            TotalTasks = Tasks.Count;
            CompletedTasks = Tasks.Count(t => t.IsCompleted);
            CompletionRate = TotalTasks > 0 ? (double)CompletedTasks / TotalTasks : 0;
        }

        private async Task ToggleTaskCompletedAsync(TaskItem? task)
        {
            if (task == null)
                return;

            await UpdateTaskAsync(task);
            await LoadTasksAsync();
        }

        private void UpdateBoardColumns()
        {
            HighPriorityTasks.Clear();
            MediumPriorityTasks.Clear();
            LowPriorityTasks.Clear();
            DoneTasks.Clear();

            foreach (var task in Tasks.OrderBy(t => t.DueDate))
            {
                if (task.IsCompleted)
                {
                    DoneTasks.Add(task);
                    continue;
                }

                switch (task.Priority)
                {
                    case TaskPriority.High:
                        HighPriorityTasks.Add(task);
                        break;
                    case TaskPriority.Medium:
                        MediumPriorityTasks.Add(task);
                        break;
                    default:
                        LowPriorityTasks.Add(task);
                        break;
                }
            }
        }

        public TaskPriority SelectedPriority
        {
            get => _selectedPriority;
            set => SetProperty(ref _selectedPriority, value);
        }

        public string SelectedPriorityName
        {
            get => _selectedPriorityName;
            set
            {
                if (SetProperty(ref _selectedPriorityName, value) &&
                    Enum.TryParse<TaskPriority>(value, out var priority))
                {
                    SelectedPriority = priority;
                }
            }
        }
    }
}
