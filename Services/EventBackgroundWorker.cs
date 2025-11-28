using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatingWeb.Services
{
    public class EventBackgroundWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventBackgroundWorker> _logger;

        public EventBackgroundWorker(IServiceProvider serviceProvider, ILogger<EventBackgroundWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EventBackgroundWorker running at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // 1. Mark finished events
                        var now = DateTime.UtcNow;
                        var eventsToFinish = await context.Events
                            .Where(e => e.EndsAt < now && e.Status == EventStatus.Published)
                            .ToListAsync(stoppingToken);

                        foreach (var evt in eventsToFinish)
                        {
                            evt.Status = EventStatus.Finished;
                            // TODO: Send rating reminders
                        }

                        if (eventsToFinish.Any())
                        {
                            await context.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Finished {count} events.", eventsToFinish.Count);
                        }

                        // 2. No-show enforcement could be added here
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in EventBackgroundWorker");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Run every minute
            }
        }
    }
}
