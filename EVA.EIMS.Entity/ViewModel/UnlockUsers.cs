using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
  public class UnlockUsers
  {
    [Key]
    [Required]
    public string userName { get; set; }
    [Required]
    public bool IsAccLock { get; set; }
    //[Required]
    //public string password { get; set; }
  }
}
