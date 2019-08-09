using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DAL.Entities;

namespace coreWebApiDemo.Business.Services
{
  public class ConsumeScopedServiceHostedService : IHostedService, IDisposable
  {
    private Timer timer;
    public IServiceProvider services { get; }

    public ConsumeScopedServiceHostedService(IServiceProvider services)
    {
      this.services = services;
    }

    private void TimerCb(object state)
    {
      using (var scope = services.CreateScope())
      {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var log = new HostedServiceLog
        {
          Message = $"ConsumeScopedServiceHostedService message: {DateTime.UtcNow.ToString()}",
          CreatedAt = DateTime.UtcNow,
        };

        context.HostedServiceLogs.Add(log);
        context.SaveChangesAsync().Wait();
      }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      timer = new Timer(TimerCb, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      timer?.Change(Timeout.Infinite, 0);
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      timer?.Dispose();
    }
  }
}
