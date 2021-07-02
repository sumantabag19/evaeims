using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class UserAnswer
    {
        [Key]
        public int AnswerId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [NotMapped]
        public int UpdatedQuestionId { get; set; }
        [Required]
        [StringLength(100)]
        public string UserAnswerText { get; set; }
        [Required]
        public Guid UserId { get; set; }
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

        public virtual User User { get; set; }
        public virtual SecurityQuestion Question { get; set; }


    }
}
