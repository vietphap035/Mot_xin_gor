using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModel
{
    public class Room
    {
        [Key]
        public string RId { get; set; }
        public string RoomName { get; set; }

        public virtual ICollection<UserRoom> UserRooms { get; set; }
        public virtual ICollection<Messages> Messages { get; set; }
    }
}
