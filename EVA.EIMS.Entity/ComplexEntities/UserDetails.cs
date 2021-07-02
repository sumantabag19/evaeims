using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class UserDetails
    {
        [Key]
        public Guid UserId { get; set; }
        public long? AppUserId { get; set; }
        public long? AppOrgId { get; set; }
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
        public bool IsAccLock { get; set; }
        public DateTime PasswordExpiration { get; set; }
        public bool IsPasswordReset { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool MobileLoginEnabled { get; set; }
        public DateTime? LastPasswordChangeOn { get; set; }
        public bool IsActive { get; set; }
        public bool MultipleOrgAccess { get; set; }
        public Guid CreatedBy { get; set; }

        private DateTime _createdOn;
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
        public Guid ModifiedBy { get; set; }
        private DateTime _modifiedOn;
        public DateTime ModifiedOn
        {
            get
            {
                _modifiedOn = DateTime.Now;
                return _modifiedOn;
            }
            set { _modifiedOn = value; }
        }
        public int PasswordActivationPeriod { get; set; }
        public string Roles { get; set; }
        public string ClientType { get; set; }
        public bool RequiredSecurityQuestion { get; set; }
        public bool IsFirstTimeLogin { get; set; }

        public string ClientTypeId { get; set; }
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
                if (!String.IsNullOrEmpty(ClientType))
                {
                    return ClientType.Split(',');
                }
                return null;
            }
        }

        [NotMapped]
        public int[] ClientTypeIdArray
        {
            get
            {
                if (!String.IsNullOrEmpty(ClientTypeId))
                {

                   return ClientTypeId.Split(',').Select(int.Parse).ToArray();

                }

                return null;
            }
        }
        public DateTime? LockAccountDate { get; set; }
        public string AppId { get; set; }
        public DateTime? UnlockAccountDate { get; set; }
        public int[] AppIdArray
        {
            get
            {
                if (!String.IsNullOrEmpty(AppId))
                {

                    return AppId.Split(',').Select(int.Parse).ToArray();

                }

                return null;
            }
        }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string AppName { get; set; }
		[NotMapped]
		public string[] AppNameArray
		{
			get
			{
				if (!String.IsNullOrEmpty(AppName))
				{
					return AppName.Split(',');
				}
				return null;
			}
		}

		public string OrgName { get; set; }
		[NotMapped]
		public string[] OrgNameArray
		{
			get
			{
				if (!String.IsNullOrEmpty(OrgName))
				{
					return OrgName.Split(',');
				}
				return null;
			}
		}
	}
}
