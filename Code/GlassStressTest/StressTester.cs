using System;
using System.Diagnostics;
using System.IO;
using Core;
using GlassPlugin;
using Microsoft.VisualBasic.Devices;

namespace GlassPluginStressTests
{
    /// <summary>
    /// Класс для нагрузочного тестирования плагина построения бокалов.
    /// </summary>
    public class StressTester
    {
        /// <summary>
        /// Коэффициент преобразования байтов в гигабайты.
        /// </summary>
        private const double GIGABYTE_IN_BYTE =
            0.000000000931322574615478515625;

        /// <summary>
        /// Объект для получения информации о системе.
        /// </summary>
        private readonly ComputerInfo _computerInfo;

        /// <summary>
        /// Директория для сохранения логов.
        /// </summary>
        private readonly string _logsDirectory;

        /// <summary>
        /// Флаг остановки тестирования.
        /// </summary>
        private volatile bool _stopRequested = false;

        /// <summary>
        /// Инициализирует новый экземпляр класса StressTester.
        /// </summary>
        public StressTester()
        {
            _computerInfo = new ComputerInfo();

            // Создаем папку для логов
            _logsDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "StressTestLogs");

            try
            {
                if (!Directory.Exists(_logsDirectory))
                {
                    Directory.CreateDirectory(_logsDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось создать папку логов: {ex.Message}");
                _logsDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// Выполняет бесконечное нагрузочное тестирование.
        /// </summary>
        /// <param name="parameters">Параметры бокала.</param>
        public void RunInfiniteStressTest(Parameters parameters)
        {
            Console.WriteLine("=== Нагрузочное тестирование плагина бокала ===");
            Console.WriteLine();
            Console.WriteLine("Параметры бокала:");
            PrintParameters(parameters);
            Console.WriteLine();

            // Информация о системе
            double totalMemoryGB = _computerInfo.TotalPhysicalMemory * GIGABYTE_IN_BYTE;
            Console.WriteLine($"Информация о системе:");
            Console.WriteLine($"  Всего ОЗУ: {totalMemoryGB:F1} ГБ");
            Console.WriteLine($"  Процессор: {Environment.ProcessorCount} ядер");
            Console.WriteLine($"  Папка логов: {_logsDirectory}");
            Console.WriteLine();

            Console.WriteLine("Начало тестирования...");
            Console.WriteLine("Для остановки нажмите Ctrl+C");
            Console.WriteLine();

            // Создаем файл для записи результатов
            string fileName = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(_logsDirectory, fileName);

            Console.WriteLine($"Лог файл: {filePath}");
            Console.WriteLine();
            Console.WriteLine("Номер\tВремя(мс)\tОЗУ(ГБ)");
            Console.WriteLine("-------------------------------");

            using (var streamWriter = new StreamWriter(filePath))
            {
                var builder = new GlassBuilder();
                var stopWatch = new Stopwatch();
                int count = 0;

                try
                {
                    // Устанавливаем обработчик Ctrl+C
                    Console.CancelKeyPress += OnCancelKeyPress;

                    // Бесконечный цикл как в методичке
                    while (!_stopRequested)
                    {
                        count++;

                        try
                        {
                            stopWatch.Restart();
                            builder.BuildGlass(parameters, false); // Без ручки
                            stopWatch.Stop();
                        }
                        catch (Exception ex)
                        {
                            stopWatch.Stop();
                            Console.WriteLine($"\nОшибка при построении {count}: {ex.Message}");
                            // Продолжаем тестирование несмотря на ошибку
                        }

                        // Получаем использование ОЗУ всей системой
                        var usedMemory = GetTotalSystemMemoryGB();

                        // Время в миллисекундах для графика
                        double timeMs = stopWatch.Elapsed.TotalMilliseconds;

                        // Записываем в формате из методички
                        streamWriter.WriteLine(
                            $"{count}\t{timeMs:F0}\t{usedMemory:F9}");
                        streamWriter.Flush();

                        // Выводим в консоль каждые итерации
                        if (count % 1 == 0) // Выводим каждую итерацию
                        {
                            Console.Write($"\r{count}\t{timeMs:F0}\t\t{usedMemory:F9}");
                        }

                        // Небольшая пауза для стабильности
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\nОшибка при тестировании: {ex.Message}");
                }
                finally
                {
                    // Убираем обработчик
                    Console.CancelKeyPress -= OnCancelKeyPress;

                    Console.WriteLine($"\n\nТестирование завершено.");
                    Console.WriteLine($"Всего построений: {count}");
                    Console.WriteLine($"Результаты сохранены в: {filePath}");

                    // Выводим итоговую информацию о памяти
                    double finalMemoryGB = GetTotalSystemMemoryGB();
                    Console.WriteLine($"Финальное использование ОЗУ: {finalMemoryGB:F9} ГБ");
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия Ctrl+C.
        /// </summary>
        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _stopRequested = true;
            e.Cancel = true; // Отменяем стандартное поведение
            Console.WriteLine("\n\nПолучен сигнал остановки (Ctrl+C)...");
        }

        /// <summary>
        /// Выполняет тест с фиксированным количеством итераций.
        /// </summary>
        /// <param name="parameters">Параметры бокала.</param>
        /// <param name="iterations">Количество итераций.</param>
        public void RunFixedStressTest(Parameters parameters, int iterations)
        {
            Console.WriteLine($"=== Нагрузочное тестирование ({iterations} итераций) ===");
            Console.WriteLine();
            Console.WriteLine("Параметры бокала:");
            PrintParameters(parameters);
            Console.WriteLine();

            double totalMemoryGB = _computerInfo.TotalPhysicalMemory * GIGABYTE_IN_BYTE;
            Console.WriteLine($"Всего ОЗУ в системе: {totalMemoryGB:F1} ГБ");
            Console.WriteLine($"Папка логов: {_logsDirectory}");
            Console.WriteLine();

            string fileName = $"log_{iterations}iter_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(_logsDirectory, fileName);

            Console.WriteLine($"Лог файл: {filePath}");
            Console.WriteLine();
            Console.WriteLine("Прогресс: 0%");

            using (var streamWriter = new StreamWriter(filePath))
            {
                var builder = new GlassBuilder();
                var stopWatch = new Stopwatch();
                int successfulBuilds = 0;
                int failedBuilds = 0;

                try
                {
                    for (int count = 1; count <= iterations; count++)
                    {
                        if (_stopRequested)
                        {
                            Console.WriteLine("\n\nТестирование прервано пользователем.");
                            break;
                        }

                        try
                        {
                            stopWatch.Restart();
                            builder.BuildGlass(parameters, false);
                            stopWatch.Stop();
                            successfulBuilds++;
                        }
                        catch (Exception ex)
                        {
                            stopWatch.Stop();
                            failedBuilds++;
                            Console.WriteLine($"\nОшибка при построении {count}: {ex.Message}");
                        }

                        var usedMemory = GetTotalSystemMemoryGB();
                        double timeMs = stopWatch.Elapsed.TotalMilliseconds;

                        streamWriter.WriteLine(
                            $"{count}\t{timeMs:F0}\t{usedMemory:F9}");
                        streamWriter.Flush();

                        if (count % 10 == 0 || count == iterations)
                        {
                            double percentage = (count * 100.0) / iterations;
                            Console.Write($"\rПрогресс: {count}/{iterations} " +
                                $"({percentage:F1}%) | Успешно: {successfulBuilds} | " +
                                $"Ошибки: {failedBuilds} | ОЗУ: {usedMemory:F3} ГБ");
                        }

                        System.Threading.Thread.Sleep(100);
                    }

                    Console.WriteLine($"\n\nТестирование завершено.");
                    Console.WriteLine($"Всего построений: {Math.Min(iterations, successfulBuilds + failedBuilds)}");
                    Console.WriteLine($"Успешных: {successfulBuilds}, Ошибок: {failedBuilds}");
                    Console.WriteLine($"Результаты сохранены в: {filePath}");

                    double finalMemoryGB = GetTotalSystemMemoryGB();
                    Console.WriteLine($"Финальное использование ОЗУ: {finalMemoryGB:F9} ГБ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nКритическая ошибка: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Получает общее использование оперативной памяти системой в ГБ.
        /// Использует ComputerInfo как в методичке.
        /// </summary>
        /// <returns>Используемая память системой в гигабайтах.</returns>
        private double GetTotalSystemMemoryGB()
        {
            try
            {
                // Метод из методички
                double usedMemory = (_computerInfo.TotalPhysicalMemory
                    - _computerInfo.AvailablePhysicalMemory) * GIGABYTE_IN_BYTE;
                return usedMemory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка получения памяти: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Выводит параметры бокала в консоль.
        /// </summary>
        /// <param name="parameters">Параметры бокала.</param>
        private void PrintParameters(Parameters parameters)
        {
            var numParams = parameters.NumericalParameters;

            Console.WriteLine($"  Высота ножки: {numParams[ParameterType.StalkHeight].Value} мм");
            Console.WriteLine($"  Высота стенки: {numParams[ParameterType.SideHeight].Value} мм");
            Console.WriteLine($"  Радиус чаши: {numParams[ParameterType.BowlRadius].Value} мм");
            Console.WriteLine($"  Радиус ножки: {numParams[ParameterType.StalkRadius].Value} мм");
            Console.WriteLine($"  Угол наклона: {numParams[ParameterType.SideAngle].Value}°");
            Console.WriteLine($"  Радиус основания: {numParams[ParameterType.StandRadius].Value} мм");
        }

        /// <summary>
        /// Останавливает тестирование.
        /// </summary>
        public void StopTesting()
        {
            _stopRequested = true;
        }
    }
}