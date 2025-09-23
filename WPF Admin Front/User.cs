using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class User
    {
        public User() { }
        public User(string name, string email, string password, string role)
        {
            this.name = name;
            this.email = email;
            this.password = password;
            this.role = role;
        }
        public Guid id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public bool isActive { get; set; }
        public string message { get; set; }
    }
}
