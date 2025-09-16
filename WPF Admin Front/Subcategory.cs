using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class Subcategory
    {
        public Subcategory() { }
        public Subcategory(string name, int categoryId)
        {
            this.name = name;
        }
        public int id { get; set; }
        public string name { get; set; }
        public int categoryId { get; set; }
    }
}
