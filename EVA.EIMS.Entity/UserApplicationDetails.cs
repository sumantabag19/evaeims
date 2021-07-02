using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class UserApplicationDetails
    {
       
            [Key]
            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string AppId { get; set; }
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
