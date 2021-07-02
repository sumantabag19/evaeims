using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class PasswordManagementDetails
    {
        [Key]
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
        public string LastPasswordChangeOn { get; set; }
        public string[] OldPwdHashArray
        {
            get
            {
                if (!String.IsNullOrEmpty(PasswordHash))
                {

                    return PasswordHash.Split(',');

                }

                return null;
            }
        }
        public DateTime[] LastPasswordChangeOnArray
        {
            get
            {
                if (!String.IsNullOrEmpty(LastPasswordChangeOn))
                {

                    return LastPasswordChangeOn.Split(',').Select(DateTime.Parse).ToArray();

                }

                return null;
            }
        }
    }
}
