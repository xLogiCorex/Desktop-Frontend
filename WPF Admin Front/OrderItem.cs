using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class OrderItem
    {
        public OrderItem() { }
        public OrderItem(string productName, int quantity, decimal unitPrice, decimal totalPrice)
        {
            this.productName = productName;
            this.quantity = quantity;
            this.unitPrice = unitPrice;
            this.totalPrice = totalPrice;
        }
        public int id { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; } 
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
        public decimal totalPrice { get; set; }
    }
}
