namespace SozcuCrawl.Models
{
    public class DocumentMatches
    {

        public string DocumentId { get; set; }
        public List<Agenda> MatchingAgendas { get; set; } = new();
        public List<Author> MatchingAuthors { get; set; } = new();
        public List<Sport> MatchingSports { get; set; } = new();
        public List<World> MatchingWorlds { get; set; } = new();
    }
}
