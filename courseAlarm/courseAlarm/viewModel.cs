using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace courseAlarm
{
    public class ViewModel
    {
        public BoundaryTrigger<decimal> BtcUsdTrigger { get; }
        public BoundaryTrigger<decimal> XbtUsdTrigger { get;}
        public string SelectedPath { get; set; }
        public TimeSpan CheckingIntervallTimeSpan { get; set; }

        public ViewModel(
            BoundaryTrigger<decimal> btcUsd, 
            BoundaryTrigger<decimal> xbtUsd, 
            string path, TimeSpan intervall)
        {
            BtcUsdTrigger = btcUsd;
            XbtUsdTrigger = xbtUsd;
            SelectedPath = path;
            CheckingIntervallTimeSpan = intervall;
        }
    }
}