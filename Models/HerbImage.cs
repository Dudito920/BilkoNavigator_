namespace BilkoNavigator_.Models
{
    public class HerbImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }   // път към файла
        public DateTime UploadedOn { get; set; }
        public int UserId { get; set; }
        public int HerbId { get; set; }
        public User User { get; set; }
        public Herb Herb { get; set; }
    }
}
