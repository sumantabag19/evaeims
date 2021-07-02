using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class SecurityQuestion
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid question id"))]
        public int QuestionId { get; set; }
        [Required]
        public string Question { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
        [Required]
        public bool? IsActive { get; set; }

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
