using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.AutoLogin]
    [Filter.CheckLogin]
    public class GroupController : BaseController
    {
       
    }
}