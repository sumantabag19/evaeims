using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
  public class OauthClientModel
  {
    [Key]
    public int OauthClientId { get; set; }
    public string ClientId { get; set; }
    public string ClientName { get; set; }
    public string Flow { get; set; }
    public string AllowedScopes { get; set; }
    public string ClientTypeName { get; set; }
    public string AppName { get; set; }
    public bool? IsActive { get; set; }
    public bool? DeleteRefreshToken { get; set; }
    public int TokenValidationPeriod { get; set; }
    public double? ClientValidationPeriod { get; set; }
    public DateTime? ClientExpireOn { get; set; }
  }
}
