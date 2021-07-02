
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class UserRoleMapping
    {
        [Key]
        public int UserRoleId { get; set; }
        public int RoleId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
        private DateTime _createdOn;
        [IgnoreDataMember]
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

        private DateTime _modifiedOn;
        [IgnoreDataMember]
        public DateTime ModifiedOn
        {
            get
            {
                return DateTime.Now;
            }
            set { _modifiedOn = value; }
        }

    }
}
