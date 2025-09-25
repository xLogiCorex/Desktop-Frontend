using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class Orders
    {
        public Orders() { }
        public Orders(string orderNumber, string partnerName, string userName, DateTime date, string status, string invoiceNumber)
        {
            this.orderNumber = orderNumber;
            this.partnerName = partnerName;
            this.userName = userName;
            this.date = date;
            this.status = status;
            this.invoiceNumber = invoiceNumber;
        }
        public int id { get; set; }
        public string orderNumber { get; set; }
        public int partnerId { get; set; }
        public string partnerName { get; set; }
        public Guid userId { get; set; }
        public string userName { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public int? invoiceId { get; set; }
        public string invoiceNumber { get; set; }
    }
}
