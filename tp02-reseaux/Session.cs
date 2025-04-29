using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp02_reseaux
{
    internal class Session
    {
        private string id;
        public Dictionary<string, string> data { get; set; }

        public Session(string id)
        {
            this.id = id;
            data = new Dictionary<string, string>();
        }
    }
}
