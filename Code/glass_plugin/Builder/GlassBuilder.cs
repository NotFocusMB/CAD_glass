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
        /// Строит профиль бокала и выполняет вращательное выдавливание
        /// </summary>
        private void BuildGlassProfile(double stalkHeight, double sideHeight, double bowlRadius,
                                      double stalkRadius, double sideAngle, double standRadius)
        {
            // Толщина стенок
            double wallThickness = stalkRadius / 2.0;

            Console.WriteLine("=== РАСЧЕТ ГЕОМЕТРИИ ПРОФИЛЯ ===");
            Console.WriteLine($"Толщина стенок: {wallThickness:F2} мм");

            // 1. Создаем эскиз на плоскости XOZ (ось вращения Z)
            Console.WriteLine("1. Создание эскиза на плоскости XOZ...");
            var sketch = _wrapper.CreateSketch((short)Kompas6Constants3D.Obj3dType.o3d_planeXOZ);
            if (sketch == null)
                throw new Exception("Не удалось создать эскиз");

            // 2. Начинаем редактирование эскиза
            var doc2D = _wrapper.BeginSketchEdit(sketch);
            if (doc2D == null)
                throw new Exception("Не удалось начать редактирование эскиза");

            try
            {
                Console.WriteLine("2. Рисование профиля бокала...");

                // === РАСЧЕТ ТОЧЕК ВНЕШНЕГО КОНТУРА ===

                // Точка A: начало (0, 0) - центр основания
                double ax = 0;
                double ay = 0;

                // Точка B: толщина основания вверх (внутренняя точка)
                double bx = 0;
                double by = wallThickness;

                // Точка C: ножка вправо
                double cx = standRadius;
                double cy = wallThickness;

                // Точка D: начало ножки вверх
                double dx = standRadius;
                double dy = wallThickness + stalkHeight;

                // Точка E: центр дуги чаши
                // Дуга радиусом bowlRadius от точки D до точки на стенке
                double ex = standRadius + bowlRadius;
                double ey = wallThickness + stalkHeight;

                // Точка F: конец дуги (начало стенки)
                // Угол дуги: 90° вниз от горизонтали
                double fx = standRadius;
                double fy = wallThickness + stalkHeight + bowlRadius;

                // Точка G: конец наклонной стенки
                double angleRad = sideAngle * Math.PI / 180.0;
                double gx = fx + sideHeight * Math.Sin(angleRad); // Наклон по X
                double gy = fy + sideHeight * Math.Cos(angleRad); // Наклон по Y

                // Точка H: верхний край (внутрь на толщину)
                double hx = gx - wallThickness * Math.Cos(angleRad);
                double hy = gy + wallThickness * Math.Sin(angleRad);

                // === ВНУТРЕННИЙ КОНТУР (зеркально) ===
                // Для простоты сделаем толщину постоянной
                // На практике нужно рассчитать внутренние точки

                Console.WriteLine("Точки внешнего контура:");
                Console.WriteLine($"A({ax:F1}, {ay:F1}) → B({bx:F1}, {by:F1})");
                Console.WriteLine($"B → C({cx:F1}, {cy:F1})");
                Console.WriteLine($"C → D({dx:F1}, {dy:F1})");
                Console.WriteLine($"Дуга: центр E({ex:F1}, {ey:F1}), R={bowlRadius}");
                Console.WriteLine($"D → F({fx:F1}, {fy:F1})");
                Console.WriteLine($"F → G({gx:F1}, {gy:F1}) угол {sideAngle}°");
                Console.WriteLine($"G → H({hx:F1}, {hy:F1}) толщина {wallThickness}");

                // 3. Рисуем внешний контур
                Console.WriteLine("3. Рисование линий...");

                // Основание (вертикальная линия)
                _wrapper.DrawLineSeg(doc2D, ax, ay, bx, by, 1);

                // Ножка (горизонтальная линия)
                _wrapper.DrawLineSeg(doc2D, bx, by, cx, cy, 1);

                // Ножка (вертикальная линия)
                _wrapper.DrawLineSeg(doc2D, cx, cy, dx, dy, 1);

                // Дуга чаши (90° от D к F)
                Console.WriteLine("4. Рисование дуги чаши...");
                _wrapper.DrawArc(doc2D, ex, ey, bowlRadius, 180, 270, 1);

                // Наклонная стенка
                _wrapper.DrawLineSeg(doc2D, fx, fy, gx, gy, 1);

                // Верхний край (по толщине)
                _wrapper.DrawLineSeg(doc2D, gx, gy, hx, hy, 1);

                // TODO: Добавить внутренний контур и замкнуть профиль
                // Для теста пока оставляем открытый контур

                Console.WriteLine("   ✓ Профиль нарисован");
            }
            finally
            {
                // 4. Завершаем редактирование эскиза
                _wrapper.EndSketchEdit(sketch);
            }

            // 5. Вращательное выдавливание (360° вокруг оси Z)
            Console.WriteLine("5. Вращательное выдавливание...");
            var revolve = _wrapper.CreateRevolvedExtrusion(sketch, 360);

            if (revolve == null)
                throw new Exception("Не удалось создать вращательное выдавливание");

            Console.WriteLine("   ✓ Бокал создан вращательным выдавливанием");
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

        /// <summary>
        /// Тестовая функция для построения простого цилиндра (только для теста)
        /// </summary>
        /// <summary>
        /// Тестовая функция - простой прямоугольник с выдавливанием
        /// </summary>
        public void TestRevolveExtrusion()
        {
            try
            {
                Console.WriteLine("=== ТЕСТ: ВРАЩЕНИЕ ВОКРУГ НЕЯВНОЙ ОСИ ЭСКИЗА ===");
                _wrapper = new KompasWrapper();

                _wrapper.StartKompas();
                _wrapper.CreateFile();
                Console.WriteLine("   ✓ Инициализация прошла успешно!");

                var part = _wrapper.GetPart();
                if (part == null)
                    throw new Exception("Не удалось получить интерфейс детали (ksPart).");

                // --- ШАГ 1: Создаем эскиз ТОЛЬКО с профилем ---
                Console.WriteLine("1. Создание эскиза (только профиль)...");
                var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
                var sketchDef = (ksSketchDefinition)sketch.GetDefinition();
                var plane = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
                sketchDef.SetPlane(plane);
                sketch.Create();

                var doc2D = (ksDocument2D)sketchDef.BeginEdit();
                try
                {
                    // Рисуем простой замкнутый прямоугольник,
                    // который НЕ КАСАЕТСЯ осей X=0 или Y=0.
                    doc2D.ksLineSeg(10, 10, 20, 10, 1);
                    doc2D.ksLineSeg(20, 10, 20, 30, 1);
                    doc2D.ksLineSeg(20, 30, 10, 30, 1);
                    doc2D.ksLineSeg(10, 30, 10, 10, 1);
                    Console.WriteLine("   ✓ Профиль нарисован");
                }
                finally
                {
                    sketchDef.EndEdit();
                }

                // --- ШАГ 2: Создаем операцию ---
                Console.WriteLine("2. Создание элемента вращения...");
                var revolve = (ksEntity)part.NewEntity((short)Obj3dType.o3d_baseRotated);
                var revolveDef = (ksBaseRotatedDefinition)revolve.GetDefinition();

                // Указываем ТОЛЬКО эскиз и угол.
                // КОМПАС должен по умолчанию использовать ось Y эскиза.
                revolveDef.SetSketch(sketch);
                revolveDef.SetSideParam(true, 360);

                Console.WriteLine("   ✓ Параметры установлены");

                // --- ШАГ 3: Создаем элемент ---
                if (revolve.Create())
                {
                    part.RebuildModel();
                    Console.WriteLine("=== ТЕСТ УСПЕШЕН: ВРАЩАТЕЛЬНОЕ ВЫДАВЛИВАНИЕ СОЗДАНО! ===");
                }
                else
                {
                    // Если это снова не сработает, значит, в API есть неочевидная особенность,
                    // которую можно найти только в официальных примерах SDK для вашей версии.
                    throw new Exception("Не удалось создать вращательное выдавливание (Create() вернул false). Проблема в интерпретации эскиза.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ТЕСТОВАЯ ОШИБКА ===");
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }


    }
}