using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
  public class ApplicationModel
  {
    [Key]
    [Required]
    [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid application id"))]
    public int AppId { get; set; }
    [Required]
    [StringLength(50)]
    public string AppName { get; set; }
    [StringLength(150)]
    public string Description { get; set; }
    [StringLength(255)]
    public string AppUrl { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsPwdExpNotify { get; set; }
    public int? PwdExpNotifyDays { get; set; }
  }
}
