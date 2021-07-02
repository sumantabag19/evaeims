using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class ModuleAccessDetails
    {
        [Key]
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool ReadAccess { get; set; }
        public bool WriteAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool DeleteAccess { get; set; }
        public bool? IsActive { get; set; }

    }
}
