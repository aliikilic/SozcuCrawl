using System.Text.Json.Serialization;

namespace SozcuCrawl.Models
{
	public class Author
	{
		[JsonPropertyName("authors.author_name")]
		public string Name { get; set; }
        [JsonPropertyName("authors.image_url")]
        public string ImageUrl { get; set; }
		public List<Article> Articles { get; set; }
	}
}
