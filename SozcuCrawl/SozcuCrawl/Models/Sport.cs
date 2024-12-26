using System.Text.Json.Serialization;

namespace SozcuCrawl.Models
{
    public class Sport
    {
        [JsonPropertyName("sports.sport_type")]
        public string SportType{ get; set; }
        [JsonPropertyName("sports.content")]
        public string Content { get; set; }

        [JsonPropertyName("sports.date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("sports.description")]
        public string Description { get; set; }

        [JsonPropertyName("sports.image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("sports.title")]
        public string Title { get; set; }
    }
}
