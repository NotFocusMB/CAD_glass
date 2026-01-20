using System;

namespace Core
{
    /// <summary>
    /// Представляет числовой параметр с границами (минимум и максимум).
    /// </summary>
    public class Parameter
    {
        private double _minValue;
        private double _maxValue;
        private double _value;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameter.
        /// </summary>
        /// <param name="minValue">Начальное минимальное значение.</param>
        /// <param name="maxValue">Начальное максимальное значение.</param>
        public Parameter(double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("Начальное минимальное значение"
                    + " не может быть больше максимального.");
            }
            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <summary>
        /// Минимально допустимое значение.
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                if (value > _maxValue)
                {
                    throw new ArgumentException("Минимальное значение не может"
                        + " быть больше максимального.");
                }
                _minValue = value;
            }
        }

        /// <summary>
        /// Максимально допустимое значение.
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (value < _minValue)
                {
                    throw new ArgumentException("Максимальное значение не может"
                        + " быть меньше минимального.");
                }
                _maxValue = value;
            }
        }

        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                Validate(value);
                _value = value;
            }
        }

        /// <summary>
        /// Проверяет, находится ли значение в допустимом диапазоне.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        private void Validate(double value)
        {
            if (value < MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Значение ({value}) не может быть меньше"
                    + $" минимального ({MinValue}).");
            }
            if (value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Значение ({value}) не может быть больше"
                    + $" максимального ({MaxValue}).");
            }
        }
    }
}
