using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
    public class AuditTable
    {
        public AuditTable()
        {
            this.IsActive = true;

        }
        [Key]
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Updatedby { get; set; } = "";
        public DateTime UpdatedOn { get; set; } = new DateTime();
        public bool IsActive { get; set; }
    }
}
