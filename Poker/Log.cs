using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker
{
    [Serializable]
    public class Log
    {
        public Log(string testo)
        {
            Data = DateTime.Now;
            Testo = testo;
        }
        public DateTime Data { get; set; }
        public string Testo { get; set; }
    }
}
