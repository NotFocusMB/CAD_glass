using System;
using System.Diagnostics;
using System.IO;
using Core;
using GlassPlugin;

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
        private const double GB_CONVERSION =
            0.000000000931322574615478515625;

        /// <summary>
        /// Директория для сохранения логов тестирования.
        /// </summary>
        private readonly string _logsDirectory;

        /// <summary>
        /// Инициализирует новый экземпляр класса StressTester.
        /// </summary>
        public StressTester()
        {
            _logsDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "StressTestLogs");
            Directory.CreateDirectory(_logsDirectory);
        }

        /// <summary>
        /// Выполняет нагрузочное тестирование.
        /// </summary>
        /// <param name="testName">Название теста.</param>
        /// <param name="parameters">Параметры бокала.</param>
        /// <param name="buildCount">
        /// Количество построений (если задано).
        /// </param>
        /// <param name="durationMinutes">
        /// Длительность теста в минутах (если задано).
        /// </param>
        /// <param name="createHandle">
        /// Создавать ручку (по умолчанию - false).
        /// </param>
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
            string filePath = Path.Combine(_logsDirectory, fileName);

            using (var streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("№\tВремя (мс)\tПамять (ГБ)\tСтатус");

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
                        RunIterationTest(builder, parameters, createHandle,
                            buildCount.Value, streamWriter, stopWatch,
                            ref count, ref successfulBuilds, ref failedBuilds);
                    }
                    else if (durationMinutes.HasValue)
                    {
                        RunDurationTest(builder, parameters, createHandle,
                            durationMinutes.Value, streamWriter, stopWatch,
                            testStopwatch, ref count,
                            ref successfulBuilds, ref failedBuilds);
                    }

                    testStopwatch.Stop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Критическая ошибка теста: {ex.Message}");
                }
                finally
                {
                    PrintTestResults(testStopwatch, count,
                        successfulBuilds, failedBuilds, filePath);
                }
            }
        }

        /// <summary>
        /// Выводит параметры теста в консоль.
        /// </summary>
        /// <param name="parameters">Параметры бокала.</param>
        /// <param name="createHandle">Флаг создания ручки.</param>
        private void PrintParameters(Parameters parameters, bool createHandle)
        {
            Console.WriteLine("Параметры бокала:");
            var numParams = parameters.NumericalParameters;

            Console.WriteLine($"  Высота ножки: " +
                $"{numParams[ParameterType.StalkHeight].Value} мм");
            Console.WriteLine($"  Высота стенки: " +
                $"{numParams[ParameterType.SideHeight].Value} мм");
            Console.WriteLine($"  Радиус чаши: " +
                $"{numParams[ParameterType.BowlRadius].Value} мм");
            Console.WriteLine($"  Радиус ножки: " +
                $"{numParams[ParameterType.StalkRadius].Value} мм");
            Console.WriteLine($"  Угол наклона: " +
                $"{numParams[ParameterType.SideAngle].Value}°");
            Console.WriteLine($"  Радиус основания: " +
                $"{numParams[ParameterType.StandRadius].Value} мм");
            Console.WriteLine($"  Ручка: {(createHandle ? "Да" : "Нет")}");
        }

        /// <summary>
        /// Выводит режим тестирования в консоль.
        /// </summary>
        /// <param name="buildCount">Количество итераций.</param>
        /// <param name="durationMinutes">Длительность теста.</param>
        private void PrintTestMode(int? buildCount, double? durationMinutes)
        {
            if (buildCount.HasValue)
            {
                Console.WriteLine($"  Режим: {buildCount.Value} итераций");
            }
            else if (durationMinutes.HasValue)
            {
                Console.WriteLine($"  Режим: {durationMinutes.Value} минут");
            }
        }

        /// <summary>
        /// Выполняет тест с фиксированным количеством итераций.
        /// </summary>
        private void RunIterationTest(GlassBuilder builder,
            Parameters parameters, bool createHandle, int buildCount,
            StreamWriter streamWriter, Stopwatch stopWatch,
            ref int count, ref int successfulBuilds, ref int failedBuilds)
        {
            for (count = 1; count <= buildCount; count++)
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
                    status = $"Ошибка: {ex.Message}";
                    failedBuilds++;
                    timeMs = stopWatch.Elapsed.TotalMilliseconds;
                }

                LogIteration(streamWriter, count, timeMs, status);

                if (count % 10 == 0 || count == buildCount)
                {
                    PrintProgress(count, buildCount, successfulBuilds,
                        failedBuilds, timeMs);
                }

                stopWatch.Reset();

                if (count < buildCount)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Выполняет тест с фиксированной длительностью.
        /// </summary>
        private void RunDurationTest(GlassBuilder builder,
            Parameters parameters, bool createHandle, double durationMinutes,
            StreamWriter streamWriter, Stopwatch stopWatch,
            Stopwatch testStopwatch, ref int count,
            ref int successfulBuilds, ref int failedBuilds)
        {
            var targetDuration = TimeSpan.FromMinutes(durationMinutes);
            count = 0;

            while (testStopwatch.Elapsed < targetDuration)
            {
                count++;
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
                    status = $"Ошибка: {ex.Message}";
                    failedBuilds++;
                    timeMs = stopWatch.Elapsed.TotalMilliseconds;
                }

                LogIteration(streamWriter, count, timeMs, status);

                if (count % 10 == 0)
                {
                    PrintDurationProgress(count, testStopwatch,
                        durationMinutes, successfulBuilds, failedBuilds, timeMs);
                }

                stopWatch.Reset();
                System.Threading.Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Логирует одну итерацию теста в файл.
        /// </summary>
        private void LogIteration(StreamWriter streamWriter,
            int iteration, double timeMs, string status)
        {
            double usedMemory = GetUsedMemoryGB();
            streamWriter.WriteLine($"{iteration}\t{timeMs:F0}\t" +
                $"{usedMemory:F3}\t{status}");
            streamWriter.Flush();
        }

        /// <summary>
        /// Выводит прогресс итерационного теста.
        /// </summary>
        private void PrintProgress(int current, int total,
            int successful, int failed, double timeMs)
        {
            double percentage = (current * 100.0) / total;
            double usedMemory = GetUsedMemoryGB();

            Console.Write($"\rПрогресс: {current}/{total} " +
                $"({percentage:F1}%) | Успешно: {successful} | " +
                $"Ошибки: {failed} | Время: {timeMs:F0} мс | " +
                $"Память: {usedMemory:F2} ГБ");
        }

        /// <summary>
        /// Выводит прогресс теста по времени.
        /// </summary>
        private void PrintDurationProgress(int count,
            Stopwatch testStopwatch, double durationMinutes,
            int successful, int failed, double timeMs)
        {
            var elapsed = testStopwatch.Elapsed;
            var remaining = TimeSpan.FromMinutes(durationMinutes) - elapsed;
            double usedMemory = GetUsedMemoryGB();

            Console.Write($"\rПрогресс: {count} построений | " +
                $"Прошло: {elapsed.Minutes:D2}:{elapsed.Seconds:D2} / " +
                $"{durationMinutes:F1} мин | Осталось: " +
                $"{remaining.Minutes:D2}:{remaining.Seconds:D2} | " +
                $"Успешно: {successful} | Ошибки: {failed} | " +
                $"Время: {timeMs:F0} мс | Память: {usedMemory:F2} ГБ");
        }

        /// <summary>
        /// Выводит итоговые результаты теста.
        /// </summary>
        private void PrintTestResults(Stopwatch testStopwatch,
            int totalCount, int successful, int failed, string filePath)
        {
            var totalTime = testStopwatch.Elapsed;
            Console.WriteLine($"\nТест завершен. Всего построений: {totalCount}");
            Console.WriteLine($"Успешных: {successful}, Ошибок: {failed}");
            Console.WriteLine($"Общее время: {totalTime.Minutes:D2}:" +
                $"{totalTime.Seconds:D2}");

            if (successful > 0)
            {
                double avgTime = totalTime.TotalMilliseconds / successful;
                Console.WriteLine($"Среднее время на построение: " +
                    $"{avgTime:F0} мс");
            }

            Console.WriteLine($"Результаты сохранены в: {filePath}");
        }

        /// <summary>
        /// Получает текущее использование оперативной памяти в ГБ.
        /// </summary>
        /// <returns>Используемая память в гигабайтах.</returns>
        private double GetUsedMemoryGB()
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
    }
}