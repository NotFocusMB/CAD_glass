using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.Devices;
using Core;
using GlassPlugin;

namespace GlassPluginStressTests
{
    /// <summary>
    /// Главный класс программы для нагрузочного тестирования.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Коэффициент преобразования байтов в гигабайты.
        /// </summary>
        private const double GIGABYTE_IN_BYTE =
            0.000000000931322574615478515625;

        /// <summary>
        /// Минимальный допустимый объем доступной памяти в ГБ.
        /// </summary>
        private const double MIN_AVAILABLE_MEMORY_GB = 1.0;

        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("=== Нагрузочное тестирование " +
                "плагина бокала ===");
            Console.WriteLine();

            Console.WriteLine("Выберите параметры бокала:");
            Console.WriteLine("1 - Минимальные параметры");
            Console.WriteLine("2 - Средние параметры");
            Console.WriteLine("3 - Максимальные параметры");
            Console.Write("Ваш выбор: ");

            var choice = Console.ReadLine();
            Parameters parameters;

            switch (choice)
            {
                case "1":
                    parameters = GetMinimalParameters();
                    Console.WriteLine("Минимальные параметры");
                    break;
                case "2":
                    parameters = GetAverageParameters();
                    Console.WriteLine("Средние параметры");
                    break;
                case "3":
                    parameters = GetMaximalParameters();
                    Console.WriteLine("Максимальные параметры");
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    return;
            }

            Console.WriteLine();
            Console.WriteLine("Создавать ручку?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            Console.Write("Ваш выбор: ");

            var handleChoice = Console.ReadLine();
            bool createHandle = (handleChoice == "1");

            Console.WriteLine();
            Console.WriteLine("Начинается нагрузочное тестирование...");
            Console.WriteLine("Нажмите Ctrl+C для остановки");
            Console.WriteLine();

            RunStressTest(parameters, createHandle);
        }

        /// <summary>
        /// Выполняет нагрузочное тестирование.
        /// </summary>
        /// <param name="parameters">Параметры бокала.</param>
        /// <param name="createHandle">Создавать ручку.</param>
        private static void RunStressTest(Parameters parameters,
            bool createHandle)
        {
            var builder = new GlassBuilder();
            var stopWatch = new Stopwatch();
            var count = 0;

            // Создаем папку для логов
            var logsDirectory = @"C:\Code\CAD_glass\Code\GlassStressTest\" +
                @"bin\Debug\StressTestLogs";
            Directory.CreateDirectory(logsDirectory);

            var fileName = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(logsDirectory, fileName);

            using (var streamWriter = new StreamWriter(filePath))
            {
                Console.WriteLine($"Лог-файл: {filePath}");
                Console.WriteLine("Счёт\tВремя\t\tОЗУ (ГБ)\t" +
                    "Доступно ОЗУ (ГБ)");
                Console.WriteLine("---------------------------------------" +
                    "------------");

                try
                {
                    while (true)
                    {
                        count++;

                        // ПРОВЕРКА ПАМЯТИ ПЕРЕД КАЖДОЙ ИТЕРАЦИЕЙ
                        var computerInfo = new ComputerInfo();
                        double availableMemoryGB =
                            computerInfo.AvailablePhysicalMemory *
                            GIGABYTE_IN_BYTE;

                        if (availableMemoryGB <= MIN_AVAILABLE_MEMORY_GB)
                        {
                            Console.WriteLine($"\n⚠️  КРИТИЧЕСКАЯ " +
                                "НЕХВАТКА ПАМЯТИ!");
                            Console.WriteLine("   Доступно: " +
                                $"{availableMemoryGB:F1} ГБ");
                            Console.WriteLine("   Тест остановлен для " +
                                "предотвращения краха системы.");
                            break;
                        }

                        try
                        {
                            stopWatch.Start();
                            builder.BuildGlass(parameters, createHandle);
                            stopWatch.Stop();
                        }
                        catch (Exception ex)
                        {
                            stopWatch.Stop();
                            Console.WriteLine("Ошибка на итерации " +
                                $"{count}: {ex.Message}");
                        }

                        var usedMemory =
                            (computerInfo.TotalPhysicalMemory
                            - computerInfo.AvailablePhysicalMemory) *
                            GIGABYTE_IN_BYTE;

                        string timeString = stopWatch.Elapsed.ToString(
                            @"hh\:mm\:ss\.fff");
                        streamWriter.WriteLine(
                            $"{count}\t{timeString}\t{usedMemory}");
                        streamWriter.Flush();

                        Console.WriteLine($"{count}\t" +
                            $"{timeString}\t" +
                            $"{usedMemory:F3}\t{availableMemoryGB:F3}");

                        stopWatch.Reset();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nТест остановлен: {ex.Message}");
                }
                finally
                {
                    var computerInfo = new ComputerInfo();
                    var totalMemory = computerInfo.TotalPhysicalMemory *
                        GIGABYTE_IN_BYTE;

                    Console.WriteLine($"\n\n=== Результаты ===");
                    Console.WriteLine($"Всего построений: {count}");
                    Console.WriteLine($"Всего памяти в системе: " +
                        $"{totalMemory:F1} ГБ");
                    Console.WriteLine($"Лог-файл: {filePath}");
                    Console.WriteLine("\nНажмите любую клавишу...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Получает параметры бокала с минимальными значениями.
        /// </summary>
        /// <returns>Параметры с минимальными значениями.</returns>
        private static Parameters GetMinimalParameters()
        {
            var parameters = new Parameters();
            parameters.SetDependencies(50.0,
                ParameterType.StalkHeight);
            parameters.SetDependencies(1.0,
                ParameterType.SideHeight);
            parameters.SetDependencies(25.0,
                ParameterType.BowlRadius);
            parameters.SetDependencies(1.0,
                ParameterType.StalkRadius);
            parameters.SetDependencies(0.0,
                ParameterType.SideAngle);
            parameters.SetDependencies(20.0,
                ParameterType.StandRadius);
            return parameters;
        }

        /// <summary>
        /// Получает параметры бокала со средними значениями.
        /// </summary>
        /// <returns>Параметры со средними значениями.</returns>
        private static Parameters GetAverageParameters()
        {
            return new Parameters();
        }

        /// <summary>
        /// Получает параметры бокала с максимальными значениями.
        /// </summary>
        /// <returns>Параметры с максимальными значениями.</returns>
        private static Parameters GetMaximalParameters()
        {
            var parameters = new Parameters();
            parameters.SetDependencies(100.0,
                ParameterType.StalkHeight);
            parameters.SetDependencies(50.0,
                ParameterType.SideHeight);
            parameters.SetDependencies(50.0,
                ParameterType.BowlRadius);
            parameters.SetDependencies(3.0,
                ParameterType.StalkRadius);
            parameters.SetDependencies(15.0,
                ParameterType.SideAngle);
            parameters.SetDependencies(75.0,
                ParameterType.StandRadius);
            return parameters;
        }
    }
}