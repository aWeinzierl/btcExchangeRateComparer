using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseAlarm
{
    public class CurrencyExchangeRate
    {
        public string From { get; }
        public string To { get; }
        public decimal Value { get;}
        public CurrencyExchangeRate(string from, string to, decimal value)
        {
            From = from;
            To = to;
            Value = value;
        }
    }
}
