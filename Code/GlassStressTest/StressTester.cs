using System;
using System.Diagnostics;
using System.IO;
using Core;
using GlassPlugin;

namespace GlassPluginStressTests
{
    public class StressTester
    {
        private const double GB_CONVERSION = 0.000000000931322574615478515625;
        private const double MIN_AVAILABLE_MEMORY_GB = 2.0;

        public void RunTest(string testName, Parameters parameters,
                           int? buildCount, double? durationMinutes,
                           bool createHandle = false)
        {
            Console.WriteLine();
            Console.WriteLine($"=== Начало теста: {testName} ===");
            PrintParameters(parameters, createHandle);
            PrintTestMode(buildCount, durationMinutes);
            Console.WriteLine();

            string fileName = $"log_{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            using (var streamWriter = new StreamWriter(fileName))
            {
                WriteTestHeader(streamWriter, testName, parameters, createHandle);

                var builder = new GlassBuilder();
                var stopWatch = new Stopwatch();
                var testStopwatch = new Stopwatch();
                int count = 0;
                int successfulBuilds = 0;
                int failedBuilds = 0;

                try
                {
                    testStopwatch.Start();

                    if (buildCount.HasValue)
                    {
                        for (count = 1; count <= buildCount.Value; count++)
                        {
                            if (IsMemoryCritical())
                            {
                                HandleMemoryCritical(streamWriter, count);
                                break;
                            }

                            RunSingleBuild(builder, parameters, createHandle,
                                stopWatch, streamWriter, count,
                                ref successfulBuilds, ref failedBuilds);
                        }
                    }
                    else if (durationMinutes.HasValue)
                    {
                        var targetDuration = TimeSpan.FromMinutes(durationMinutes.Value);

                        while (testStopwatch.Elapsed < targetDuration)
                        {
                            count++;

                            if (IsMemoryCritical())
                            {
                                HandleMemoryCritical(streamWriter, count);
                                break;
                            }

                            RunSingleBuild(builder, parameters, createHandle,
                                stopWatch, streamWriter, count,
                                ref successfulBuilds, ref failedBuilds);
                        }
                    }

                    testStopwatch.Stop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nКритическая ошибка: {ex.Message}");
                }
                finally
                {
                    PrintTestResults(testStopwatch, count,
                        successfulBuilds, failedBuilds, fileName);
                }
            }
        }

        private void RunSingleBuild(GlassBuilder builder, Parameters parameters,
            bool createHandle, Stopwatch stopWatch, StreamWriter streamWriter,
            int count, ref int successfulBuilds, ref int failedBuilds)
        {
            string status = "OK";
            double timeMs = 0;

            try
            {
                stopWatch.Start();
                builder.BuildGlass(parameters, createHandle);
                stopWatch.Stop();

                timeMs = stopWatch.Elapsed.TotalMilliseconds;
                successfulBuilds++;
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                status = ex.GetType().Name;
                timeMs = stopWatch.Elapsed.TotalMilliseconds;
                failedBuilds++;
            }

            double usedMemory = GetProcessMemoryGB();
            double availableMemory = GetAvailableMemoryGB();

            streamWriter.WriteLine($"{count}\t{timeMs:F0}\t{usedMemory:F3}\t" +
                $"{availableMemory:F3}\t{status}");
            streamWriter.Flush();

            if (count % 10 == 0)
            {
                Console.Write($"\rПостроений: {count} | " +
                    $"Успешно: {successfulBuilds} | Ошибок: {failedBuilds} | " +
                    $"Время: {timeMs:F0} мс | Память: {usedMemory:F2} ГБ | " +
                    $"Доступно ОЗУ: {availableMemory:F1} ГБ");
            }

            stopWatch.Reset();
            System.Threading.Thread.Sleep(50);
        }

        private bool IsMemoryCritical()
        {
            double availableGB = GetAvailableMemoryGB();
            return availableGB < MIN_AVAILABLE_MEMORY_GB;
        }

        private void HandleMemoryCritical(StreamWriter streamWriter, int count)
        {
            double availableGB = GetAvailableMemoryGB();
            Console.WriteLine($"\n⚠️  КРИТИЧЕСКАЯ НЕХВАТКА ПАМЯТИ!");
            Console.WriteLine($"   Доступно: {availableGB:F1} ГБ");
            Console.WriteLine($"   Тест остановлен.");
            streamWriter.WriteLine($"\nТЕСТ ОСТАНОВЛЕН: нехватка памяти. " +
                $"Доступно: {availableGB:F1} ГБ");
        }

        private void WriteTestHeader(StreamWriter streamWriter,
            string testName, Parameters parameters, bool createHandle)
        {
            var numParams = parameters.NumericalParameters;

            streamWriter.WriteLine($"Тест: {testName}");
            streamWriter.WriteLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            streamWriter.WriteLine($"Параметры: " +
                $"H={numParams[ParameterType.StalkHeight].Value}, " +
                $"H2={numParams[ParameterType.SideHeight].Value}, " +
                $"R={numParams[ParameterType.BowlRadius].Value}, " +
                $"R2={numParams[ParameterType.StalkRadius].Value}, " +
                $"A={numParams[ParameterType.SideAngle].Value}, " +
                $"R3={numParams[ParameterType.StandRadius].Value}");
            streamWriter.WriteLine($"Ручка: {(createHandle ? "Да" : "Нет")}");
            streamWriter.WriteLine("№\tВремя(мс)\tПамять(ГБ)\t" +
                "Доступно ОЗУ(ГБ)\tСтатус");
        }

        private void PrintParameters(Parameters parameters, bool createHandle)
        {
            Console.WriteLine("Параметры:");
            var numParams = parameters.NumericalParameters;
            Console.WriteLine($"  H={numParams[ParameterType.StalkHeight].Value} " +
                $"H2={numParams[ParameterType.SideHeight].Value} " +
                $"R={numParams[ParameterType.BowlRadius].Value} " +
                $"R2={numParams[ParameterType.StalkRadius].Value} " +
                $"A={numParams[ParameterType.SideAngle].Value} " +
                $"R3={numParams[ParameterType.StandRadius].Value}");
            Console.WriteLine($"  Ручка: {(createHandle ? "Да" : "Нет")}");
        }

        private void PrintTestMode(int? buildCount, double? durationMinutes)
        {
            if (buildCount.HasValue)
            {
                Console.WriteLine($"Режим: {buildCount.Value} итераций");
            }
            else if (durationMinutes.HasValue)
            {
                Console.WriteLine($"Режим: {durationMinutes.Value} минут");
            }
        }

        private void PrintTestResults(Stopwatch testStopwatch,
            int totalCount, int successful, int failed, string filePath)
        {
            var totalTime = testStopwatch.Elapsed;
            Console.WriteLine($"\n=== Результаты ===");
            Console.WriteLine($"Всего построений: {totalCount}");
            Console.WriteLine($"Успешно: {successful}, Ошибок: {failed}");
            Console.WriteLine($"Общее время: {totalTime:hh\\:mm\\:ss}");

            if (successful > 0)
            {
                Console.WriteLine($"Среднее время: " +
                    $"{totalTime.TotalMilliseconds / successful:F0} мс");
            }

            Console.WriteLine($"Файл лога: {filePath}");
        }

        private double GetProcessMemoryGB()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                return process.WorkingSet64 * GB_CONVERSION;
            }
            catch
            {
                return 0;
            }
        }

        private double GetAvailableMemoryGB()
        {
            try
            {
                var pc = new PerformanceCounter("Memory", "Available Bytes");
                float availableBytes = pc.NextValue();
                return availableBytes * GB_CONVERSION;
            }
            catch
            {
                return 0;
            }
        }
    }
}