using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<List<TaskItem>> GetTasksByCategoryAsync(TaskCategory category);
        Task<List<TaskItem>> SearchTasksAsync(string searchTerm);
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> AddTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id);
        Task<List<TaskItem>> GetUpcomingTasksAsync(int days = 7);
        Task<int> GetTodaysCompletedCountAsync();
    }

    public class TaskService : ITaskService
    {
        private readonly ProductivityDbContext _dbContext;
        private readonly INotificationService _notificationService;

        public TaskService(ProductivityDbContext dbContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _dbContext.Tasks
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetTasksByCategoryAsync(TaskCategory category)
        {
            return await _dbContext.Tasks
                .Where(t => t.Category == category && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskItem>> SearchTasksAsync(string searchTerm)
        {
            return await _dbContext.Tasks
                .Where(t => t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _dbContext.Tasks.FindAsync(id);
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.Now;
            task.LastModified = DateTime.Now;
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // If task is due soon (within 1 hour), show a notification
            try
            {
                if (task.DueDate != default && task.DueDate <= DateTime.Now.AddHours(1) && !task.IsCompleted)
                {
                                    await _notificationService.ShowNotificationAsync("Task Due Soon", $"{task.Title} is due at {task.DueDate.ToShortTimeString()}");
                }
            }
            catch
            {
                // Ignore notification errors
            }

            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            task.LastModified = DateTime.Now;
            if (task.IsCompleted && task.CompletedAt == null)
                task.CompletedAt = DateTime.Now;

            _dbContext.Tasks.Update(task);
            await _dbContext.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _dbContext.Tasks.FindAsync(id);
            if (task != null)
            {
                _dbContext.Tasks.Remove(task);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<TaskItem>> GetUpcomingTasksAsync(int days = 7)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(days);

            return await _dbContext.Tasks
                .Where(t => !t.IsCompleted && t.DueDate >= startDate && t.DueDate <= endDate)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<int> GetTodaysCompletedCountAsync()
        {
            var today = DateTime.Today;
            return await _dbContext.Tasks
                .Where(t => t.IsCompleted && t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today)
                .CountAsync();
        }
    }
}
