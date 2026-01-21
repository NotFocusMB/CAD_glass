using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    /// <summary>
    /// Управляет всеми параметрами модели бокала, их значениями,
    /// ограничениями и сложными взаимозависимостями.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Основной словарь, хранящий текущие объекты параметров модели.
        /// </summary>
        public Dictionary<ParameterType, Parameter> NumericalParameters { get; set; }

        // Приватный словарь-эталон для хранения "заводских" настроек.
        private readonly Dictionary<ParameterType,
            (double Min, double Max, double Default)> _initialLimits;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameters.
        /// </summary>
        public Parameters()
        {
            InitializeParameters();
            SetDefaultValues();

            // Кэшируем "заводские" Min/Max и дефолтные значения.
            // Это должно быть сделано прямо в конструкторе из-за 'readonly'.
            _initialLimits = NumericalParameters.ToDictionary(
                kvp => kvp.Key,
                kvp => (kvp.Value.MinValue, kvp.Value.MaxValue, kvp.Value.Value)
            );

            UpdateAllLimits();
        }

        /// <summary>
        /// Обновляет значение параметра и пересчитывает зависимые ограничения.
        /// </summary>
        /// <param name="changedValue">Новое значение параметра.</param>
        /// <param name="changedType">Тип измененного параметра.</param>
        public void SetDependencies(double changedValue, ParameterType changedType)
        {
            try
            {
                NumericalParameters[changedType].Value = changedValue;
                UpdateAllLimits();
            }
            catch (ArgumentException)
            {
                // Пробрасываем исключение валидации из класса Parameter,
                // сохраняя исходный стек вызовов.
                throw;
            }
        }

        /// <summary>
        /// Сбрасывает все значения и ограничения к начальным настройкам.
        /// </summary>
        public void ResetToDefaults()
        {
            ResetLimitsToInitial();
            SetDefaultValues();
            UpdateAllLimits();
        }

        /// <summary>
        /// Возвращает строку с текущими ограничениями для параметра.
        /// </summary>
        /// <param name="type">Тип параметра.</param>
        /// <returns>Форматированная строка (например, "10.5 - 50.0").</returns>
        public string GetLimitsString(ParameterType type)
        {
            if (NumericalParameters.ContainsKey(type))
            {
                var param = NumericalParameters[type];
                // Добавляем проверку на несовместимость
                if (param.MinValue > param.MaxValue ||
                    (param.MinValue > _initialLimits[type].Max) ||
                    (param.MaxValue < _initialLimits[type].Min))
                    return "Конфликт!";

                return $"{param.MinValue:F1} - {param.MaxValue:F1}";
            }
            return "N/A";
        }

        /// <summary>
        /// Инициализирует словарь параметров с "заводскими" Min/Max.
        /// </summary>
        private void InitializeParameters()
        {
            // Используем новый конструктор класса Parameter
            var stalkHeight = new Parameter(50, 100);
            var sideHeight = new Parameter(1, 50);
            var bowlRadius = new Parameter(25, 50);
            var stalkRadius = new Parameter(1, 3);
            var sideAngle = new Parameter(0, 15);
            var standRadius = new Parameter(20, 75);

            NumericalParameters = new Dictionary<ParameterType, Parameter>()
            {
                [ParameterType.StalkHeight] = stalkHeight,
                [ParameterType.SideHeight] = sideHeight,
                [ParameterType.BowlRadius] = bowlRadius,
                [ParameterType.StalkRadius] = stalkRadius,
                [ParameterType.SideAngle] = sideAngle,
                [ParameterType.StandRadius] = standRadius,
            };
        }

        /// <summary>
        /// Устанавливает начальные значения по умолчанию.
        /// </summary>
        private void SetDefaultValues()
        {
            NumericalParameters[ParameterType.StalkHeight].Value = 75;
            NumericalParameters[ParameterType.SideHeight].Value = 40;
            NumericalParameters[ParameterType.BowlRadius].Value = 35;
            NumericalParameters[ParameterType.StalkRadius].Value = 2;
            NumericalParameters[ParameterType.SideAngle].Value = 10;
            NumericalParameters[ParameterType.StandRadius].Value = 30;
        }

        /// <summary>
        /// Пересчитывает все динамические ограничения.
        /// </summary>
        private void UpdateAllLimits()
        {
            ResetLimitsToInitial();
            ApplyStandBowlRadiusDependency();
            ApplySideHeightAngleDependency();
        }

        /// <summary>
        /// Сбрасывает все лимиты к "заводским" перед каждым пересчетом.
        /// </summary>
        private void ResetLimitsToInitial()
        {
            foreach (var kvp in _initialLimits)
            {
                var param = NumericalParameters[kvp.Key];
                param.MinValue = kvp.Value.Min;
                param.MaxValue = kvp.Value.Max;
            }
        }

        /// <summary>
        /// Применяет зависимость: StandRadius >= 2/3 * BowlRadius.
        /// </summary>
        private void ApplyStandBowlRadiusDependency()
        {
            double bowlRadius = 
                NumericalParameters[ParameterType.BowlRadius].Value;
            double standRadius = 
                NumericalParameters[ParameterType.StandRadius].Value;

            var standRadiusParam = 
                NumericalParameters[ParameterType.StandRadius];
            standRadiusParam.MinValue = Math.Max(standRadiusParam.MinValue,
                (2.0 / 3.0) * bowlRadius);

            var bowlRadiusParam = NumericalParameters[ParameterType.BowlRadius];
            bowlRadiusParam.MaxValue = Math.Min(bowlRadiusParam.MaxValue,
                1.5 * standRadius);
        }

        /// <summary>
        /// Применяет зависимость: SideHeight * sin(SideAngle) <= BowlRadius / 2.
        /// </summary>
        private void ApplySideHeightAngleDependency()
        {
            double sideHeight = 
                NumericalParameters[ParameterType.SideHeight].Value;
            double bowlRadius = 
                NumericalParameters[ParameterType.BowlRadius].Value;
            double sideAngle = 
                NumericalParameters[ParameterType.SideAngle].Value;
            double angleRad = sideAngle * Math.PI / 180.0;

            // Ограничение для SideHeight
            if (Math.Sin(angleRad) > 0.001)
            {
                var sideHeightParam = 
                    NumericalParameters[ParameterType.SideHeight];
                var maxSideHeight = (bowlRadius / 2.0) / Math.Sin(angleRad);
                sideHeightParam.MaxValue = Math.Min(sideHeightParam.MaxValue,
                    maxSideHeight);
            }

            // Ограничение для SideAngle
            if (sideHeight > 0)
            {
                var sinMaxAngle = (bowlRadius / 2.0) / sideHeight;
                if (sinMaxAngle <= 1 && sinMaxAngle > 0)
                {
                    var sideAngleParam = 
                        NumericalParameters[ParameterType.SideAngle];
                    var maxAngle = Math.Asin(sinMaxAngle) * 180.0 / Math.PI;
                    sideAngleParam.MaxValue = Math.Min(sideAngleParam.MaxValue,
                        maxAngle);
                }
            }

            // Ограничение для BowlRadius
            var bowlRadiusParam = 
                NumericalParameters[ParameterType.BowlRadius];
            var minBowlRadius = 2.0 * sideHeight * Math.Sin(angleRad);
            bowlRadiusParam.MinValue = Math.Max(bowlRadiusParam.MinValue,
                minBowlRadius);
        }
    }
}
