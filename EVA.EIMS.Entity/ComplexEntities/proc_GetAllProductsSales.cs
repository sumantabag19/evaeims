using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity.ComplexEntities
{

    public class proc_GetAllProductsSales
    {
        [Key]
        public int saleID { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public string releaseDate { get; set; }
        public string description { get; set; }
    }
}
