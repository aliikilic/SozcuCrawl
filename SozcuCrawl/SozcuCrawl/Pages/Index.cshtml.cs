using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SozcuCrawl.Models;
using SozcuCrawl.Service;

namespace SozcuCrawl.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataCoordinator _dataCoordinator;
        private readonly ElasticsearchService _elasticsearchService;

        public IndexModel(DataCoordinator dataCoordinator, ElasticsearchService elasticsearchService)
        {
            _dataCoordinator = dataCoordinator;
            _elasticsearchService = elasticsearchService;
        }
        public string SelectedCategory { get; set; } = "dunya";
        public DocumentMatches SearchResults { get; set; }

        [BindProperty]
        public string SearchQuery { get; set; }

        public IActionResult OnGet(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                SelectedCategory = category.ToLower();
            }
            ViewData["AllData"] = _dataCoordinator.CachedData;
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["AllData"] = _dataCoordinator.CachedData;

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                SearchResults = await _elasticsearchService.SearchAsync(SearchQuery);
                ViewData["MatchingAuthors"] = SearchResults.MatchingAuthors;
                ViewData["MatchingAgendas"] = SearchResults.MatchingAgendas;
                ViewData["MatchingSports"] = SearchResults.MatchingSports;
                ViewData["MatchingWorlds"] = SearchResults.MatchingWorlds;

            }
            else
            {
                ViewData["MatchingAuthors"] = null;
                ViewData["MatchingAgendas"] = null;
                ViewData["MatchingSports"] = null;
                ViewData["MatchingWorlds"] = null;
            }

            return Page();
        }
    }
}