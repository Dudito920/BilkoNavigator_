namespace BilkoNavigator_.Models
{
    public class Herb
    {
        public int Id { get; set; }
        public string PopularName { get; set; }    
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

    

