using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface IRecurringTaskService
    {
        Task<int> ProcessDueRecurrencesAsync();
        Task<RecurringTask> AddRecurringTaskAsync(RecurringTask recurringTask);
    }

    public class RecurringTaskService : IRecurringTaskService
    {
        private readonly ProductivityDbContext _dbContext;

        public RecurringTaskService(ProductivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RecurringTask> AddRecurringTaskAsync(RecurringTask recurringTask)
        {
            recurringTask.CreatedAt = DateTime.Now;
            _dbContext.RecurringTasks.Add(recurringTask);
            await _dbContext.SaveChangesAsync();
            return recurringTask;
        }

        public async Task<int> ProcessDueRecurrencesAsync()
        {
            var now = DateTime.Now;
            var due = await _dbContext.RecurringTasks
                .Where(r => r.IsActive && r.NextOccurrence <= now)
                .ToListAsync();

            var created = 0;

            foreach (var r in due)
            {
                // Create a new TaskItem for the recurrence
                var task = new TaskItem
                {
                    Title = r.Title,
                    Description = r.Description,
                    Category = TaskCategory.Other,
                    Priority = TaskPriority.Medium,
                    DueDate = r.NextOccurrence,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                };

                _dbContext.Tasks.Add(task);
                created++;

                // Advance next occurrence based on RecurrenceRule
                try
                {
                    switch (r.RecurrenceRule.ToLowerInvariant())
                    {
                        case "daily":
                            r.NextOccurrence = r.NextOccurrence.AddDays(1);
                            break;
                        case "weekly":
                            r.NextOccurrence = r.NextOccurrence.AddDays(7);
                            break;
                        case "monthly":
                            r.NextOccurrence = r.NextOccurrence.AddMonths(1);
                            break;
                        default:
                            // unsupported rule — deactivate
                            r.IsActive = false;
                            break;
                    }

                    _dbContext.RecurringTasks.Update(r);
                }
                catch
                {
                    r.IsActive = false;
                    _dbContext.RecurringTasks.Update(r);
                }
            }

            await _dbContext.SaveChangesAsync();
            return created;
        }
    }
}
