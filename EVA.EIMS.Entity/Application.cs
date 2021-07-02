using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class Application
    {        
        [Key]
        [Required]
        [Range(1,Double.MaxValue,ErrorMessage =("Please provide valid application id"))]
        public int AppId { get; set; }
        [Required]
        [StringLength(50)]
        public string AppName { get; set; }
        [StringLength(150)]
        public string Description{ get; set; }
        [StringLength(255)]
        public string AppUrl { get; set; }
        [Required]        
        public bool? IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime CreatedOn { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
        [IgnoreDataMember]
        public DateTime ModifiedOn { get; set; }
        [Required]
        public bool? IsPwdExpNotify { get; set; }
        [Required]       
        [Range(0,30)]
        public int PwdExpNotifyDays { get; set; }
        public Guid? AzureAppId { get; set; }
    }
}
