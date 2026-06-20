using T3awuny.Application.Contracts;

namespace T3awunyWebService.BackgroundServices
{
    public class AuctionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuctionBackgroundService> _logger;
        public AuctionBackgroundService(IServiceProvider serviceProvider, ILogger<AuctionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

                    await auctionService.ProcessAuctionStartsAsync();
                    await auctionService.ProcessAuctionEndsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Error processing auctions");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }

}
