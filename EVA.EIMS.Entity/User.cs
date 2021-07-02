using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class User
    {
        [Key]
        [Required]
        //[Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid user id"))]
        public Guid UserId { get; set; }
        public long? AppUserId { get; set; }
        public long? AppOrgId { get; set; }
        public Guid Subject { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string FamilyName { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string EmailId { get; set; }
        [StringLength(60)]
        public string PhoneNumber { get; set; }
        [IgnoreDataMember]
        public bool PhoneNumberConfirmed { get; set; }
        [JsonIgnore]
        [JsonProperty(PropertyName = "ProtectedPassword")]
        [IgnoreDataMember]
        public string PasswordHash { get; set; }
        [Required]
        public int? OrgId { get; set; }
        [NotMapped]
        public string OrgName { get; set; }
        [IgnoreDataMember]
        public bool IsAccLock { get; set; }
        [IgnoreDataMember]
        public DateTime PasswordExpiration { get; set; }
        [IgnoreDataMember]
        public bool IsPasswordReset { get; set; }
        [Required]
        public bool? TwoFactorEnabled { get; set; }
        [IgnoreDataMember]
        public DateTime? LastPasswordChangeOn { get; set; }
        public bool? IsActive { get; set; }
        [IgnoreDataMember]
        public int? LockTypeID { get; set; }
        [IgnoreDataMember]
        public DateTime? LockAccountDate { get; set; }
        [IgnoreDataMember]
        public bool IsFirstTimeLogin { get; set; }
        [IgnoreDataMember]
        public bool RequiredSecurityQuestion { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
        private DateTime _createdOn;
        [IgnoreDataMember]
        public DateTime CreatedOn
        {
            get
            {
                if (_createdOn == null)
                    return DateTime.Now;
                else
                    return _createdOn;
            }
            set { _createdOn = value; }
        }

        private DateTime _modifiedOn;
        [IgnoreDataMember]
        public DateTime ModifiedOn
        {
            get
            {
                _modifiedOn = DateTime.Now;
                return _modifiedOn;
            }
            set { _modifiedOn = value; }
        }
        [NotMapped]
        [Required]
        public string PlainTextPassword { get; set; }
        [NotMapped]
        [Required]
        public string[] ClientType { get; set; }
        [NotMapped]
        [IgnoreDataMember]
        public string Organization { get; set; }
        [NotMapped]
        [Required]
        public string[] Roles { get; set; }
        [IgnoreDataMember]
        public DateTime? UnlockAccountDate { get; set; }
        [IgnoreDataMember]
        public int ProviderId { get; set; }
        [NotMapped]
        [Required]
        [StringLength(500)]
        public string ProviderName { get; set; }
        public bool? EmailVerified { get; set; }
    }
}

