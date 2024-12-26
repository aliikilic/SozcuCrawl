using System.Text.Json.Serialization;

namespace SozcuCrawl.Models
{
    public class World
    {
        [JsonPropertyName("world.content")]
        public string Content { get; set; }

        [JsonPropertyName("world.date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("world.description")]
        public string Description { get; set; }

        [JsonPropertyName("world.image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("world.title")]
        public string Title { get; set; }
    }
}
