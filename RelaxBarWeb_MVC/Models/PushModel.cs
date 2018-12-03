using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelaxBarWeb_MVC.Models
{
    public class PushModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class AreaModels
    {
        private int _id = 0;
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _value = string.Empty;
        public string value
        {
            get { return _value; }
            set { _value = value; }
        }
        private List<AreaModels> _childs = new List<AreaModels>();
        public List<AreaModels> childs
        {
            get { return _childs; }
            set { _childs = value; }
        }
    }
}