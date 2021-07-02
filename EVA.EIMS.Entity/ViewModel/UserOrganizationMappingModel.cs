using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class UserOrganizationMappingModel
    {
        [Required]
        public List<Guid> UserIdArray { get; set; }
        [Required]
        public List<string> OrgNameArray { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int[] OrgIdArray { get; set; }
    }
}
