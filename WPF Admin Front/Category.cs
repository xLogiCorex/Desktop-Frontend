using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class Category
    {
        public Category() { }
        public Category(string name)
        {
            this.name = name;
        }
        public int id { get; set; }
        public string name { get; set; }
    }
}
