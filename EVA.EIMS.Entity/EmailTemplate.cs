using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class EmailTemplate
    {
        [Key]
        public int EmailTemplateId { get; set; }
        public string EmailTemplateName { get; set; }
        [StringLength(160)]
        [DataType(DataType.EmailAddress)]
        public string EmailFrom { get; set; }
        [StringLength(250)]
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailConfidentialMsg { get; set; }
        public string EmailFooter { get; set; }
        public Guid LanguageId { get; set; }
        public int AppId { get; set; }
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


        public virtual Language Language { get; set; }
        public virtual Application Application { get; set; }
    }
}
