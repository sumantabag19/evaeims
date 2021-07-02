using EVA.EIMS.Entity.ComplexEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class UserViewModel
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public int? OrgId { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public bool? IsActive { get; set; }
        public string PlainTextPassword { get; set; }
        public int? Role { get; set; }
        public string ProviderName { get; set; }
        public MultiSelect[] ClientTypes { get; set; }
        public string EmailId { get; set; }
    }
}
