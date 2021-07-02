using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{
    public class Language
    {
        [Key]
        public Guid LanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }
}


