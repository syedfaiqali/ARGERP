using HumanResource.Service.Services;
using Microsoft.Extensions.Options;

namespace HumanResource.Service.Workers
{
    public sealed class ZkAttendanceSyncWorker : BackgroundService
    {
        private readonly ILogger<ZkAttendanceSyncWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ZkAttendanceSyncOptions _options;

        public ZkAttendanceSyncWorker(
            ILogger<ZkAttendanceSyncWorker> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<ZkAttendanceSyncOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ZK Attendance Sync Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<IZkAttendanceSyncService>();
                    await syncService.SyncAttendanceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during ZK attendance sync.");
                }

                var intervalMinutes = _options.IntervalMinutes > 0 ? _options.IntervalMinutes : 5;
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }

            _logger.LogInformation("ZK Attendance Sync Worker stopped.");
        }
    }
}
