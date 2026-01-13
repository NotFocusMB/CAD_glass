using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Parameter
    {
        private double _minValue;
        private double _maxValue;
        private double _value;
        public double MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                Validate(value);
                _value = value;
            }
        }

        private void Validate(double value)
        {
            if (value < MinValue)
            {
                throw new ArgumentException("Value_small");
            }
            if (value > MaxValue)
            {
                throw new ArgumentException("Value_TooBig");
            }
        }
    }
}
