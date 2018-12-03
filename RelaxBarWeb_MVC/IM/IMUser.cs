using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Web;

namespace RelaxBarWeb_MVC.IM
{
    public class IMUser
    {
        public WebSocket socket { get; set; }
        public string token { get; set; }
    }
}