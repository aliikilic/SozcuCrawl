using HtmlAgilityPack;
using SozcuCrawl.Models;
using System.Text;
using System;
using System.Net;
using Nest;
using System.Globalization;

namespace SozcuCrawl.Service
{
    public class ScrapperService
    {
        private readonly HttpClient _httpClient;

        public ScrapperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Author>> GetAuthorsAsync()
        {
            var authorList = new List<Author>();
            try
            {
                var html = await _httpClient.GetByteArrayAsync("https://www.sozcu.com.tr/yazarlar");
                string htmlContent = Encoding.UTF8.GetString(html);
                var document = new HtmlDocument();
                document.LoadHtml(htmlContent);

                var authorNodes = document.DocumentNode.SelectNodes("//a[contains(@class, 'author') and contains(@class, 'w-100')]");
                if (authorNodes != null)
                {
                    foreach (var authorNode in authorNodes)
                    {
                        var author = authorNode.SelectSingleNode(".//span[contains(@class, 'author-name')]");
                        string authorName = WebUtility.HtmlDecode(author.InnerText.Trim());

                        var titleNode = authorNode.SelectSingleNode(".//span[contains(@class, 'title')]");
                        string title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());

                        var dateNode = authorNode.SelectSingleNode(".//span[@class= 'small text-secondary']");
                        string date = WebUtility.HtmlDecode(dateNode.InnerText.Trim());
                        DateTime formattedDate = Convert.ToDateTime(date, new CultureInfo("tr-TR"));

                        var imgNode = authorNode.SelectSingleNode(".//img");
                        string imgUrl = imgNode.GetAttributeValue("src", null);

                        var articleList = new List<Article>();



                        string href = authorNode.GetAttributeValue("href", "Link Yok");
                        string articleContent = await GetArticleContentAsync(href);
                        articleList.Add(new Article()
                        {
                            Title = title,
                            Date = formattedDate,
                            Content = articleContent
                        });

                        authorList.Add(new Author
                        {
                            Name = authorName,
                            ImageUrl = imgUrl,
                            Articles = articleList
                        });

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching authors: {ex.Message}");
            }

            return authorList;
        }
        private async Task<string> GetArticleContentAsync(string articleLink)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetByteArrayAsync(articleLink);
                string htmlContent = Encoding.UTF8.GetString(response);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var articleBody = htmlDoc.DocumentNode.SelectSingleNode("//article//div[contains(@class, 'article-body')]");
                if (articleBody != null)
                {
                    var paragraphNodes = articleBody.SelectNodes(".//p");
                    if (paragraphNodes != null)
                    {
                        return string.Join("\n", paragraphNodes.Select(s => WebUtility.HtmlDecode(s.InnerText.Trim())));
                    }
                }
                return "Makale içeriği bulunamadı.";

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching article content: {ex.Message}");
                return "Makale içeriği alınırken hata oluştu.";
            }
        }
        public async Task<List<Agenda>> GetAgendasAsync()
        {

            var agendaList = new List<Agenda>();

            try
            {
                var html = await _httpClient.GetByteArrayAsync("https://www.sozcu.com.tr/gundem");
                string htmlContent = Encoding.UTF8.GetString(html);

                var document = new HtmlDocument();
                document.LoadHtml(htmlContent);

                var agendasNodes = document.DocumentNode.SelectNodes("//div[@class='col-md-6 col-lg-4 mb-4']");
                if (agendasNodes != null)
                {
                    foreach (var agendaNodes in agendasNodes)
                    {
                        var imgNode = agendaNodes.SelectSingleNode(".//a[@class='img-holder wide radius-base mb-2']/img");
                        string imgUrl = WebUtility.HtmlDecode(imgNode.GetAttributeValue("src", null));

                        var agendaLinkNode = agendaNodes.SelectSingleNode(".//a[@class='img-holder wide radius-base mb-2']");
                        var agendaLink = agendaLinkNode.GetAttributeValue("href", null);

                        var titleNode = agendaNodes.SelectSingleNode(".//span[@class = 'd-block fs-5 fw-semibold text-truncate-2']");
                        var title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());

                        var descriptionNode = agendaNodes.SelectSingleNode(".//span[@class = 'small text-secondary text-truncate-2']");
                        var description = WebUtility.HtmlDecode(descriptionNode.InnerText.Trim());

                        string agendaContent = await GetArticleContentAsync(agendaLink);
                        DateTime agendaDate = await GetArticleDateAsync(agendaLink);
                        agendaList.Add(new Agenda
                        {
                            Content = agendaContent,
                            Description = description,
                            ImageUrl = imgUrl,
                            Title = title,
                            Date = agendaDate
                        });

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            return agendaList;

        }
        private async Task<DateTime> GetArticleDateAsync(string agendaLink)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetByteArrayAsync(agendaLink);
            string htmlContent = Encoding.UTF8.GetString(response);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var dateNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='content-meta-date']/time/text()");
            string date = WebUtility.HtmlDecode(dateNode.InnerText.Trim());
            DateTime dateParse = DateTime.ParseExact(date, "dd MMMM yyyy - HH:mm", new CultureInfo("tr-TR"));
            
            DateTime formattedDate = dateParse.Date;

            return formattedDate;


        }
        public async Task<List<Sport>> GetSportsAsync()
        {

            var sportList = new List<Sport>();

            try
            {
                var html = await _httpClient.GetByteArrayAsync("https://www.sozcu.com.tr/spor");
                string htmlContent = Encoding.UTF8.GetString(html);

                var document = new HtmlDocument();
                document.LoadHtml(htmlContent);

                var sportsNodes = document.DocumentNode.SelectNodes("//div[@class='col-md-6 col-lg-4 mb-4']");
                if (sportsNodes != null)
                {
                    foreach (var sportNodes in sportsNodes)
                    {
                        var imgNode = sportNodes.SelectSingleNode(".//a[@class='img-holder wide radius-base mb-2']/img");
                        string imgUrl = WebUtility.HtmlDecode(imgNode.GetAttributeValue("src", null));

                        var sportLinkNode = sportNodes.SelectSingleNode(".//a[@class='img-holder wide radius-base mb-2']");
                        var sportLink = sportLinkNode.GetAttributeValue("href", null);

                        var titleNode = sportNodes.SelectSingleNode(".//span[@class = 'd-block fs-5 fw-semibold text-truncate-2']");
                        var title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());

                        var descriptionNode = sportNodes.SelectSingleNode(".//span[@class = 'small text-secondary text-truncate-2']");
                        var description = WebUtility.HtmlDecode(descriptionNode.InnerText.Trim());

                        var sportTypeNode = sportNodes.SelectSingleNode("//a[@class='d-block small fw-semibold text-sport-link']");
                        var sportType = WebUtility.HtmlDecode(sportTypeNode.InnerText.Trim());

                        string sportContent = await GetArticleContentAsync(sportLink);
                        DateTime sportDate = await GetArticleDateAsync(sportLink);
                        sportList.Add(new Sport
                        {
                            Content = sportContent,
                            Description = description,
                            ImageUrl = imgUrl,
                            Title = title,
                            Date = sportDate,
                            SportType=sportType
                        });

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            return sportList;
        }
        public async Task<List<World>> GetWorldsAsync()
        {

            var worldList = new List<World>();

            try
            {
                var html = await _httpClient.GetByteArrayAsync("https://www.sozcu.com.tr/dunya");
                string htmlContent = Encoding.UTF8.GetString(html);

                var document = new HtmlDocument();
                document.LoadHtml(htmlContent);

                var worldsNodes = document.DocumentNode.SelectNodes("//div[@class='col-md-6 col-lg-4 mb-4']");
                if (worldsNodes != null)
                {
                    foreach (var worldNodes in worldsNodes)
                    {
                        var imgNode = worldNodes.SelectSingleNode(".//a[@class='img-holder wide radius-base mb-2']/img");
                        string imgUrl = WebUtility.HtmlDecode(imgNode.GetAttributeValue("src", null));

                        var worldLinkNode = worldNodes.SelectSingleNode(".//a[@class='img-holder wide radius-base mb-2']");
                        var worldLink = worldLinkNode.GetAttributeValue("href", null);

                        var titleNode = worldNodes.SelectSingleNode(".//span[@class = 'd-block fs-5 fw-semibold text-truncate-2']");
                        var title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());

                        var descriptionNode = worldNodes.SelectSingleNode(".//span[@class = 'small text-secondary text-truncate-2']");
                        var description = WebUtility.HtmlDecode(descriptionNode.InnerText.Trim());

                        string agendaContent = await GetArticleContentAsync(worldLink);
                        DateTime agendaDate = await GetArticleDateAsync(worldLink);
                        worldList.Add(new World
                        {
                            Content = agendaContent,
                            Description = description,
                            ImageUrl = imgUrl,
                            Title = title,
                            Date = agendaDate
                        });

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            return worldList;

        }
    }
}
