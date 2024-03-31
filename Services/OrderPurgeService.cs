using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Project.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Services
{
    public class OrderPurgeService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public OrderPurgeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set the scheduled task start time to today at 14:30
            var currentTime = DateTime.Now;
            var purgeTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 14, 30, 0);

            // If it's already past 14:30, schedule the task for the next day
            if (currentTime > purgeTime)
            {
                purgeTime = purgeTime.AddDays(1);
            }

            var delay = purgeTime - currentTime;

            _timer = new Timer(PurgeOrders, null, delay, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void PurgeOrders(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BreadPitContext>();

                var orders = await context.Orders.ToListAsync();

                context.Orders.RemoveRange(orders);

                await context.SaveChangesAsync();
            }
        }
    }
}
