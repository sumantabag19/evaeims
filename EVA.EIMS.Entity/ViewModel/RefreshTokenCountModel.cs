using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity.ViewModel
{
    public class RefreshTokenCountModel
    {
        [Key]
        public string ClientId { get; set; }
        public int RequestThreshold { get; set; }
        public int TokenCount { get; set; }
    }
}
