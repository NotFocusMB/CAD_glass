using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Parameters
    {
        /// <summary>
        /// Конструктор класса Parameter.
        /// Содержит инициализацию численных параметров гранёного стакана
        /// </summary>
        public Parameters()
        {
            //Инициализация параметра длины гранёного стакана
            Parameter stalkheight = new Parameter();
            stalkheight.MinValue = 75;
            stalkheight.MaxValue = 150;

            //Инициализация параметра радиуса внешней окружности гранёного стакана
            Parameter SideHeight = new Parameter();
            SideHeight.MinValue = 0;
            SideHeight.MaxValue = 100;

            //Инициализация высоты дна гранёного стакана
            Parameter bowlradius = new Parameter();
            bowlradius.MinValue = 30;
            bowlradius.MaxValue = 60;

            //Инициализация параметра толщины нижней грани гранёного стакана
            Parameter stalkradius = new Parameter();
            stalkradius.MinValue = 1;
            stalkradius.MaxValue = 4;

            //Инициализация параметра толщины верхней грани гранёного стакана
            Parameter sideangle = new Parameter();
            sideangle.MinValue = 0;
            sideangle.MaxValue = 17.5;

            //Инициализация параметра высоты верхней грани гранёного стакана
            Parameter standradius = new Parameter();
            standradius.MinValue = 45;
            standradius.MaxValue = 90;

            //Занесения значений параметров в словарь с соотвествующими ключами
            NumericalParameters = new Dictionary<ParameterType, Parameter>()
            {
                [ParameterType.StalkHeight] = stalkheight,
                [ParameterType.SideHeight] = SideHeight,
                [ParameterType.BowlRadius] = bowlradius,
                [ParameterType.StalkRadius] = stalkradius,
                [ParameterType.SideAngle] = sideangle,
                [ParameterType.StandRadius] = standradius,
            };

        }
        public Dictionary<ParameterType, Parameter> NumericalParameters { get; set; }

        /// <summary>
        /// Выставляет максимальное и минимальное(Если такое задано) значения для параметра
        /// </summary>
        /// <param name="independ"> Параметр на основе которого будет вычислятся макс и мин значения</param>
        /// <param name="depend"> Параметр к которому будет примернятся максимально и минимальное значение</param>
        /// <param name="maxratio"> Соотношение велечин для максимального значения</param>
        /// <param name="minratio">Соотношение велечин для минимального значения</param>
        public void SetDependenses(double changedValue, ParameterType changedType)
        {
            switch (changedType)
            {
                case ParameterType.BowlRadius:
                    // Зависимость 1: BowlRadius <= 2/3 * StandRadius
                    // Значит StandRadius >= 1.5 * BowlRadius
                    var standRadius1 = NumericalParameters[ParameterType.StandRadius];
                    double minStandRadius = changedValue * 1.5;

                    if (minStandRadius > standRadius1.MinValue)
                    {
                        standRadius1.MinValue = minStandRadius;
                    }

                    // Зависимость 2: sin(SideAngle) * SideHeight <= BowlRadius / 2
                    // Проверяем, есть ли два других параметра
                    TryUpdateTriangularLimits();
                    break;

                case ParameterType.StandRadius:
                    // Обратная зависимость: BowlRadius <= 2/3 * StandRadius
                    var bowlRadius = NumericalParameters[ParameterType.BowlRadius];
                    double maxBowlRadius = changedValue * (2.0 / 3.0);

                    if (maxBowlRadius < bowlRadius.MaxValue)
                    {
                        bowlRadius.MaxValue = maxBowlRadius;
                    }
                    break;

                case ParameterType.SideAngle:
                case ParameterType.SideHeight:
                    // Зависимость 2: sin(SideAngle) * SideHeight <= BowlRadius / 2
                    TryUpdateTriangularLimits();
                    break;
            }
        }

        /// <summary>
        /// Пытается обновить ограничения для трех взаимозависимых параметров
        /// Работает только если есть хотя бы 2 значения из 3
        /// </summary>
        private void TryUpdateTriangularLimits()
        {
            // Получаем параметры
            var bowlRadius = NumericalParameters[ParameterType.BowlRadius];
            var sideAngle = NumericalParameters[ParameterType.SideAngle];
            var sideHeight = NumericalParameters[ParameterType.SideHeight];

            // Проверяем, какие значения установлены
            bool hasBowl = TryGetValue(bowlRadius, out double bowlValue);
            bool hasAngle = TryGetValue(sideAngle, out double angleValue);
            bool hasHeight = TryGetValue(sideHeight, out double heightValue);

            // Если есть хотя бы 2 значения
            if ((hasBowl && hasAngle) || (hasBowl && hasHeight) || (hasAngle && hasHeight))
            {
                // Рассчитываем максимальное произведение
                double maxProduct = 0;

                if (hasBowl)
                {
                    maxProduct = bowlValue / 2.0;
                }
                else if (hasAngle && hasHeight)
                {
                    // Вычисляем bowlValue из формулы: bowlValue = 2 * sin(angle) * height
                    double angleRad = angleValue * Math.PI / 180.0;
                    double sinAngle = Math.Sin(angleRad);
                    bowlValue = 2 * sinAngle * heightValue;
                    maxProduct = bowlValue / 2.0;
                }

                // Обновляем ограничения
                if (hasAngle && hasHeight)
                {
                    // Ограничиваем SideHeight
                    double angleRad = angleValue * Math.PI / 180.0;
                    double sinAngle = Math.Sin(angleRad);

                    if (sinAngle > 0)
                    {
                        double maxHeight = maxProduct / sinAngle;
                        if (maxHeight < sideHeight.MaxValue)
                        {
                            sideHeight.MaxValue = maxHeight;
                        }
                    }

                    // Ограничиваем SideAngle
                    if (heightValue > 0)
                    {
                        double maxSinAngle = maxProduct / heightValue;
                        if (maxSinAngle <= 1.0)
                        {
                            double maxAngleRad = Math.Asin(maxSinAngle);
                            double maxAngleDeg = maxAngleRad * 180.0 / Math.PI;

                            if (maxAngleDeg < sideAngle.MaxValue)
                            {
                                sideAngle.MaxValue = maxAngleDeg;
                            }
                        }
                    }
                }
                else if (hasBowl && hasAngle)
                {
                    // Ограничиваем только SideHeight
                    double angleRad = angleValue * Math.PI / 180.0;
                    double sinAngle = Math.Sin(angleRad);

                    if (sinAngle > 0)
                    {
                        double maxHeight = maxProduct / sinAngle;
                        if (maxHeight < sideHeight.MaxValue)
                        {
                            sideHeight.MaxValue = maxHeight;
                        }
                    }
                }
                else if (hasBowl && hasHeight)
                {
                    // Ограничиваем только SideAngle
                    if (heightValue > 0)
                    {
                        double maxSinAngle = maxProduct / heightValue;
                        if (maxSinAngle <= 1.0)
                        {
                            double maxAngleRad = Math.Asin(maxSinAngle);
                            double maxAngleDeg = maxAngleRad * 180.0 / Math.PI;

                            if (maxAngleDeg < sideAngle.MaxValue)
                            {
                                sideAngle.MaxValue = maxAngleDeg;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Пытается получить значение параметра
        /// </summary>
        private bool TryGetValue(Parameter param, out double value)
        {
            try
            {
                value = param.Value;
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Возвращает строку ограничений для параметра
        /// </summary>
        public string GetLimitsString(ParameterType type)
        {
            if (NumericalParameters.ContainsKey(type))
            {
                var param = NumericalParameters[type];
                return $"{param.MinValue:F1} - {param.MaxValue:F1}";
            }
            return "N/A";
        }
    }
}
