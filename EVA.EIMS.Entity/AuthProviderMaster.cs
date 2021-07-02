using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
   public class AuthProviderMaster
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid provider id"))]
        public int ProviderID { get; set; }
        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; }
        [StringLength(200)]
        public string  ProviderDescription { get; set; }
        [StringLength(500)]
        public string Configuration { get; set; }
        [IgnoreDataMember]
        public Guid? UpdatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime? UpdatedOn { get; set; }
        [Required]
        public bool? IsActive { get; set; }
    }
}
