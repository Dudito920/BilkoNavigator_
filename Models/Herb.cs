using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilkoNavigator_.Models
{
    public class Herb
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Популярното име е задължително")]
        public string PopularName { get; set; } = string.Empty;

        
        public string? LatinName { get; set; }

        public string? DialectNames { get; set; }
        public string? Aroma { get; set; }
        public string? Taste { get; set; }
        public string? Habitat { get; set; }
        public string? Season { get; set; }

        public bool IsPoisonous { get; set; } = false;
        public bool IsProtected { get; set; } = false;

        public string? UsedPart { get; set; }
        public string? Benefits { get; set; }
        public string? Description { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public HerbImage? Image { get; set; } // 🔴 КРИТИЧНО
    }
}
