using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    public class SubCategory
    {
        public SubCategory() { }
        public SubCategory(string name, int categoryId)
        {
            this.name = name;
        }
        public int id { get; set; }
        public string name { get; set; }
        public int categoryId { get; set; }
    }
}
