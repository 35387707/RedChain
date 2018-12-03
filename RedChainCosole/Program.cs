using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarBLL;
namespace RedChainCosole
{
    class Program
    {
        static void Main(string[] args)
        {
            RedPacksBLL pbll = new RedPacksBLL();
            pbll.SendTimeOutRedpack();//发送
        }
    }
}
