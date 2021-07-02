using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class ApplicationUserMappingModel
    {
        public int MappingId {get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int [] AppId { get; set; }
        public bool? IsActive { get; set; }
    }
}
