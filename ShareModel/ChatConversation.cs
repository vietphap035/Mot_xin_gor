using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModel
{
    public class ChatConversation
    {
        public string rId { get; set; }
        public string displayName { get; set; }
        public string lastMessage { get; set; }  
        public string timeAgo { get; set; }     
    }
}
