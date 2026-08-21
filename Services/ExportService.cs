using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface IExportService
    {
        Task<string> ExportTasksAsync(IEnumerable<TaskItem> tasks, string format = "csv");
    }

    public class ExportService : IExportService
    {
        public async Task<string> ExportTasksAsync(IEnumerable<TaskItem> tasks, string format = "csv")
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var filename = $"TodoApp-Tasks-{timestamp}.{format.ToLowerInvariant()}";
            var fullPath = Path.Combine(docs, filename);

            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(tasks, options);
                await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);
                return fullPath;
            }

            // default: CSV
            var sb = new StringBuilder();
            sb.AppendLine("Id,Title,Description,Category,Priority,DueDate,IsCompleted,CreatedAt,CompletedAt,LastModified");
            foreach (var t in tasks)
            {
                var line = $"{t.Id},\"{Escape(t.Title)}\",\"{Escape(t.Description ?? string.Empty)}\",{(int)t.Category},{(int)t.Priority},{t.DueDate:O},{(t.IsCompleted ? 1 : 0)},{t.CreatedAt:O},{(t.CompletedAt?.ToString("O") ?? "")},{t.LastModified:O}";
                sb.AppendLine(line);
            }

            await File.WriteAllTextAsync(fullPath, sb.ToString(), Encoding.UTF8);
            return fullPath;
        }

        private string Escape(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Replace("\"", "\"\"");
        }
    }
}
