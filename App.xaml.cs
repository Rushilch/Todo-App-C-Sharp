using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ProductivityApp.Data;
using ProductivityApp.Services;
using ProductivityApp.ViewModels;

namespace ProductivityApp
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        public App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose; // Close app when main window closes
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Set up dependency injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // Initialize database
                var dbContext = _serviceProvider.GetRequiredService<ProductivityDbContext>();
                await dbContext.InitializeDatabaseAsync();

                // Create and show main window
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup Error: {ex.Message}\n\n{ex.StackTrace}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register DbContext
            services.AddDbContext<ProductivityDbContext>();

            // Register Services
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<IPomodoroService, PomodoroService>();
            services.AddSingleton<ICalendarService, CalendarService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();

            // Register ViewModels
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<TasksViewModel>();
            services.AddSingleton<PomodoroViewModel>();
            services.AddSingleton<CalendarViewModel>();
            services.AddSingleton<ClockViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AnalyticsViewModel>();
            services.AddSingleton<MainViewViewModel>();

            // Register Views
            services.AddSingleton<MainWindow>();
        }
    }
}

