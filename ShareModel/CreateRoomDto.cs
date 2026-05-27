using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModel
{
    public class CreateRoomDto
    {
        public string RoomName { get; set; }
        public List<string> UserRooms { get; set; }
    }
}
