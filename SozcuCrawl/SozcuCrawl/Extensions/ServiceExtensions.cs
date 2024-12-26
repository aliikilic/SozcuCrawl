using Elasticsearch.Net;
using Nest;
using SozcuCrawl.Service;

namespace SozcuCrawl.Extensions
{
	public static class ServiceExtensions
	{
		public static void ConfigureElasticService(this IServiceCollection services, IConfiguration configuration)
		{
			var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Url"]!));
			var settings = new ConnectionSettings(pool);

			var client = new ElasticClient(settings);
			services.AddSingleton(client);
		}
		public static void ConfigureElasticsearchService(this IServiceCollection services) =>
			services.AddSingleton<ElasticsearchService>();

		public static void ConfigureScrapperService(this IServiceCollection services) =>
			services.AddSingleton<ScrapperService>();

        public static void ConfigureDataCoordinatorService(this IServiceCollection services) =>
            services.AddSingleton<DataCoordinator>();
        public static void ConfigureDataRefresherService(this IServiceCollection services)=>
            services.AddHostedService<DataRefresherService>();



    }
}