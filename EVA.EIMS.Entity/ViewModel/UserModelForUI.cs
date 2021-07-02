using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class UserModelForUI
    {
        [Key]
        public Guid UserId { get; set; }
        public Guid Subject { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        [JsonIgnore]
        [JsonProperty(PropertyName = "ProtectedPassword")]
        public string PasswordHash { get; set; }
        public int? OrgId { get; set; }
        [JsonIgnore]
        public bool IsAccLock { get; set; }
        public DateTime PasswordExpiration { get; set; }
        public bool IsPasswordReset { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LastPasswordChangeOn { get; set; }
        public bool IsActive { get; set; }
        // marked as allow null as value is null in database
        public DateTime? LockAccountDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int? PasswordActivationPeriod { get; set; }
        public string Roles { get; set; }
        public string ClientType { get; set; }
        public bool RequiredSecurityQuestion { get; set; }
        public bool IsFirstTimeLogin { get; set; }
        public DateTime? UnlockAccountDate { get; set; }
        public string OrgName { get; set; }
        [NotMapped]
        public string[] RolesArray
        {
            get
            {
                if (!String.IsNullOrEmpty(Roles))
                {
                    return Roles.Split(',');
                }
                return null;
            }

        }
        [NotMapped]
        public string[] ClientTypeArray
        {
            get
            {
                if (!String.IsNullOrEmpty(Roles))
                {
                    return ClientType.Split(',');
                }
                return null;
            }
        }
    }
}



