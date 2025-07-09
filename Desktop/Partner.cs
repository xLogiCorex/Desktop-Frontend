using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    internal class Partner
    {
        public Partner() { }
        public Partner(string name, string email, string phone)
        {
            this.name = name;
            this.email = email;
            this.phone = phone;
        }
        public int id { get; set; }
        public string name { get; set; }
        public string taxNumber { get; set; }
        public string address { get; set; }
        public string contactPerson { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool isActive { get; set; }
        public string message { get; set; }
    }
}
