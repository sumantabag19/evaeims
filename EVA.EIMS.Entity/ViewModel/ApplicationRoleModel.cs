using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
  public class ApplicationRoleModel
  {
    [Key]
    public int ApplicationRoleId { get; set; }
    public string AppName { get; set; }
    public string RoleName { get; set; }
    public bool? IsActive { get; set; }
  }
}
