using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WPF_Admin_Front
{
    public class Logs
    {
        public Logs() { }
        public int id { get; set; }
        public Guid userId { get; set; }
        public string action { get; set; }
        public string targetType { get; set; }
        public int targetId { get; set; }
        public string payload { get; set; }
        public DateTime createdAt { get; set; }
        public string username { get; set; }
    }
}
