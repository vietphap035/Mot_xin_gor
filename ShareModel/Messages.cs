using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModel
{
    public class Messages
    {
        [Key]
        public string MId { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public string UId { get; set; }
        [ForeignKey("UId")]
        public virtual User User { get; set; }

        [Required]
        public string RId { get; set; }
        [ForeignKey("RId")]
        public virtual Room Room { get; set; }
    }
}
