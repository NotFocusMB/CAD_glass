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

        // Приватный словарь-эталон для хранения "заводских" настроек (Min, Max, Default).
        // Используется для сброса и предотвращения "сжимающихся" ограничений.
        private readonly Dictionary<ParameterType, (double Min, double Max, double Default)> _initialLimits;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameters,
        /// устанавливает начальные значения и ограничения для всех параметров.
        /// </summary>
        public Parameters()
        {
            // --- Инициализация объектов Parameter с "заводскими" Min/Max ---
            Parameter stalkheight = new Parameter { MinValue = 50, MaxValue = 100 };
            Parameter SideHeight = new Parameter { MinValue = 0, MaxValue = 50 };
            Parameter bowlradius = new Parameter { MinValue = 25, MaxValue = 50 };
            Parameter stalkradius = new Parameter { MinValue = 1, MaxValue = 3 };
            Parameter sideangle = new Parameter { MinValue = 0, MaxValue = 15 };
            Parameter standradius = new Parameter { MinValue = 20, MaxValue = 75 };

            NumericalParameters = new Dictionary<ParameterType, Parameter>()
            {
                [ParameterType.StalkHeight] = stalkheight,
                [ParameterType.SideHeight] = SideHeight,
                [ParameterType.BowlRadius] = bowlradius,
                [ParameterType.StalkRadius] = stalkradius,
                [ParameterType.SideAngle] = sideangle,
                [ParameterType.StandRadius] = standradius,
            };

            // --- Установка начальных значений по умолчанию ---
            NumericalParameters[ParameterType.StalkHeight].Value = 75;
            NumericalParameters[ParameterType.SideHeight].Value = 40;
            NumericalParameters[ParameterType.BowlRadius].Value = 35;
            NumericalParameters[ParameterType.StalkRadius].Value = 2;
            NumericalParameters[ParameterType.SideAngle].Value = 10;
            NumericalParameters[ParameterType.StandRadius].Value = 30;

            // --- Создание "Эталона" ---
            // Копируем "заводские" Min/Max и текущие значения (как дефолтные) в словарь-хранилище.
            _initialLimits = NumericalParameters.ToDictionary(
                kvp => kvp.Key,
                kvp => (kvp.Value.MinValue, kvp.Value.MaxValue, kvp.Value.Value)
            );

            // Сразу применяем все зависимости, чтобы скорректировать лимиты на основе начальных значений.
            UpdateAllLimits();
        }

        /// <summary>
        /// Обновляет значение указанного параметра и пересчитывает все зависимые ограничения.
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
            catch (ArgumentException ex)
            {
                throw ex; // Пробрасываем исключение валидации из класса Parameter.
            }
        }

        /// <summary>
        /// Полностью сбрасывает все значения и динамические ограничения к начальным "заводским" настройкам.
        /// </summary>
        public void ResetToDefaults()
        {
            // ШАГ 1: Сначала восстанавливаем диапазоны MinValue/MaxValue из эталона.
            foreach (var kvp in _initialLimits)
            {
                var param = NumericalParameters[kvp.Key];
                param.MinValue = kvp.Value.Min;
                param.MaxValue = kvp.Value.Max;
            }

            // ШАГ 2: Только теперь, когда диапазоны широкие, устанавливаем значения по умолчанию.
            foreach (var kvp in _initialLimits)
            {
                NumericalParameters[kvp.Key].Value = kvp.Value.Default;
            }

            // Пересчитываем лимиты на основе сброшенных значений.
            UpdateAllLimits();
        }

        /// <summary>
        /// Приватный метод, который пересчитывает все динамические ограничения,
        /// выбирая самое строгое из "заводского" и вычисленного геометрически.
        /// </summary>
        private void UpdateAllLimits()
        {
            //Сбрасываем все лимиты к "заводским" перед каждым пересчетом.
            foreach (var kvp in _initialLimits)
            {
                var param = NumericalParameters[kvp.Key];
                param.MinValue = kvp.Value.Min;
                param.MaxValue = kvp.Value.Max;
            }

            // Получаем текущие значения для расчетов.
            double sideHeight = NumericalParameters[ParameterType.SideHeight].Value;
            double bowlRadius = NumericalParameters[ParameterType.BowlRadius].Value;
            double sideAngle = NumericalParameters[ParameterType.SideAngle].Value;
            double standRadius = NumericalParameters[ParameterType.StandRadius].Value;

            // --- Применение геометрических зависимостей ---

            // ЗАВИСИМОСТЬ 1: StandRadius >= 2/3 * BowlRadius
            var standRadiusParam = NumericalParameters[ParameterType.StandRadius];
            standRadiusParam.MinValue = Math.Max(standRadiusParam.MinValue, (2.0 / 3.0) * bowlRadius);

            var bowlRadiusParam = NumericalParameters[ParameterType.BowlRadius];
            bowlRadiusParam.MaxValue = Math.Min(bowlRadiusParam.MaxValue, 1.5 * standRadius);

            // ЗАВИСИМОСТЬ 2: SideHeight * sin(SideAngle) <= BowlRadius / 2
            double angleRad = sideAngle * Math.PI / 180.0;

            // Ограничение для SideHeight
            if (Math.Sin(angleRad) > 0.001)
            {
                var sideHeightParam = NumericalParameters[ParameterType.SideHeight];
                sideHeightParam.MaxValue = Math.Min(sideHeightParam.MaxValue, (bowlRadius / 2.0) / Math.Sin(angleRad));
            }

            // Ограничение для SideAngle
            if (sideHeight > 0)
            {
                double sinMaxAngle = (bowlRadius / 2.0) / sideHeight;
                if (sinMaxAngle <= 1 && sinMaxAngle > 0)
                {
                    var sideAngleParam = NumericalParameters[ParameterType.SideAngle];
                    sideAngleParam.MaxValue = Math.Min(sideAngleParam.MaxValue, Math.Asin(sinMaxAngle) * 180.0 / Math.PI);
                }
            }

            // Ограничение для BowlRadius
            bowlRadiusParam.MinValue = Math.Max(bowlRadiusParam.MinValue, 2.0 * sideHeight * Math.Sin(angleRad));
        }

        /// <summary>
        /// Возвращает строку с текущими минимальным и максимальным ограничениями для указанного параметра.
        /// </summary>
        /// <param name="type">Тип параметра.</param>
        /// <returns>Форматированная строка (например, "10,5 - 50,0") или "Конфликт!".</returns>
        public string GetLimitsString(ParameterType type)
        {
            if (NumericalParameters.ContainsKey(type))
            {
                var param = NumericalParameters[type];
                if (param.MinValue > param.MaxValue) return "Конфликт!";
                return $"{param.MinValue:F1} - {param.MaxValue:F1}";
            }
            return "N/A";
        }
    }
}
