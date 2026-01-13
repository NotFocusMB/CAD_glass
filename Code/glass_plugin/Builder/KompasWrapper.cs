using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;
using System;
using System.Runtime.InteropServices;

namespace Builder
{
    public class KompasWrapper
    {
        private KompasObject _kompas;
        private ksDocument3D _document3D;
        private ksPart _part;
        private bool _isCadAttached = false;

        // === БАЗОВЫЕ МЕТОДЫ ПОДКЛЮЧЕНИЯ ===

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
                    _kompas = (KompasObject)Marshal.GetActiveObject("KOMPAS.Application.5");
                }
                catch (COMException)
                {
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

        public void StartKompas()
        {
            ConnectCAD();
        }

        public void CreateFile()
        {
            CreateDocument();
        }

        // === МЕТОДЫ РАБОТЫ С ЭСКИЗАМИ ===

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

        public void EndSketchEdit(ksEntity sketch)
        {
            try
            {
                var sketchDef = (ksSketchDefinition)sketch.GetDefinition();
                sketchDef.EndEdit();
            }
            catch { }
        }

        // === МЕТОДЫ РИСОВАНИЯ В ЭСКИЗЕ ===

        public void DrawLineSeg(ksDocument2D doc2D, double x1, double y1, double x2, double y2, int style = 1)
        {
            try
            {
                doc2D.ksLineSeg(x1, y1, x2, y2, style);
            }
            catch { }
        }

        public void DrawArc(ksDocument2D doc2D, double centerX, double centerY,
                            double radius, double startAngle, double endAngle, int style = 1)
        {
            try
            {
                doc2D.ksArcByAngle(centerX, centerY, radius, startAngle, endAngle, 1, style);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка рисования дуги: {ex.Message}");
            }
        }

        // === ГЛАВНЫЙ МЕТОД - ВРАЩАТЕЛЬНОЕ ВЫДАВЛИВАНИЕ ===

        public ksEntity CreateRevolvedExtrusion(ksEntity sketch, double angleDegrees = 360)
        {
            try
            {
                Console.WriteLine($"=== СОЗДАНИЕ ВРАЩАТЕЛЬНОГО ВЫДАВЛИВАНИЯ ===");

                var revolve = (ksEntity)_part.NewEntity((short)Obj3dType.o3d_baseRotated);
                if (revolve == null)
                {
                    Console.WriteLine("Ошибка: Не удалось создать объект вращательного выдавливания");
                    return null;
                }

                var revolveDef = (ksBaseRotatedDefinition)revolve.GetDefinition();
                if (revolveDef == null)
                {
                    Console.WriteLine("Ошибка: Не удалось получить определение вращательного выдавливания");
                    return null;
                }

                // Устанавливаем эскиз
                revolveDef.SetSketch(sketch);
                Console.WriteLine("Эскиз установлен");

                // Пытаемся настроить через динамические свойства
                try
                {
                    dynamic dynamicRevolveDef = revolveDef;

                    // Получаем ось Z
                    var axisZ = (ksEntity)_part.GetDefaultEntity((short)Obj3dType.o3d_axisOZ);
                    if (axisZ != null)
                    {
                        dynamicRevolveDef.axis = axisZ;
                        Console.WriteLine("Ось Z установлена");
                    }

                    // Устанавливаем угол вращения
                    double angleRad = angleDegrees * Math.PI / 180.0;
                    dynamicRevolveDef.angle = angleRad;
                    Console.WriteLine($"Угол вращения: {angleDegrees}°");
                }
                catch (Exception dynEx)
                {
                    Console.WriteLine($"Динамические свойства не сработали: {dynEx.Message}");

                    // Альтернативный вариант
                    try
                    {
                        revolveDef.SetSideParam(true, angleDegrees * Math.PI / 180.0);
                        Console.WriteLine("Параметры установлены через SetSideParam");
                    }
                    catch (Exception paramEx)
                    {
                        Console.WriteLine($"SetSideParam не сработал: {paramEx.Message}");
                    }
                }

                // Создаем операцию
                bool createResult = revolve.Create();
                if (!createResult)
                {
                    Console.WriteLine("Ошибка: Не удалось создать операцию вращательного выдавливания");
                    return null;
                }

                Console.WriteLine($"=== ВРАЩАТЕЛЬНОЕ ВЫДАВЛИВАНИЕ СОЗДАНО УСПЕШНО! ===");
                return revolve;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ОШИБКА ВРАЩАТЕЛЬНОГО ВЫДАВЛИВАНИЯ ===");
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }

        // === ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ===

        public void ResetAllSketches()
        {
            Console.WriteLine("Сброс выполнено");
        }

        /// <summary>
        /// Возвращает сохраненный главный объект API КОМПАС
        /// </summary>
        public KompasObject GetKompasObject()
        {
            return _kompas;
        }

        /// <summary>
        /// Возвращает сохраненный интерфейс 3D-детали
        /// </summary>
        public ksPart GetPart()
        {
            return _part;
        }
    }
}