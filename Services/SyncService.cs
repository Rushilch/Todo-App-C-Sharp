using System;
using System.IO;
using System.Threading.Tasks;

namespace ProductivityApp.Services
{
    public interface ISyncService
    {
        Task<string> SyncToCloudAsync();
    }

    // Simple file-based "cloud" sync that copies the DB file to user's Documents/TodoAppCloud
    public class SyncService : ISyncService
    {
        public async Task<string> SyncToCloudAsync()
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var cloudDir = Path.Combine(docs, "TodoAppCloud");
            Directory.CreateDirectory(cloudDir);

            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProductivityApp", "productivity.db");
            if (!File.Exists(dbPath))
                throw new FileNotFoundException("Database file not found", dbPath);

            var dest = Path.Combine(cloudDir, $"productivity-sync-{DateTime.Now:yyyyMMdd-HHmmss}.db");
            // Use simple file copy
            File.Copy(dbPath, dest, overwrite: true);
            await Task.CompletedTask;
            return dest;
        }
    }
}
