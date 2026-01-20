using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;
using System;
using System.Runtime.InteropServices;

namespace Builder
{
    /// <summary>
    /// Предоставляет упрощенный интерфейс (обертку) для работы с API КОМПАС-3D.
    /// Инкапсулирует логику подключения, создания документов и базовых 3D-операций.
    /// </summary>
    public class KompasWrapper
    {
        //TODO: rsdn (исправлено)
        /// <summary>
        /// Главный объект для взаимодействия с API КОМПАС-3D.
        /// </summary>
        private KompasObject _kompas;

        /// <summary>
        /// Объект 3D-документа, в котором ведется работа.
        /// </summary>
        private ksDocument3D _document3D;

        /// <summary>
        /// Объект основной детали 3D-документа.
        /// </summary>
        private ksPart _part;

        /// <summary>
        /// Флаг, указывающий, было ли установлено соединение с КОМПАС.
        /// </summary>
        private bool _isCadAttached = false;

        /// <summary>
        /// Подключается к запущенному экземпляру КОМПАС-3D или запускает новый.
        /// </summary>
        /// <returns>True, если подключение успешно; иначе False.</returns>
        public bool ConnectCAD()
        {
            try
            {
                if (_kompas != null)
                {
                    _kompas.Visible = true;
                    _kompas.ActivateControllerAPI();
                    _isCadAttached = true;
                    return true;
                }
                try
                {
                    // Пытаемся получить доступ к запущенному экземпляру КОМПАС.
                    _kompas = (KompasObject)Marshal.GetActiveObject(
                        "KOMPAS.Application.5");
                }
                catch (COMException)
                {
                    // Если КОМПАС не запущен, создаем новый экземпляр.
                    var kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                    if (kompasType == null)
                        return false;
                    _kompas = (KompasObject)Activator.CreateInstance(kompasType);
                }

                if (_kompas == null)
                    return false;

                _kompas.Visible = true;
                _kompas.ActivateControllerAPI();
                _isCadAttached = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Создает новый 3D-документ и получает доступ к его основной детали.
        /// </summary>
        /// <returns>True, если документ и деталь успешно созданы; иначе False.</returns>
        public bool CreateDocument()
        {
            try
            {
                if (!_isCadAttached && !ConnectCAD())
                    return false;

                _document3D = (ksDocument3D)_kompas.Document3D();
                _document3D.Create();
                _document3D = (ksDocument3D)_kompas.ActiveDocument3D();
                _part = (ksPart)_document3D.GetPart((short)Part_Type.pTop_Part);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет соединение с КОМПАС-3D. Является оберткой над ConnectCAD.
        /// </summary>
        /// <returns>True, если соединение может быть установлено; иначе False.</returns>
        public bool TestConnection()
        {
            try
            {
                return ConnectCAD();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Запускает КОМПАС-3D и устанавливает с ним соединение.
        /// </summary>
        public void StartKompas()
        {
            ConnectCAD();
        }

        /// <summary>
        /// Создает новый 3D-документ.
        /// </summary>
        public void CreateFile()
        {
            CreateDocument();
        }

        /// <summary>
        /// Создает новый пустой эскиз на одной из базовых плоскостей.
        /// </summary>
        /// <param name="planeType">Тип базовой плоскости (o3d_planeXOZ и т.д.).</param>
        /// <returns>Объект созданного эскиза (ksEntity).</returns>
        public ksEntity CreateSketch(short planeType)
        {
            try
            {
                var plane = (ksEntity)_part.GetDefaultEntity(planeType);
                var sketch = (ksEntity)_part.NewEntity((short)Obj3dType.o3d_sketch);
                var sketchDef = (ksSketchDefinition)sketch.GetDefinition();
                sketchDef.SetPlane(plane);
                sketch.Create();
                return sketch;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Переводит указанный эскиз в режим редактирования.
        /// </summary>
        /// <param name="sketch">Эскиз, который нужно начать редактировать.</param>
        /// <returns>Объект ksDocument2D для рисования в эскизе.</returns>
        public ksDocument2D BeginSketchEdit(ksEntity sketch)
        {
            try
            {
                var sketchDef = (ksSketchDefinition)sketch.GetDefinition();
                return sketchDef.BeginEdit();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Завершает режим редактирования для указанного эскиза.
        /// </summary>
        /// <param name="sketch">Эскиз, редактирование которого нужно завершить.</param>
        public void EndSketchEdit(ksEntity sketch)
        {
            try
            {
                var sketchDef = (ksSketchDefinition)sketch.GetDefinition();
                sketchDef.EndEdit();
            }
            //TODO: error! (исправлено)
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка завершения редактирования: {ex.Message}");
            }
        }

        /// <summary>
        /// Рисует отрезок линии в 2D-документе эскиза.
        /// </summary>
        /// <param name="doc2D">2D-документ эскиза.</param>
        /// <param name="x1">X-координата начальной точки.</param>
        /// <param name="y1">Y-координата начальной точки.</param>
        /// <param name="x2">X-координата конечной точки.</param>
        /// <param name="y2">Y-координата конечной точки.</param>
        /// <param name="style">Стиль линии (1 - основная, 3 - осевая).</param>
        public void DrawLineSeg(ksDocument2D doc2D, double x1, double y1,
            double x2, double y2, int style = 1)
        {
            try
            {
                doc2D.ksLineSeg(x1, y1, x2, y2, style);
            }
            //TODO: error! (исправлено)
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка рисования отрезка: {ex.Message}");
            }
        }

        /// <summary>
        /// Рисует дугу в 2D-документе эскиза по центральной точке, радиусу и углам.
        /// </summary>
        /// <param name="doc2D">2D-документ эскиза.</param>
        /// <param name="centerX">X-координата центра дуги.</param>
        /// <param name="centerY">Y-координата центра дуги.</param>
        /// <param name="radius">Радиус дуги.</param>
        /// <param name="startAngle">Начальный угол в градусах.</param>
        /// <param name="endAngle">Конечный угол в градусах.</param>
        /// <param name="style">Стиль линии.</param>
        public void DrawArc(ksDocument2D doc2D, double centerX, double centerY,
                            double radius, double startAngle, double endAngle,
                            int style = 1)
        {
            try
            {
                doc2D.ksArcByAngle(centerX, centerY, radius,
                    startAngle, endAngle, 1, style);
            }
            //TODO: error! (исправлено)
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка рисования дуги: {ex.Message}");
            }
        }

        //TODO: rsdn (исправлено)
        /// <summary>
        /// Создает тело вращения на основе эскиза. 
        /// </summary>
        /// <param name="sketch">Эскиз, содержащий контур и осевую линию.</param>
        /// <param name="angleDegrees">Угол вращения в градусах.</param>
        /// <returns>Объект созданного тела вращения.</returns>
        public ksEntity CreateRevolvedExtrusion(ksEntity sketch,
            double angleDegrees = 360)
        {
            try
            {
                Console.WriteLine("=== СОЗДАНИЕ ВРАЩАТЕЛЬНОГО ВЫДАВЛИВАНИЯ ===");
                var revolve = (ksEntity)_part.NewEntity(
                    (short)Obj3dType.o3d_baseRotated);
                if (revolve == null)
                {
                    Console.WriteLine("Ошибка: Не удалось создать объект"
                        + " вращательного выдавливания");
                    return null;
                }

                var revolveDef = (ksBaseRotatedDefinition)revolve.GetDefinition();
                if (revolveDef == null)
                {
                    Console.WriteLine("Ошибка: Не удалось получить определение"
                        + " вращательного выдавливания");
                    return null;
                }

                revolveDef.SetSketch(sketch);
                Console.WriteLine("Эскиз установлен");

                try
                {
                    dynamic dynamicRevolveDef = revolveDef;
                    var axisZ = (ksEntity)_part.GetDefaultEntity(
                        (short)Obj3dType.o3d_axisOZ);
                    if (axisZ != null)
                    {
                        dynamicRevolveDef.axis = axisZ;
                        Console.WriteLine("Ось Z установлена");
                    }
                    double angleRad = angleDegrees * Math.PI / 180.0;
                    dynamicRevolveDef.angle = angleRad;
                    Console.WriteLine($"Угол вращения: {angleDegrees}°");
                }
                catch (Exception dynEx)
                {
                    Console.WriteLine("Динамические свойства не сработали:"
                        + $" {dynEx.Message}");
                    try
                    {
                        revolveDef.SetSideParam(true, angleDegrees * Math.PI / 180.0);
                        Console.WriteLine("Параметры установлены через SetSideParam");
                    }
                    catch (Exception paramEx)
                    {
                        Console.WriteLine("SetSideParam не сработал:"
                            + $" {paramEx.Message}");
                    }
                }

                bool createResult = revolve.Create();
                if (!createResult)
                {
                    Console.WriteLine("Ошибка: Не удалось создать операцию"
                        + " вращательного выдавливания");
                    return null;
                }

                Console.WriteLine("=== ВРАЩАТЕЛЬНОЕ ВЫДАВЛИВАНИЕ СОЗДАНО! ===");
                return revolve;
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== ОШИБКА ВРАЩАТЕЛЬНОГО ВЫДАВЛИВАНИЯ ===");
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Выполняет сброс (в данной реализации - заглушка).
        /// </summary>
        public void ResetAllSketches()
        {
            Console.WriteLine("Сброс выполнен");
        }

        /// <summary>
        /// Возвращает сохраненный главный объект API КОМПАС.
        /// </summary>
        /// <returns>Объект KompasObject.</returns>
        public KompasObject GetKompasObject()
        {
            return _kompas;
        }

        /// <summary>
        /// Возвращает сохраненный интерфейс 3D-детали.
        /// </summary>
        /// <returns>Объект ksPart.</returns>
        public ksPart GetPart()
        {
            return _part;
        }
    }
}
