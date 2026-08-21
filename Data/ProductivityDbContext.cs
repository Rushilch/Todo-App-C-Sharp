using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data.Models;
using System;
using System.IO;

namespace ProductivityApp.Data
{
    public class ProductivityDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<PomodoroSession> PomodoroSessions { get; set; } = null!;
        public DbSet<TimerSession> TimerSessions { get; set; } = null!;
        public DbSet<RecurringTask> RecurringTasks { get; set; } = null!;
        public DbSet<CalendarEvent> CalendarEvents { get; set; } = null!;
        public DbSet<AppSettings> AppSettings { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<DailyStats> DailyStats { get; set; } = null!;

        private static string DbPath { get; } = System.IO.Path.Join(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "ProductivityApp",
            "productivity.db");

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed default app settings
            modelBuilder.Entity<AppSettings>().HasData(new AppSettings
            {
                Id = 1,
                PomodoroFocusMinutes = 25,
                PomodoroBreakMinutes = 5,
                PomodoroLongBreakMinutes = 15,
                SessionsBeforeLongBreak = 4,
                DarkModeEnabled = false,
                NotificationsEnabled = true,
                Use24HourFormat = false,
                Theme = "Light"
            });
        }

        public async Task InitializeDatabaseAsync()
        {
            // Ensure the directory exists
            var dbDir = System.IO.Path.GetDirectoryName(DbPath) ?? AppContext.BaseDirectory;
            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            var migrations = Database.GetMigrations();
            if (migrations.Any())
            {
                await Database.MigrateAsync();
                return;
            }

            await Database.EnsureCreatedAsync();
            await EnsureSqliteTablesAsync();
        }

        private async Task EnsureSqliteTablesAsync()
        {
            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "Tasks" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tasks" PRIMARY KEY AUTOINCREMENT,
                    "Title" TEXT NOT NULL,
                    "Description" TEXT NULL,
                    "Category" INTEGER NOT NULL,
                    "Priority" INTEGER NOT NULL,
                    "DueDate" TEXT NOT NULL,
                    "IsCompleted" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "CompletedAt" TEXT NULL,
                    "LastModified" TEXT NOT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "PomodoroSessions" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_PomodoroSessions" PRIMARY KEY AUTOINCREMENT,
                    "StartTime" TEXT NOT NULL,
                    "EndTime" TEXT NULL,
                    "Duration" INTEGER NOT NULL,
                    "IsCompleted" INTEGER NOT NULL,
                    "SessionNumber" INTEGER NOT NULL,
                    "TaskName" TEXT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "TimerSessions" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_TimerSessions" PRIMARY KEY AUTOINCREMENT,
                    "StartTime" TEXT NOT NULL,
                    "Duration" INTEGER NOT NULL,
                    "Name" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "RecurringTasks" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_RecurringTasks" PRIMARY KEY AUTOINCREMENT,
                    "Title" TEXT NOT NULL,
                    "Description" TEXT NULL,
                    "RecurrenceRule" TEXT NOT NULL,
                    "NextOccurrence" TEXT NOT NULL,
                    "IsActive" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "CalendarEvents" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_CalendarEvents" PRIMARY KEY AUTOINCREMENT,
                    "Title" TEXT NOT NULL,
                    "Description" TEXT NULL,
                    "EventDate" TEXT NOT NULL,
                    "EventTime" TEXT NULL,
                    "TaskId" INTEGER NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "HasReminder" INTEGER NOT NULL,
                    "ReminderMinutesBefore" INTEGER NOT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "AppSettings" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_AppSettings" PRIMARY KEY AUTOINCREMENT,
                    "PomodoroFocusMinutes" INTEGER NOT NULL,
                    "PomodoroBreakMinutes" INTEGER NOT NULL,
                    "PomodoroLongBreakMinutes" INTEGER NOT NULL,
                    "SessionsBeforeLongBreak" INTEGER NOT NULL,
                    "DarkModeEnabled" INTEGER NOT NULL,
                    "NotificationsEnabled" INTEGER NOT NULL,
                    "Use24HourFormat" INTEGER NOT NULL,
                    "Theme" TEXT NOT NULL,
                    "CurrentUserId" INTEGER NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "UserProfiles" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_UserProfiles" PRIMARY KEY AUTOINCREMENT,
                    "UserName" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "DailyStats" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_DailyStats" PRIMARY KEY AUTOINCREMENT,
                    "Date" TEXT NOT NULL,
                    "TasksCompleted" INTEGER NOT NULL,
                    "PomodorosCompleted" INTEGER NOT NULL,
                    "TotalFocusTimeMinutes" INTEGER NOT NULL
                );
                """);

            await Database.ExecuteSqlRawAsync("""
                INSERT OR IGNORE INTO "AppSettings" (
                    "Id",
                    "PomodoroFocusMinutes",
                    "PomodoroBreakMinutes",
                    "PomodoroLongBreakMinutes",
                    "SessionsBeforeLongBreak",
                    "DarkModeEnabled",
                    "NotificationsEnabled",
                    "Use24HourFormat",
                    "Theme",
                    "CurrentUserId")
                VALUES (1, 25, 5, 15, 4, 0, 1, 0, 'Default', NULL);
                """);
        }
    }
}
