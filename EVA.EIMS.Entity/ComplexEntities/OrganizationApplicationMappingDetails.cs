using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class OrganizationApplicationMappingDetails
    {
        [Key]
        public int OrgId { get; set; }
        public string AppId { get; set; }
        
       
    }
}
