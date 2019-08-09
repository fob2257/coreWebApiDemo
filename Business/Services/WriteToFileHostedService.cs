using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace coreWebApiDemo.Business.Services
{
  public class WriteToFileHostedService : IHostedService, IDisposable
  {
    private readonly IHostingEnvironment environment;
    private readonly string fileName = "file_1.txt";
    private Timer timer;
    private double ms = DateTime.Now.Subtract(DateTime.MinValue).TotalMilliseconds;

    public WriteToFileHostedService(IHostingEnvironment environment)
    {
      this.environment = environment;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      WriteToFile("WriteToFileHostedService: Process started");
      timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      WriteToFile("WriteToFileHostedService: Process stopped");
      timer?.Change(Timeout.Infinite, 0);
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      timer?.Dispose();
    }

    private void DoWork(object state)
    {
      WriteToFile($"WriteToFileHostedService: {DateTime.UtcNow}");
    }

    private void WriteToFile(string message)
    {
      var path = $@"{environment.ContentRootPath}\{fileName}";

      using (StreamWriter writer = new StreamWriter(path, append: true))
      {
        writer.WriteLine(message);
      }
    }
  }
}
