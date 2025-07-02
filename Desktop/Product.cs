using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    internal class Product
    {
        public Product() { }
        public Product(string name, int price)
        {
            this.name = name;
            this.price = price;
        }
        public int id { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public int categoryId { get; set; }
        public int subcategoryId { get; set; }
        public string unit { get; set; }
        public int price { get; set; }
        public int stockQuantity { get; set; }
        public int minstockLevel { get; set; }
        public bool isActive { get; set; }
        public string message { get; set; }
    }
}
