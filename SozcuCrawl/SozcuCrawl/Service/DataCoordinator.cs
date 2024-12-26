using Nest;
using Newtonsoft.Json;

namespace SozcuCrawl.Service
{
    public class DataCoordinator
    {
        private readonly ScrapperService _scrapperService;
        private readonly ElasticsearchService _elasticsearchService;
        public string CachedData { get; private set; }

        public DataCoordinator(ScrapperService scrapperService, ElasticsearchService elasticsearchService)
        {
            _scrapperService = scrapperService;
            _elasticsearchService = elasticsearchService;
        }

        private async Task FetchAndIndexAuthorAsync()
        {
            var authors = await _scrapperService.GetAuthorsAsync();
            if(authors == null || !authors.Any())
            {

                throw new Exception("yazar verisi bulunamadı");
            }

            bool isSucces= await _elasticsearchService.IndexAuthorsAsync(authors);
            if(!isSucces)
                throw new Exception("yazar verisi eklenirken HATA oluştu");


        }
        private async Task FetchAndIndexAgendaAsync()
        {
            var agendas = await _scrapperService.GetAgendasAsync();
            if (agendas == null || !agendas.Any())
                throw new Exception("Gündem verisi bulunamadı");
            bool isSuccess = await _elasticsearchService.IndexAgendasAsync(agendas);
            if (!isSuccess)
                throw new Exception("Gündem verisi eklenirken bir hata oluştu.");
        }
        private async Task FetchAndIndexSportAsync()
        {
            var sports = await _scrapperService.GetSportsAsync();
            if (sports == null || !sports.Any())
                throw new Exception("Spor verisi bulunamadı");
            bool isSuccess = await _elasticsearchService.IndexSportsAsync(sports);
            if (!isSuccess)
                throw new Exception("Spor verisi eklenirken bir hata oluştu.");
        }
        private async Task FetchAndIndexWorldAsync()
        {
            var agendas = await _scrapperService.GetWorldsAsync();
            if (agendas == null || !agendas.Any())
                throw new Exception("Dünya haberleri verisi bulunamadı");
            bool isSuccess = await _elasticsearchService.IndexWorldsAsync(agendas);
            if (!isSuccess)
                throw new Exception("Dünya haberleri verisi eklenirken bir hata oluştu.");
        }
        public async Task InitializeDataAsync()
        {
            await FetchAndIndexAuthorAsync();
            await FetchAndIndexAgendaAsync();
            await FetchAndIndexSportAsync();
            await FetchAndIndexWorldAsync();


            var authors = await _elasticsearchService.GetAuthorsAsync();
            var agendas = await _elasticsearchService.GetAgendasAsync();
            var sports = await _elasticsearchService.GetSportsAsync();
            var worlds = await _elasticsearchService.GetWorldsAsync();

            CachedData = JsonConvert.SerializeObject(new
            {
                Authors = authors,
                Agendas = agendas,
                Sports = sports,
                Worlds = worlds
            });
        }

    }
}
