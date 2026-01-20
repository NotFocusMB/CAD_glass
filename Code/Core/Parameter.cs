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
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если minValue больше maxValue.
        /// </exception>
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если значение находится вне допустимого диапазона.
        /// </exception>
        public double Value
        {
            get
            {
                Validate(_value, nameof(Value));
                return _value;
            }
            set
            {
                Validate(value, nameof(value));
                _value = value;
            }
        }

        /// <summary>
        /// Проверяет, находится ли значение в допустимом диапазоне.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="paramName">Имя параметра для сообщения об ошибке.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если значение вне диапазона.
        /// </exception>
        private void Validate(double value, string paramName)
        {
            if (value < _minValue)
            {
                throw new ArgumentOutOfRangeException(paramName,
                    $"Значение ({value}) не может быть меньше"
                    + $" минимального ({_minValue}).");
            }
            if (value > _maxValue)
            {
                throw new ArgumentOutOfRangeException(paramName,
                    $"Значение ({value}) не может быть больше"
                    + $" максимального ({_maxValue}).");
            }
        }
    }
}