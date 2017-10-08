using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace btcExchangeRateComparer
{
    public class ViewModel
    {
        public string SelectedPath { get; set; }
        public TimeSpan CheckingIntervallBitfinex { get; set; }
        public TimeSpan CheckingIntervallKraken { get; set; }
        public decimal MaxDifferenceKrakenToBitfinex { get; set; }
        public decimal MaxDifferenceBitfinexToKraken { get; set; }
        public decimal CurrentKrakenRate { get; set; }
        public decimal CurrentBitfinexRate { get; set; }
        public bool AlarmActive { get; set; }

        public ViewModel(
            decimal currentKrakenRate,
            decimal currentBitfinexRate,
            decimal maxDifferenceKrakenToBitfinex,
            decimal maxDifferenceBitfinexToKrakenm,
            string path,
            TimeSpan intervallKraken,
            TimeSpan intervallBitfinex,
            bool alarmActive)
        {
            AlarmActive = alarmActive;
            SelectedPath = path;
            CheckingIntervallBitfinex = intervallBitfinex;
            CheckingIntervallKraken = intervallKraken;
            MaxDifferenceBitfinexToKraken = maxDifferenceBitfinexToKrakenm;
            MaxDifferenceKrakenToBitfinex = maxDifferenceKrakenToBitfinex;
            CurrentKrakenRate = currentKrakenRate;
            CurrentBitfinexRate = currentBitfinexRate;
        }
    }
}