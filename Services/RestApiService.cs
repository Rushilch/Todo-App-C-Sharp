using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ProductivityApp.Services;
using ProductivityApp.Data.Models;
using System.Collections.Generic;
using System.Text;

namespace ProductivityApp.Services
{
    public interface IRestApiService
    {
        Task StartAsync();
        Task StopAsync();
    }

    // Very small read-only local REST API for tasks
    public class RestApiService : IRestApiService
    {
        private readonly ITaskService _taskService;
        private HttpListener? _listener;
        private bool _running;

        public RestApiService(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public Task StartAsync()
        {
            if (_running) return Task.CompletedTask;
            _running = true;
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5050/");
            _listener.Start();

            Task.Run(async () =>
            {
                while (_running)
                {
                    try
                    {
                        var ctx = await _listener.GetContextAsync();
                        _ = HandleContextAsync(ctx);
                    }
                    catch { /* ignore */ }
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _running = false;
            try { _listener?.Stop(); } catch { }
            _listener = null;
            return Task.CompletedTask;
        }

        private async Task HandleContextAsync(HttpListenerContext ctx)
        {
            try
            {
                var req = ctx.Request;
                var res = ctx.Response;
                if (req.HttpMethod == "GET" && req.Url.AbsolutePath.Trim('/') == "tasks")
                {
                    var tasks = await _taskService.GetAllTasksAsync();
                    var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
                    var buffer = Encoding.UTF8.GetBytes(json);
                    res.ContentType = "application/json";
                    res.ContentEncoding = Encoding.UTF8;
                    res.ContentLength64 = buffer.Length;
                    await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    res.StatusCode = 404;
                    var msg = Encoding.UTF8.GetBytes("Not Found");
                    res.OutputStream.Write(msg, 0, msg.Length);
                }
                res.OutputStream.Close();
            }
            catch { }
        }
    }
}
