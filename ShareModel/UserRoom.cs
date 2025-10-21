using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModel
{
    public class UserRoom
    {
        public string UId { get; set; }
        public virtual User User { get; set; }
        public string RId { get; set; }
        public virtual Room Room { get; set; }
    }
}
