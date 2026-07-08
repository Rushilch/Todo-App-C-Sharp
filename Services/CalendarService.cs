using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface ICalendarService
    {
        Task<List<CalendarEvent>> GetEventsForMonthAsync(DateTime month);
        Task<List<CalendarEvent>> GetEventsForDayAsync(DateTime day);
        Task<CalendarEvent> AddEventAsync(CalendarEvent @event);
        Task<CalendarEvent> UpdateEventAsync(CalendarEvent @event);
        Task DeleteEventAsync(int id);
        Task<List<CalendarEvent>> GetUpcomingEventsAsync(int days = 7);
    }

    public class CalendarService : ICalendarService
    {
        private readonly ProductivityDbContext _dbContext;

        public CalendarService(ProductivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CalendarEvent>> GetEventsForMonthAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _dbContext.CalendarEvents
                .Where(e => e.EventDate >= startDate && e.EventDate <= endDate)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetEventsForDayAsync(DateTime day)
        {
            var dayStart = day.Date;
            var dayEnd = dayStart.AddDays(1);

            return await _dbContext.CalendarEvents
                .Where(e => e.EventDate >= dayStart && e.EventDate < dayEnd)
                .OrderBy(e => e.EventTime)
                .ToListAsync();
        }

        public async Task<CalendarEvent> AddEventAsync(CalendarEvent @event)
        {
            @event.CreatedAt = DateTime.Now;
            _dbContext.CalendarEvents.Add(@event);
            await _dbContext.SaveChangesAsync();
            return @event;
        }

        public async Task<CalendarEvent> UpdateEventAsync(CalendarEvent @event)
        {
            _dbContext.CalendarEvents.Update(@event);
            await _dbContext.SaveChangesAsync();
            return @event;
        }

        public async Task DeleteEventAsync(int id)
        {
            var @event = await _dbContext.CalendarEvents.FindAsync(id);
            if (@event != null)
            {
                _dbContext.CalendarEvents.Remove(@event);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<CalendarEvent>> GetUpcomingEventsAsync(int days = 7)
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(days);

            return await _dbContext.CalendarEvents
                .Where(e => e.EventDate >= today && e.EventDate <= endDate)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }
    }
}
