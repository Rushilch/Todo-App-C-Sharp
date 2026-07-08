using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface ISettingsService
    {
        Task<AppSettings> GetSettingsAsync();
        Task<AppSettings> UpdateSettingsAsync(AppSettings settings);
    }

    public class SettingsService : ISettingsService
    {
        private readonly ProductivityDbContext _dbContext;

        public SettingsService(ProductivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppSettings> GetSettingsAsync()
        {
            var settings = await _dbContext.AppSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new AppSettings();
                _dbContext.AppSettings.Add(settings);
                await _dbContext.SaveChangesAsync();
            }
            return settings;
        }

        public async Task<AppSettings> UpdateSettingsAsync(AppSettings settings)
        {
            _dbContext.AppSettings.Update(settings);
            await _dbContext.SaveChangesAsync();
            return settings;
        }
    }

    public interface IAnalyticsService
    {
        Task<DailyStats?> GetDailyStatsAsync(DateTime date);
        Task<DailyStats> UpdateDailyStatsAsync(DateTime date);
        Task<List<DailyStats>> GetWeeklyStatsAsync();
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ProductivityDbContext _dbContext;
        private readonly IPomodoroService _pomodoroService;
        private readonly ITaskService _taskService;

        public AnalyticsService(ProductivityDbContext dbContext, IPomodoroService pomodoroService, ITaskService taskService)
        {
            _dbContext = dbContext;
            _pomodoroService = pomodoroService;
            _taskService = taskService;
        }

        public async Task<DailyStats?> GetDailyStatsAsync(DateTime date)
        {
            var targetDate = date.Date;
            return await _dbContext.DailyStats.FirstOrDefaultAsync(s => s.Date == targetDate);
        }

        public async Task<DailyStats> UpdateDailyStatsAsync(DateTime date)
        {
            var targetDate = date.Date;
            var stats = await GetDailyStatsAsync(targetDate);

            if (stats == null)
            {
                stats = new DailyStats { Date = targetDate };
                _dbContext.DailyStats.Add(stats);
            }

            // Calculate current stats
            var tasksCompleted = await _dbContext.Tasks
                .Where(t => t.IsCompleted && t.CompletedAt.HasValue && t.CompletedAt.Value.Date == targetDate)
                .CountAsync();

            var pomodoroSessions = await _dbContext.PomodoroSessions
                .Where(p => p.StartTime.Date == targetDate && p.IsCompleted)
                .ToListAsync();

            stats.TasksCompleted = tasksCompleted;
            stats.PomodorosCompleted = pomodoroSessions.Count;
            stats.TotalFocusTimeMinutes = pomodoroSessions.Sum(p => p.Duration);

            _dbContext.DailyStats.Update(stats);
            await _dbContext.SaveChangesAsync();
            return stats;
        }

        public async Task<List<DailyStats>> GetWeeklyStatsAsync()
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-6);

            return await _dbContext.DailyStats
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .OrderBy(s => s.Date)
                .ToListAsync();
        }
    }
}
