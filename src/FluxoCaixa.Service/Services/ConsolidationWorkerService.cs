using FluxoCaixa.Application.Interfaces;

namespace FluxoCaixa.Service.Services
{
    public class ConsolidationWorkerService(ILogger<ConsolidationWorkerService> logger, IServiceFactory serviceFactory, IConfiguration configuration) : BackgroundService
    {
        private readonly ILogger<ConsolidationWorkerService> _logger = logger;
        private readonly IServiceFactory _serviceFactory = serviceFactory;
        private readonly int _frequency = Convert.ToInt16(configuration["Frequency"] ?? "1");

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var correlationId = Guid.NewGuid().ToString();

                using (_logger.BeginScope(new { CorrelationId = correlationId }))
                {
                    try
                    {
                        _logger.LogInformation("ConsolidationWorker running at: {time} [CorrelationId: {correlationId}]", DateTime.Now, correlationId);

                        var consolidationService = _serviceFactory.CreateConsolidationService();
                        _ = await consolidationService.GenerateDailyconsolidationAsync(DateTime.Today);

                        _logger.LogInformation("ConsolidationWorker process completed at {time} [CorrelationId: {correlationId}]", DateTime.Now, correlationId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during consolidation: {message} [CorrelationId: {correlationId}]", ex.Message, correlationId);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(_frequency), stoppingToken);
            }
        }
    }
}
