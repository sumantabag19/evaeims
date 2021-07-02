using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class OauthClient
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid client id"))]
        public int OauthClientId { get; set; }
        [Required]
        [StringLength(100)]
        public string ClientId { get; set; }
        public Guid ClientGuid { get; set; }
        [Required]
        [StringLength(250)]
        public string ClientName { get; set; }
        [StringLength(250)]
        public string ClientSecret { get; set; }
        [Required]
        [StringLength(60)]
        public string Flow { get; set; }
        [Required]
        [StringLength(160)]
        public string AllowedScopes { get; set; }
        [Required]
        public int ClientTypeId { get; set; }
        [Required]
        public int AppId{ get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [Required]
        public bool? DeleteRefreshToken { get; set; }
        [Required]
        [Range(0,1440)]
        public int TokenValidationPeriod { get; set; }
        [Required]
        public double? ClientValidationPeriod { get; set; }
        public DateTime? ClientExpireOn { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
        private DateTime? _createdDate;
        [IgnoreDataMember]
        public DateTime? CreatedOn
        {
            get
            {
                if (_createdDate == null)
                    return DateTime.Now;
                else
                    return _createdDate;
            }
            set { _createdDate = value; }
        }
        private DateTime? _modifiedDate;
        [IgnoreDataMember]
        public DateTime? ModifiedOn
        {
            get
            {
                _modifiedDate = DateTime.Now;
                return _modifiedDate;
            }
            set { _modifiedDate = value; }
        }
        public string RequestURL { get; set; }
        public string RedirectURL { get; set; }
        public string DebugURL { get; set; }
        public int? RequestThreshold { get; set; }

        public static explicit operator OauthClient(ConfiguredTaskAwaitable<OauthClient> v)
        {
          throw new NotImplementedException();
        }
  }


    public class ClientApplicationDetails
    {
        [Key]
        public string ClientId { get; set; }
        public int? AppId { get; set; }
        public string AppName { get; set; }
        public Guid? AzureAppId { get; set; }
        public string RedirectURL { get; set; }
        public string DebugURL { get; set; }
    }
}
