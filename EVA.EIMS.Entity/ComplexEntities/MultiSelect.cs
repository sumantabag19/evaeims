using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class MultiSelect
    {
        [Key]
        public int id { get; set; }
        public string text { get; set; }
        public bool? selected { get; set; }
    }
}
