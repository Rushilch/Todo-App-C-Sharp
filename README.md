# Productivity Suite - A Modern Desktop Application

A professional, feature-rich productivity application built with C# and WPF, designed as a final-year Computer Science project. This application combines task management, time tracking, and personal analytics in a sleek, modern interface.

## 🎯 Core Features

### 1. **Task Management System**
- ✅ Create, edit, and delete tasks
- 📂 Organize tasks by categories (Work, Personal, Study, Health, Other)
- 🎯 Priority levels (Low, Medium, High)
- 📅 Due date management with reminders
- ✓ Mark tasks as completed
- 🔍 Search and filter functionality
- 💾 Persistent SQLite database storage

### 2. **Pomodoro Timer**
- 🍅 25-minute focus sessions + 5-minute breaks (default)
- ⚙️ Fully customizable durations
- 🔄 Auto-switch between focus and break modes
- 📊 Daily session tracking
- 📈 Focus time analytics

### 3. **Timer & Stopwatch**
- ⏳ Custom countdown timer with alerts
- ⏱️ Stopwatch with lap functionality
- 🎯 Real-time smooth updates
- 🔔 Visual and audio notifications

### 4. **Calendar & Events**
- 📅 Monthly and weekly calendar views
- 📌 Add events with date and time
- ⏰ Reminder notifications
- 🔗 Optional task-calendar sync

### 5. **Digital Clock Dashboard**
- 🕐 Live clock display
- 🌍 12/24-hour format toggle
- 📆 Date and day display
- Optional: World clock support

## 🎨 Modern UI/UX

- **Clean & Minimal Design**: Professional interface inspired by modern productivity apps
- **Dark Mode + Light Mode**: Easy on the eyes, user preference saved
- **MVVM Architecture**: Scalable, testable, maintainable codebase
- **Smooth Animations**: Responsive transitions and visual feedback
- **Sidebar Navigation**: Quick access to all features
- **Responsive Layout**: Adapts to different screen sizes

## 🧠 Advanced Features

### Data & Analytics
- 📊 **Analytics Dashboard**: View weekly productivity stats
  - Tasks completed per day
  - Focus time tracked
  - Pomodoro sessions completed
- 📈 **Productivity Insights**: Track trends and patterns
- 💾 **Export Functionality**: Export tasks and events as CSV/JSON

### Settings & Customization
- ⚙️ **Customizable Timers**: Adjust Pomodoro durations
- 🎨 **Theme Selection**: Dark/Light mode toggle
- 🔔 **Notification Preferences**: Control alerts
- ⏰ **Clock Format**: 12/24-hour format selection

### Data Persistence
- 🗄️ **SQLite Database**: Local storage for all data
- 🔒 **Automatic Backups**: (Optional future feature)
- ☁️ **Cloud Sync**: (Optional future enhancement)

## 🏗️ Architecture & Tech Stack

### Technology Stack
- **Language**: C# 10+
- **Framework**: .NET 8.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Database**: SQLite with Entity Framework Core 8.0
- **Architecture Pattern**: MVVM (Model-View-ViewModel)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

### Project Structure
```
ProductivityApp/
├── Data/                          # Data access layer
│   ├── Models/
│   │   └── Entities.cs           # Database entity models
│   └── ProductivityDbContext.cs  # EF Core DbContext
├── Services/                      # Business logic layer
│   ├── TaskService.cs
│   ├── PomodoroService.cs
│   ├── CalendarService.cs
│   ├── SettingsAndAnalyticsService.cs
│   └── Interfaces/
├── ViewModels/                    # MVVM ViewModels
│   ├── BaseViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── TasksViewModel.cs
│   ├── PomodoroViewModel.cs
│   ├── CalendarAndClockViewModel.cs
│   ├── SettingsAndAnalyticsViewModel.cs
│   └── MainViewViewModel.cs
├── Views/                         # WPF Views (UI)
│   ├── DashboardView.xaml
│   ├── TasksView.xaml
│   ├── PomodoroView.xaml
│   ├── CalendarView.xaml
│   ├── ClockView.xaml
│   ├── AnalyticsView.xaml
│   └── SettingsView.xaml
├── MVVM/                          # MVVM framework utilities
│   └── BaseViewModel.cs
└── MainWindow.xaml                # Main application shell
```

### Design Patterns Used
- **MVVM**: Separation of UI and business logic
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **Observer Pattern**: INotifyPropertyChanged for reactive UI
- **Singleton Pattern**: Service instances

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Windows 10/11
- Visual Studio 2022 or Visual Studio Code with C# extensions

### Installation

1. **Clone or Download** the repository
```bash
cd ProductivityApp
```

2. **Restore Dependencies**
```bash
dotnet restore
```

3. **Build the Application**
```bash
dotnet build
```

4. **Run the Application**
```bash
dotnet run
```

## 📊 Database Schema

The application uses SQLite with the following main entities:

### TaskItem
- Id (Primary Key)
- Title
- Description
- Category (Enum)
- Priority (Enum)
- DueDate
- IsCompleted
- CreatedAt
- CompletedAt
- LastModified

### PomodoroSession
- Id (Primary Key)
- StartTime
- EndTime
- Duration (minutes)
- IsCompleted
- SessionNumber
- TaskName

### CalendarEvent
- Id (Primary Key)
- Title
- Description
- EventDate
- EventTime
- TaskId (Foreign Key)
- CreatedAt
- HasReminder
- ReminderMinutesBefore

### AppSettings
- Id (Primary Key)
- PomodoroFocusMinutes
- PomodoroBreakMinutes
- PomodoroLongBreakMinutes
- DarkModeEnabled
- NotificationsEnabled
- Use24HourFormat

### DailyStats
- Id (Primary Key)
- Date
- TasksCompleted
- PomodorosCompleted
- TotalFocusTimeMinutes

## 🔒 Security & Best Practices

- ✅ Input validation for all user inputs
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Null reference handling with nullable reference types (#nullable enable)
- ✅ Asynchronous database operations
- ✅ Proper exception handling
- ✅ LocalAppData storage for user data

## 📈 Future Enhancements

- [ ] Cloud synchronization (OneDrive/GitHub)
- [ ] Multi-user support with authentication
- [ ] Windows Toast Notifications integration
- [ ] Drag-and-drop task reordering
- [ ] Keyboard shortcuts
- [ ] Productivity reports (PDF export)
- [ ] Integration with calendar services (Outlook, Google Calendar)
- [ ] Mobile companion app
- [ ] WinUI 3 migration for modern Windows 11 integration

## 🎓 Learning Outcomes

This project demonstrates:
- **Software Architecture**: MVVM pattern implementation
- **Database Design**: Entity relationships and EF Core usage
- **UI/UX Design**: Modern WPF application design
- **C# Mastery**: Advanced features and best practices
- **Object-Oriented Programming**: Inheritance, interfaces, polymorphism
- **Clean Code**: Well-organized, documented, maintainable codebase
- **Testing**: (Testable architecture with dependency injection)

## 📝 Code Quality

- **Language**: C# 10+ with nullable reference types enabled
- **Code Style**: follows Microsoft C# coding conventions
- **Documentation**: XML comments on public APIs
- **Architecture**: Clean separation of concerns
- **Error Handling**: Comprehensive exception handling

## 🎯 Key Implementation Highlights

### 1. Reactive UI with MVVM
```csharp
// BaseViewModel implementation with property change notifications
protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
{
    if (Equals(field, value)) return false;
    field = value;
    OnPropertyChanged(propertyName);
    return true;
}
```

### 2. Async Database Operations
```csharp
// Fully asynchronous service methods
public async Task<List<TaskItem>> GetUpcomingTasksAsync(int days = 7)
{
    var startDate = DateTime.Today;
    var endDate = startDate.AddDays(days);
    
    return await _dbContext.Tasks
        .Where(t => !t.IsCompleted && t.DueDate >= startDate && t.DueDate <= endDate)
        .OrderBy(t => t.DueDate)
        .ToListAsync();
}
```

### 3. Dependency Injection Configuration
```csharp
// Centralized DI setup in App.xaml.cs
private void ConfigureServices(ServiceCollection services)
{
    services.AddDbContext<ProductivityDbContext>();
    services.AddSingleton<ITaskService, TaskService>();
    services.AddSingleton<IPomodoroService, PomodoroService>();
    // ... more registrations
}
```

## 📄 License

This project is provided as an educational resource. Feel free to use it as a portfolio project or learning material.

## 🤝 Contributing

Suggestions for improvements are welcome! Feel free to:
- Report bugs
- Suggest new features
- Improve documentation
- Enhance the UI/UX
