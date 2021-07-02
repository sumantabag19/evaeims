using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{
    public class PurchaseorderSaleDetails
    {
        [Key]
        public int saleID { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public int Quantity { get; set; }
    }
}
