namespace BilkoNavigator_.Models
{
    public class HerbFinding
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HerbId { get; set; }
        public int LocationId { get; set; }
        public DateTime FoundOn { get; set; }
        public User User { get; set; }
        public Herb Herb { get; set; }
        public Location Location { get; set; }
    }
}
