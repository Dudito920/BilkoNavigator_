using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BilkoNavigator_.Models
{
    public class HerbFinding
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Променено от int на string
        public int HerbId { get; set; }
        public int LocationId { get; set; }
        public DateTime FoundOn { get; set; } = DateTime.Now;

        [ValidateNever]
        public User User { get; set; } = null!;

        [ValidateNever]
        public Herb Herb { get; set; } = null!;

        [ValidateNever]
        public Location Location { get; set; } = null!;
    }
}
