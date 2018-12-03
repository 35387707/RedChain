using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelaxBarWeb_MVC.Utils
{
    public class MyLinkedList<T>: System.Collections.Generic.LinkedList<T>
    {
        private object sync = new object();
        public new void AddLast(LinkedListNode<T> node)
        {
            lock (sync) {
                base.AddLast(node);
            }
        }
    }
}