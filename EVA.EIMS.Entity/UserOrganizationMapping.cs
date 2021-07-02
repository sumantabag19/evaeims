using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class UserOrganizationMapping
    {
        [Key]
        public int UserOrgId { get; set; }
        public Guid UserId { get; set; }
        public int OrgId { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime CreatedOn { get; set; }
    }
}