using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class OrganizationApplicationDetails
    {
        [Key]
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string AppId { get; set; }
        public string CanAccessAllUser { get; set; }
        public string[] CanAccessAllUsers
        {
            get
            {
                return CanAccessAllUser.Split(',');
            }
        }
        public string[] AppIds
        {
            get
            {
                return AppId.Split(',');                
            }
        }        
        public string AppName { get; set; }
        public string[] AppNames
        {
            get
            {
                return AppName.Split(',');
            }
        }      
        public string AppDescription { get; set; }
        public string[] AppDescriptions
        {
            get
            {
                return AppDescription.Split(',');
            }
        }
    }
}
