using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
        public class SendUser
        {
            public string Username { get; set; }
        }

        public class ReceiveCase
        {
            public List<string> Cases { get; set; }
        }
}