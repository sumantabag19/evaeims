using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class ApplicationUserMapping
    {
        [Key]
        public int MappingId { get; set; }
        public Guid UserId { get; set; }
        public int AppId { get; set; }
        public bool IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }

        private DateTime? _createdDate;
        [IgnoreDataMember]
        public DateTime? CreatedOn
        {
            get
            {
                if (_createdDate == null)
                    return DateTime.Now;
                else
                    return _createdDate;
            }
            set { _createdDate = value; }
        }


        private DateTime? _modifiedDate;
        [IgnoreDataMember]
        public DateTime? ModifiedOn
        {
            get
            {
                _modifiedDate = DateTime.Now;
                return _modifiedDate;
               
            }
            set { _modifiedDate = value; }
        }


    }
}
