using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class IPAddressRange
    {
        [Key]
        public string IPStartAddress { get; set; }
        public string IPEndAddress { get; set; }
    }
}
