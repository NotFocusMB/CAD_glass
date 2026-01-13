using Builder;
using Core;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;
using System;

namespace GlassPlugin
{
    public class GlassBuilder
    {
        private KompasWrapper _wrapper;

        /// <summary>
        /// Функция, запускающая и проводящая процесс постройки бокала
        /// </summary>
        /// <param name="parameters">Параметры для постройки</param>
        public void BuildGlass(Parameters parameters)
        {
            try
            {
                Console.WriteLine("=== НАЧАЛО ПОСТРОЕНИЯ БОКАЛА ===");

                _wrapper = new KompasWrapper();

                // 1. Подключение к КОМПАС
                Console.WriteLine("1. Подключение к КОМПАС...");
                if (!_wrapper.TestConnection())
                {
                    throw new ArgumentException("Не удалось подключиться к КОМПАС. Убедитесь, что КОМПАС 3D установлен и запущен.");
                }
                Console.WriteLine("   ✓ Подключение успешно");

                // 2. Создание документа
                Console.WriteLine("2. Запуск КОМПАС и создание документа...");
                _wrapper.StartKompas();
                _wrapper.CreateFile();
                Console.WriteLine("   ✓ Документ создан");

                // 3. Получение параметров
                Console.WriteLine("3. Чтение параметров...");

                double stalkHeight = parameters.NumericalParameters[ParameterType.StalkHeight].Value;
                double sideHeight = parameters.NumericalParameters[ParameterType.SideHeight].Value;
                double bowlRadius = parameters.NumericalParameters[ParameterType.BowlRadius].Value;
                double stalkRadius = parameters.NumericalParameters[ParameterType.StalkRadius].Value;
                double sideAngle = parameters.NumericalParameters[ParameterType.SideAngle].Value;
                double standRadius = parameters.NumericalParameters[ParameterType.StandRadius].Value;

                // 4. Проверка параметров
                Console.WriteLine("4. Проверка параметров...");
                ValidateParameters(stalkHeight, sideHeight, bowlRadius, stalkRadius, sideAngle, standRadius);
                Console.WriteLine("   ✓ Параметры корректны");

                // 5. Построение профиля бокала
                Console.WriteLine("5. Построение профиля бокала...");
                BuildGlassProfile(stalkHeight, sideHeight, bowlRadius, stalkRadius, sideAngle, standRadius);

                Console.WriteLine("=== ПОСТРОЕНИЕ БОКАЛА ЗАВЕРШЕНО УСПЕШНО! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ОШИБКА ПОСТРОЕНИЯ ===");
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw new ArgumentException($"Ошибка построения бокала:\n{ex.Message}");
            }
        }

        /// <summary>
        /// Строит профиль бокала и выполняет вращательное выдавливание.
        /// </summary>
        private void BuildGlassProfile(double stalkHeight, double sideHeight, double bowlRadius,
                                       double stalkRadius, double sideAngle, double standRadius)
        {
            // Получаем ссылку на деталь из нашего Wrapper
            var part = _wrapper.GetPart();
            if (part == null)
                throw new Exception("Не удалось получить интерфейс детали (ksPart) из KompasWrapper.");

            // Рассчитываем толщину стенок
            double wallThickness = stalkRadius / 2.0;
            if (wallThickness < 1) wallThickness = 1.0; // Защита от слишком тонких стенок

            // Создаем эскиз
            Console.WriteLine("1. Создание эскиза...");
            var sketchEntity = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var sketchDef = (ksSketchDefinition)sketchEntity.GetDefinition();
            var plane = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            sketchDef.SetPlane(plane);
            sketchEntity.Create();

            // Рисуем сложный профиль бокала и осевую линию 
            Console.WriteLine("2. Рисование финального профиля бокала и оси...");
            var doc2D = (ksDocument2D)sketchDef.BeginEdit();
            try
            {
                // === ОСЬ ВРАЩЕНИЯ ===
                double totalHeight = wallThickness + stalkHeight + bowlRadius + sideHeight + 10;
                doc2D.ksLineSeg(0, 0, 0, totalHeight, 3);

                // Внешний контур
                var p1 = (X: standRadius, Y: 0.0);
                var p2 = (X: standRadius, Y: stalkRadius);
                var p3 = (X: stalkRadius, Y: stalkRadius);
                var p4 = (X: stalkRadius, Y: stalkRadius + stalkHeight);
                var p5_center = (X: 0.0, Y: stalkRadius + stalkHeight + bowlRadius);
                var p5_end = (X: bowlRadius, Y: stalkRadius + stalkHeight + bowlRadius);
                var p6_angleRad = sideAngle * Math.PI / 180.0;
                var p6 = (X: p5_end.X - sideHeight * Math.Sin(p6_angleRad), Y: p5_end.Y + sideHeight * Math.Cos(p6_angleRad));

                doc2D.ksLineSeg(p1.X, p1.Y, p2.X, p2.Y, 1);
                doc2D.ksLineSeg(p2.X, p2.Y, p3.X, p3.Y, 1);
                doc2D.ksLineSeg(p3.X, p3.Y, p4.X, p4.Y, 1);
                doc2D.ksArcByPoint(p5_center.X, p5_center.Y, bowlRadius, p4.X, p4.Y, p5_end.X, p5_end.Y, 1, 1);
                doc2D.ksLineSeg(p5_end.X, p5_end.Y, p6.X, p6.Y, 1);

                // Внутренний контур
                var p7_inner_top = (X: p6.X - wallThickness, Y: p6.Y);
                var p8_inner_arc_end = (X: bowlRadius - wallThickness, Y: p5_end.Y );
                double innerBowlRadius = bowlRadius - wallThickness;
                var p9_inner_stalk_top = (X: 0.0, Y: stalkRadius + stalkHeight + wallThickness);
                var p10_inner_stand_corner = (X: 0.0, Y: 0.0);

                doc2D.ksLineSeg(p6.X, p6.Y, p7_inner_top.X, p7_inner_top.Y, 1);
                doc2D.ksLineSeg(p7_inner_top.X, p7_inner_top.Y, p8_inner_arc_end.X, p8_inner_arc_end.Y, 1);
                doc2D.ksArcByPoint(p5_center.X, p5_center.Y, innerBowlRadius, p8_inner_arc_end.X, p8_inner_arc_end.Y, p9_inner_stalk_top.X, p9_inner_stalk_top.Y, -1, 1);
                //doc2D.ksLineSeg(p9_inner_stalk_top.X, p9_inner_stalk_top.Y, p10_inner_stand_corner.X, p10_inner_stand_corner.Y, 1);

                // Замыкаем основание
                doc2D.ksLineSeg(p10_inner_stand_corner.X, p10_inner_stand_corner.Y, p1.X, p1.Y, 1);

                Console.WriteLine("   ✓ Профиль и ось нарисованы");
            }
            finally
            {
                sketchDef.EndEdit();
            }

            // Создаем операцию вращения
            Console.WriteLine("3. Создание элемента вращения...");
            var revolveEntity = (ksEntity)part.NewEntity((short)Obj3dType.o3d_baseRotated);
            var revolveDef = (ksBaseRotatedDefinition)revolveEntity.GetDefinition();

            revolveDef.SetSketch(sketchEntity);
            revolveDef.SetSideParam(true, 360);
            revolveDef.directionType = (short)Direction_Type.dtNormal;

            if (revolveEntity.Create())
            {
                part.RebuildModel();
                Console.WriteLine("   ✓ Финальная модель бокала успешно создана!");
            }
            else
            {
                throw new Exception("Не удалось создать вращательное выдавливание (метод Create() вернул false).");
            }
        }


        /// <summary>
        /// Проверка параметров на валидность
        /// </summary>
        private void ValidateParameters(double stalkHeight, double sideHeight, double bowlRadius,
                                       double stalkRadius, double sideAngle, double standRadius)
        {
            if (stalkHeight <= 0) throw new ArgumentException("Высота ножки должна быть положительной");
            if (sideHeight <= 0) throw new ArgumentException("Высота стенки должна быть положительной");
            if (bowlRadius <= 0) throw new ArgumentException("Радиус чаши должен быть положительным");
            if (stalkRadius <= 0) throw new ArgumentException("Радиус ножки должен быть положительным");
            if (sideAngle < 0 || sideAngle > 90) throw new ArgumentException("Угол наклона должен быть от 0 до 90 градусов");
            if (standRadius <= 0) throw new ArgumentException("Радиус основания должен быть положительным");

            // Проверка минимальной толщины
            double wallThickness = stalkRadius / 2.0;
            if (wallThickness < 0.1)
                throw new ArgumentException($"Толщина стенок слишком мала: {wallThickness:F2} мм");

            // Проверка геометрической совместимости
            if (bowlRadius <= wallThickness)
                throw new ArgumentException($"Радиус чаши ({bowlRadius}) должен быть больше толщины стенок ({wallThickness:F2})");

            if (standRadius <= wallThickness)
                throw new ArgumentException($"Радиус основания ({standRadius}) должен быть больше толщины стенок ({wallThickness:F2})");
        }

        /// <summary>
        /// Проверка параметров объекта Parameters
        /// </summary>
        public bool ValidateParameters(Parameters parameters)
        {
            try
            {
                double stalkHeight = parameters.NumericalParameters[ParameterType.StalkHeight].Value;
                double sideHeight = parameters.NumericalParameters[ParameterType.SideHeight].Value;
                double bowlRadius = parameters.NumericalParameters[ParameterType.BowlRadius].Value;
                double stalkRadius = parameters.NumericalParameters[ParameterType.StalkRadius].Value;
                double sideAngle = parameters.NumericalParameters[ParameterType.SideAngle].Value;
                double standRadius = parameters.NumericalParameters[ParameterType.StandRadius].Value;

                ValidateParameters(stalkHeight, sideHeight, bowlRadius, stalkRadius, sideAngle, standRadius);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Сброс всех построений
        /// </summary>
        public void Reset()
        {
            try
            {
                if (_wrapper != null)
                {
                    _wrapper.ResetAllSketches();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сброса построений: {ex.Message}");
            }
        }
    }
}