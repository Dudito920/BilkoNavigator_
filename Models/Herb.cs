using System.ComponentModel.DataAnnotations;

namespace BilkoNavigator_.Models
{
    public class Herb
    {
        public int Id { get; set; }

        [Required]
        public string PopularName { get; set; }    

        [Required]
        public string LatinName { get; set; }        

        public string DialectNames { get; set; }     

        public string Aroma { get; set; }
        public string Taste { get; set; }
        public string Habitat { get; set; }
        public string Season { get; set; }
        public bool IsPoisonous { get; set; }
        public bool IsProtected { get; set; }
        public string UsedPart { get; set; }
        public string Benefits { get; set; }
        public string Description { get; set; }
        public HerbImage Image { get; set; }
    }
}

