using System.Text.Json.Serialization;

namespace SozcuCrawl.Models
{
    public class Agenda
    {
        [JsonPropertyName("agendas.content")]
        public string Content { get; set; }

        [JsonPropertyName("agendas.date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("agendas.description")]
        public string Description { get; set; }

        [JsonPropertyName("agendas.image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("agendas.title")]
        public string Title { get; set; }
    }
}
