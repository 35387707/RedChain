using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelaxBarWeb_MVC.Utils
{
    public static class MyExtend
    {
        public static bool CheckString(this string str) {
            if (str.IndexOf("|") != -1 || str.IndexOf(">") != -1 || str.IndexOf("<") != -1 || str.IndexOf("&") != -1 || str.IndexOf("'") != -1 || str.IndexOf("\"") != -1 || str.IndexOf("\\") != -1)
            {
                return false;
            }
            return true;
        }
    }
}