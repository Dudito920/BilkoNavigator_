using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BilkoNavigator_.Models
{
    public class HerbImage
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }   // път към файла
        public DateTime UploadedOn { get; set; }= DateTime.Now;
        
        public int HerbId { get; set; }
        [ValidateNever]
        public Herb Herb { get; set; } = null!;

        //public string? UserId { get; set; }
        //[ValidateNever]
        //public User User { get; set; }


    }
}
