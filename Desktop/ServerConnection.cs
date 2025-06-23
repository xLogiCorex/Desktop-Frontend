using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Desktop
{
    public class ServerConnection
    {
        private HttpClient client = new HttpClient();
        private string baseURL = "";
        public ServerConnection(string url)
        {
            baseURL = url;
        }
    }

}
