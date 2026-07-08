using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface IPomodoroService
    {
        Task<PomodoroSession> StartSessionAsync(string? taskName = null);
        Task<PomodoroSession> EndSessionAsync(int sessionId);
        Task<List<PomodoroSession>> GetTodaysSessionsAsync();
        Task<int> GetTodaysCompletedCountAsync();
        Task<int> GetTotalFocusTimeAsync(DateTime? date = null);
    }

    public class PomodoroService : IPomodoroService
    {
        private readonly ProductivityDbContext _dbContext;

        public PomodoroService(ProductivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PomodoroSession> StartSessionAsync(string? taskName = null)
        {
            var settings = await _dbContext.AppSettings.FirstOrDefaultAsync();
            var duration = settings?.PomodoroFocusMinutes ?? 25;

            var session = new PomodoroSession
            {
                StartTime = DateTime.Now,
                Duration = duration,
                IsCompleted = false,
                SessionNumber = await GetTodaysSessionCountAsync() + 1,
                TaskName = taskName
            };

            _dbContext.PomodoroSessions.Add(session);
            await _dbContext.SaveChangesAsync();
            return session;
        }

        public async Task<PomodoroSession> EndSessionAsync(int sessionId)
        {
            var session = await _dbContext.PomodoroSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.EndTime = DateTime.Now;
                session.IsCompleted = true;
                _dbContext.PomodoroSessions.Update(session);
                await _dbContext.SaveChangesAsync();
            }

            return session ?? new PomodoroSession();
        }

        public async Task<List<PomodoroSession>> GetTodaysSessionsAsync()
        {
            var today = DateTime.Today;
            return await _dbContext.PomodoroSessions
                .Where(s => s.StartTime.Date == today)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<int> GetTodaysCompletedCountAsync()
        {
            var today = DateTime.Today;
            return await _dbContext.PomodoroSessions
                .Where(s => s.StartTime.Date == today && s.IsCompleted)
                .CountAsync();
        }

        public async Task<int> GetTotalFocusTimeAsync(DateTime? date = null)
        {
            var targetDate = (date ?? DateTime.Today).Date;
            var sessions = await _dbContext.PomodoroSessions
                .Where(s => s.StartTime.Date == targetDate && s.IsCompleted)
                .ToListAsync();

            return sessions.Sum(s => s.Duration);
        }

        private async Task<int> GetTodaysSessionCountAsync()
        {
            var today = DateTime.Today;
            return await _dbContext.PomodoroSessions
                .Where(s => s.StartTime.Date == today)
                .CountAsync();
        }
    }
}
