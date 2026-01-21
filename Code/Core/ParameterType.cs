using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Определяет типы числовых параметров, используемых в модели бокала.
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Высота ножки бокала.
        /// </summary>
        StalkHeight,

        /// <summary>
        /// Высота стенки чаши бокала.
        /// </summary>
        SideHeight,

        /// <summary>
        /// Радиус чаши бокала.
        /// </summary>
        BowlRadius,

        /// <summary>
        /// Радиус ножки бокала.
        /// </summary>
        StalkRadius,

        /// <summary>
        /// Угол наклона стенки чаши бокала.
        /// </summary>
        SideAngle,

        /// <summary>
        /// Радиус основания бокала.
        /// </summary>
        StandRadius
    }
}
