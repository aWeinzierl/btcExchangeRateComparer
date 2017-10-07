using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace btcExchangeRateComparer
{
    public delegate void TriggeredFunction();

    public class BoundaryTrigger<T> where T : IComparable<T>
    {
        private TriggeredFunction _triggeredFunction;
        private bool _triggered;
        private T _lowerBoundary;
        private T _upperBoundary;

        public T LowerBoundary
        {
            get { return _lowerBoundary; }
            set
            {
                _triggered = false;
                _lowerBoundary = value;
            }
        }
        public T UpperBoundary
        {
            get { return _upperBoundary; }
            set
            {
                _triggered = false;
                _upperBoundary = value;
            }
        }

        public BoundaryTrigger(TriggeredFunction triggeredFunction, T lowerBoundary, T upperBoundary)
        {
            _triggeredFunction = triggeredFunction;
            _triggered = false;
        }

        public void Report(T value)
        {
            if (_triggered) { return; }
            if (value.CompareTo(UpperBoundary) <= 0 && value.CompareTo(LowerBoundary) >= 0) return;

            _triggered = true;
            _triggeredFunction();
        }
    }

}
