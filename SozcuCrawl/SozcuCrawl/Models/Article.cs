using System.Text.Json.Serialization;

namespace SozcuCrawl.Models
{
	public class Article
    {
        [JsonPropertyName("articles.title")]

        public string Title { get; set; }
        [JsonPropertyName("articles.content")]
        public string Content { get; set; }
        [JsonPropertyName("articles.date")]
        public DateTime? Date { get; set; }
	}
}
