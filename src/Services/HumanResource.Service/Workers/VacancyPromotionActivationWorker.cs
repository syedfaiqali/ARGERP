using HumanResource.Service.Services;

namespace HumanResource.Service.Workers;

public sealed class VacancyPromotionActivationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<VacancyPromotionActivationWorker> _logger;

    public VacancyPromotionActivationWorker(IServiceScopeFactory scopeFactory, ILogger<VacancyPromotionActivationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));
        do
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IVacancyFulfillmentService>();
                var promotionCount = await service.ActivateDuePromotionsAsync(stoppingToken);
                if (promotionCount > 0)
                    _logger.LogInformation("Activated {Count} scheduled internal vacancy promotions.", promotionCount);
                var exitCount = await service.ReleaseDueEmployeeExitsAsync(stoppingToken);
                if (exitCount > 0)
                    _logger.LogInformation("Released {Count} positions for employees whose leaving date is due.", exitCount);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Vacancy promotion activation cycle failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
