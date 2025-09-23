using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class Invoices
    {
        public Invoices() { }
        public Invoices(string invoiceNumber, string orderNumber, string partnerName, string username, DateTime issueDate, decimal totalNet, decimal totalGross)
        {
            this.invoiceNumber = invoiceNumber;
            this.orderNumber = orderNumber;
            this.partnerName = partnerName;
            this.username = username;
            this.issueDate = issueDate;
            this.totalNet = totalNet;
            this.totalGross = totalGross;
            this.orderId = orderId;
        }
        public int id { get; set; }
        public string invoiceNumber { get; set; }
        public int orderId { get; set; }
        public string orderNumber { get; set; }
        public int partnerId { get; set; }
        public string partnerName { get; set; }
        public Guid userId { get; set; }
        public string username { get; set; }
        public DateTime issueDate { get; set; }
        public string items { get; set; }
        public decimal totalNet { get; set; }
        public decimal totalVat { get; set; }
        public decimal totalGross { get; set; }
        public string note { get; set; }
    }
}
