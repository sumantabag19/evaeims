using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
   public class OrganizationTenantMappingModel
    {
        [Key]
        public int OrganizationTenantId { get; set; }
        public string OrgName { get; set; }
        public int OrgId { get; set; }
        public Guid TenantId { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }

    public class OrganizationApplicationDetailModel
    {
        [Key]
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public Guid TenantId { get; set; }
        public int AppId { get; set; }
        public string AppName { get; set; }
        public Guid AzureAppId { get; set; }
    }

    public class OrganizationTenantMappingDomainModel
    {
        [Key]
        public int OrganizationTenantId { get; set; }
        public int OrgId { get; set; }
        public Guid TenantId { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
