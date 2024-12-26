namespace SozcuCrawl.Service
{
    public class DataRefresherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DataRefresherService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dataCoordinator = scope.ServiceProvider.GetRequiredService<DataCoordinator>();
                    await dataCoordinator.InitializeDataAsync();
                }
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);

            }
        }
    }
}
