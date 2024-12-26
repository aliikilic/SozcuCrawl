using Nest;
using Newtonsoft.Json;
using SozcuCrawl.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace SozcuCrawl.Service
{
    public class ElasticsearchService
    {
        private readonly ElasticClient _elasticClient;

        public ElasticsearchService(ElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<bool> IndexAuthorsAsync(List<Author> authors)
        {
            if (authors == null || !authors.Any())
                return false;

            var newAuthors = new List<Author>();

            foreach (var author in authors)
            {
                var isAuthorExists = await _elasticClient.SearchAsync<SozcuCrawlModel>(s => s
                    .Index("sozcucrawl")
                    .Query(q => q
                        .Nested(n => n
                            .Path(p => p.Authors)
                            .Query(nq => nq
                                .Term(t => t.Field("authors.name.keyword").Value(author.Name))
                            )
                        )
                    )
                );

                if (!isAuthorExists.Documents.Any())
                {
                    newAuthors.Add(new Author
                    {
                        Name = author.Name,
                        ImageUrl = author.ImageUrl,
                        Articles = author.Articles.Select(article => new Article
                        {
                            Title = article.Title,
                            Content = article.Content,
                            Date = article.Date
                        }).ToList()
                    });
                }
                else
                {
                    var existingAuthor = isAuthorExists.Documents.First().Authors.First(a => a.Name == author.Name);
                    var newArticles = author.Articles.Where(article =>
                        !existingAuthor.Articles.Any(existingArticle =>
                            existingArticle.Title == article.Title &&
                            existingArticle.Date == article.Date
                        )
                    ).ToList();

                    if (newArticles.Any())
                    {
                        newAuthors.Add(new Author
                        {
                            Name = author.Name,
                            ImageUrl = author.ImageUrl,
                            Articles = newArticles.Select(article => new Article
                            {
                                Title = article.Title,
                                Content = article.Content,
                                Date = article.Date
                            }).ToList()
                        });
                    }
                }
            }

            if (!newAuthors.Any())
            {
                return true;
            }

            // Yeni verileri Elasticsearch'e ekle
            var sozcuCrawlModel = new SozcuCrawlModel
            {
                Authors = newAuthors
            };

            var response = await _elasticClient.IndexAsync(sozcuCrawlModel, s => s.Index("sozcucrawl").Id(Guid.NewGuid().ToString()));

            if (!response.IsValid)
            {
                Console.WriteLine($"Error indexing authors: {response.ServerError?.Error.Reason}");
                return false;
            }

            Console.WriteLine("Yeni veriler başarıyla kaydedildi.");
            return true;
        }
        public async Task<List<Author>> GetAuthorsAsync()
        {
            var result = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                s.Index("sozcucrawl").Query(q =>
                    q.Nested(n =>
                        n.Path(p => p.Authors).Query(nq =>
                            nq.MatchAll()))).Source(src =>
                                src.Includes(i =>
                                    i.Fields(f => f.Authors))));



            List<Author> authors = result.Documents.SelectMany(doc => doc.Authors).ToList();
            return authors;
        }
        public async Task<bool> IndexAgendasAsync(List<Agenda> agendas)
        {
            if (agendas == null || !agendas.Any())
                return false;

            var newAgendas = new List<Agenda>();

            foreach (var agenda in agendas)
            {

                var isAgendaExists = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                    s.Index("sozcucrawl")
                    .Query(q => q
                        .Nested(n => n
                            .Path(p => p
                                .Agendas).Query(nq => nq
                                    .Term(t => t
                                        .Field("agendas.title.keyword").Value(agenda.Title))))));
                if (!isAgendaExists.Documents.Any())
                {
                    newAgendas.Add(new Agenda
                    {
                        ImageUrl = agenda.ImageUrl,
                        Title = agenda.Title,
                        Description = agenda.Description,
                        Content = agenda.Content,
                        Date = agenda.Date,
                    });
                    continue;
                }
            }

            var sozcuCrawlModel = new SozcuCrawlModel
            {
                Agendas = newAgendas
            };

            var response = await _elasticClient.IndexAsync(sozcuCrawlModel, s => s.Index("sozcucrawl").Id(Guid.NewGuid().ToString()));
            if (!response.IsValid)
                return false;

            return true;
        }
        public async Task<List<Agenda>> GetAgendasAsync()
        {
            var result = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                s.Index("sozcucrawl").Query(q =>
                    q.Nested(n =>
                        n.Path(p => p.Agendas).Query(nq =>
                            nq.MatchAll()))).Source(src =>
                                src.Includes(i =>
                                    i.Fields(f => f.Agendas))));

            List<Agenda> agendas = result.Documents.SelectMany(doc => doc.Agendas).ToList();
            return agendas;
        }
        public async Task<bool> IndexSportsAsync(List<Sport> sports)
        {
            if (sports == null || !sports.Any())
                return false;
            var newSports = new List<Sport>();

            foreach (var sport in sports)
            {
                var isSportExists = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                    s.Index("sozcucrawl").Query(q =>
                        q.Nested(n =>
                            n.Path(p =>
                                p.Sports).Query(nq =>
                                    nq.Term(t =>
                                        t.Field("sports.title.keyword").Value(sport.Title))))));
                if (!isSportExists.Documents.Any())
                {
                    newSports.Add(new Sport
                    {
                        SportType = sport.SportType,
                        Title = sport.Title,
                        Description = sport.Description,
                        ImageUrl = sport.ImageUrl,
                        Content = sport.Content,
                        Date = sport.Date,
                    });
                    continue;
                }
            }

            var sozcuCrawlModel = new SozcuCrawlModel
            {
                Sports = newSports
            };

            var response = await _elasticClient.IndexAsync(sozcuCrawlModel, s => s.Index("sozcucrawl").Id(Guid.NewGuid().ToString()));
            if (!response.IsValid)
                return false;

            return true;

        }
        public async Task<List<Sport>> GetSportsAsync()
        {
            var result = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                s.Index("sozcucrawl").Query(q =>
                    q.Nested(n =>
                        n.Path(p => p.Sports).Query(nq =>
                            nq.MatchAll()))).Source(src =>
                                src.Includes(i =>
                                    i.Fields(f => f.Sports))));

            List<Sport> sports = result.Documents.SelectMany(doc => doc.Sports).ToList();
            return sports;
        }
        public async Task<bool> IndexWorldsAsync(List<World> worlds)
        {
            if (worlds == null || !worlds.Any())
                return false;

            var newWorlds = new List<World>();

            foreach (var world in worlds)
            {
                var isWorldExists = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                    s.Index("sozcucrawl")
                    .Query(q => q
                        .Nested(n => n
                            .Path(p => p
                                .Worlds).Query(nq => nq
                                    .Term(t => t
                                        .Field("worlds.title.keyword").Value(world.Title))))));
                if (!isWorldExists.Documents.Any())
                {
                    newWorlds.Add(new World
                    {
                        ImageUrl = world.ImageUrl,
                        Title = world.Title,
                        Description = world.Description,
                        Content = world.Content,
                        Date = world.Date

                    });
                    continue;
                }
            }

            var sozcuCrawlModel = new SozcuCrawlModel
            {
                Worlds = newWorlds
            };

            var response = await _elasticClient.IndexAsync(sozcuCrawlModel, s => s.Index("sozcucrawl").Id(Guid.NewGuid().ToString()));
            if (!response.IsValid)
                return false;

            return true;
        }
        public async Task<List<World>> GetWorldsAsync()
        {
            var result = await _elasticClient.SearchAsync<SozcuCrawlModel>(s =>
                s.Index("sozcucrawl").Query(q =>
                    q.Nested(n =>
                        n.Path(p => p.Worlds).Query(nq =>
                            nq.MatchAll()))).Source(src =>
                                src.Includes(i =>
                                    i.Fields(f => f.Worlds))));

            List<World> worlds = result.Documents.SelectMany(doc => doc.Worlds).ToList();
            return worlds;
        }

        public async Task<DocumentMatches> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new DocumentMatches();

            var response = await _elasticClient.SearchAsync<dynamic>(s => s.Index("sozcucrawl").Size(100).Source(false)
            .Query(q => q
                .Bool(b => b
                .Should(sh => sh
                    .Nested(n => n
                        .Path("agendas")
                        .Query(nq => nq
                            .MultiMatch(mm => mm
                                .Query(searchTerm)
                                .Operator(Nest.Operator.Or)
                                .Fields(f => f
                                    .Field("agendas.title^3")
                                    .Field("agendas.content^2")
                                    .Field("agendas.description")
                                    .Field("agendas.decription")
                                )
                            )
                        )
                        .InnerHits(ih => ih
                            .Name("matching_agendas")
                            .Size(100)
                        )
                    ),
                sh => sh
                    .Nested(n => n
                        .Path("authors")
                        .Query(nq => nq
                            .Bool(bb => bb
                                .Should(
                                    s1 => s1
                                        .MultiMatch(mm => mm
                                            .Query(searchTerm)
                                            .Operator(Nest.Operator.Or)
                                            .Fields(f => f
                                                .Field("authors.name^3")
                                            )
                                        ),
                                    s2 => s2
                                        .Nested(nn => nn
                                            .Path("authors.articles")
                                            .Query(nqq => nqq
                                                .MultiMatch(mmm => mmm
                                                    .Query(searchTerm)
                                                    .Operator(Nest.Operator.Or)
                                                    .Fields(ff => ff
                                                        .Field("authors.articles.title^3")
                                                        .Field("authors.articles.content")
                                                    )
                                                )
                                            )
                                            .InnerHits(iih => iih
                                                .Name("matching_articles")
                                                .Size(100)
                                            )
                                        )
                                )
                                .MinimumShouldMatch(1)
                            )
                        )
                        .InnerHits(ih => ih
                            .Name("matching_authors")
                            .Size(100)
                        )
                    ),

                sh => sh
                    .Nested(n => n
                        .Path("sports")
                        .Query(nq => nq
                            .MultiMatch(mm => mm
                                .Query(searchTerm)
                                .Operator(Nest.Operator.Or)
                                .Fields(f => f
                                    .Field("sports.title^3")
                                    .Field("sports.content^2")
                                    .Field("sports.description")
                                    .Field("sports.imageUrl")
                                    .Field("sports.sportType")
                                    .Field("sports.sport_type")
                                )
                            )
                        )
                        .InnerHits(ih => ih
                            .Name("matching_sports")
                            .Size(100)
                        )
                    ),
                sh => sh
                    .Nested(n => n
                        .Path("worlds")
                        .Query(nq => nq
                            .MultiMatch(mm => mm
                                .Query(searchTerm)
                                .Operator(Nest.Operator.Or)
                                .Fields(f => f
                                    .Field("worlds.title^3")
                                    .Field("worlds.content^2")
                                    .Field("worlds.description")
                                    .Field("worlds.decription")
                                )
                            )
                        )
                        .InnerHits(ih => ih
                            .Name("matching_worlds")
                            .Size(100)
                        )
                    )
            )
            .MinimumShouldMatch(1)
        )
    )
    .Sort(srt => srt
        .Descending(SortSpecialField.Score)
    )
);
            var responseList = new DocumentMatches();

            foreach (var hit in response.Hits)
            {
                if(hit.InnerHits != null && hit.InnerHits.ContainsKey("matching_worlds"))
                {
                    var worldsInnerHits = hit.InnerHits["matching_worlds"].Hits.Hits;
                    foreach (var worldHit in worldsInnerHits)
                    {
                        var worldData = worldHit.Source.As<World>();
                        responseList.MatchingWorlds.Add(worldData);
                    }
                }
                if (hit.InnerHits != null && hit.InnerHits.ContainsKey("matching_sports"))
                {
                    var sportsInnerHits = hit.InnerHits["matching_sports"].Hits.Hits;
                    foreach (var sportHit in sportsInnerHits)
                    {
                        var sportData = sportHit.Source.As<Sport>();
                        responseList.MatchingSports.Add(sportData);
                    }
                }
                if (hit.InnerHits != null && hit.InnerHits.ContainsKey("matching_agendas"))
                {
                    var agendasInnerHits = hit.InnerHits["matching_agendas"].Hits.Hits;
                    foreach (var agendaHit in agendasInnerHits)
                    {
                        var agendaData = agendaHit.Source.As<Agenda>();
                        responseList.MatchingAgendas.Add(agendaData);
                    }
                }
                if (hit.InnerHits != null && hit.InnerHits.ContainsKey("matching_authors"))
                {
                    var authorsInnerHits = hit.InnerHits["matching_authors"].Hits.Hits;
                    foreach (var authorHit in authorsInnerHits)
                    {
                        var authorData = authorHit.Source.As<Author>();
                        
                        if (authorHit.InnerHits != null && authorHit.InnerHits.ContainsKey("matching_articles"))
                        {
                            var articlesInnerHits = authorHit.InnerHits["matching_articles"].Hits.Hits;
                            foreach (var articleHit in articlesInnerHits)
                            {
                                var articleData = articleHit.Source.As<Article>(); 
                                authorData.Articles.Add(articleData);
                                
                            }
                        }
                        responseList.MatchingAuthors.Add(authorData);
                    }
                }

            }


            return responseList;


        }


    }
}


