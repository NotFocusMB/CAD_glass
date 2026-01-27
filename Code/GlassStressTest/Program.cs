using System;
using Core;
using GlassPluginStressTests;

namespace GlassPluginStressTests
{
    /// <summary>
    /// Главный класс программы для нагрузочного тестирования.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("=== Нагрузочное тестирование плагина бокала ===");
            Console.WriteLine();

            try
            {
                RunTestMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine();
            Console.WriteLine("Тестирование завершено!");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Запускает меню выбора тестов.
        /// </summary>
        private static void RunTestMenu()
        {
            Console.WriteLine("Выберите тип параметров:");
            Console.WriteLine("1 - Минимальные параметры");
            Console.WriteLine("2 - Средние параметры (по умолчанию)");
            Console.WriteLine("3 - Максимальные параметры");
            Console.WriteLine("4 - Все три режима последовательно");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Выберите режим тестирования:");
            Console.WriteLine("1 - По количеству итераций");
            Console.WriteLine("2 - По времени (минуты)");
            Console.Write("Ваш выбор: ");

            string durationChoice = Console.ReadLine();

            int? buildCount = null;
            double? durationMinutes = null;

            if (durationChoice == "1")
            {
                buildCount = ReadBuildCount();
            }
            else if (durationChoice == "2")
            {
                durationMinutes = ReadDurationMinutes();
            }
            else
            {
                Console.WriteLine("Неверный выбор! Используется режим " +
                    "итераций (1000 построений)");
                buildCount = 1000;
            }

            Console.WriteLine();
            Console.WriteLine("Создавать ручку?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            Console.Write("Ваш выбор: ");

            string handleChoice = Console.ReadLine();
            bool createHandle = (handleChoice == "1");

            var tester = new StressTester();

            switch (choice)
            {
                case "1":
                    tester.RunTest("minimal",
                        GetMinimalParameters(),
                        buildCount, durationMinutes, createHandle);
                    break;
                case "2":
                    tester.RunTest("average",
                        GetAverageParameters(),
                        buildCount, durationMinutes, createHandle);
                    break;
                case "3":
                    tester.RunTest("maximal",
                        GetMaximalParameters(),
                        buildCount, durationMinutes, createHandle);
                    break;
                case "4":
                    RunAllTests(tester, buildCount,
                        durationMinutes, createHandle);
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        /// <summary>
        /// Запускает все три теста последовательно.
        /// </summary>
        private static void RunAllTests(StressTester tester,
            int? buildCount, double? durationMinutes, bool createHandle)
        {
            Console.WriteLine("\n=== Запуск всех тестов ===");

            // Для последовательного запуска уменьшаем количество итераций
            if (buildCount.HasValue && buildCount > 100)
            {
                buildCount = buildCount / 3;
                Console.WriteLine($"Количество итераций на тест: {buildCount}");
            }

            Console.WriteLine("\n--- Тест 1: Минимальные параметры ---");
            tester.RunTest("minimal", GetMinimalParameters(),
                buildCount, durationMinutes, createHandle);

            Console.WriteLine("\n--- Тест 2: Средние параметры ---");
            tester.RunTest("average", GetAverageParameters(),
                buildCount, durationMinutes, createHandle);

            Console.WriteLine("\n--- Тест 3: Максимальные параметры ---");
            tester.RunTest("maximal", GetMaximalParameters(),
                buildCount, durationMinutes, createHandle);
        }

        /// <summary>
        /// Считывает количество построений из консоли.
        /// </summary>
        /// <returns>Количество построений или значение по умолчанию.</returns>
        private static int ReadBuildCount()
        {
            Console.Write("\nВведите количество построений: ");

            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Неверное число! Используется " +
                    "значение по умолчанию: 1000");
                return 1000;
            }

            return count;
        }

        /// <summary>
        /// Считывает длительность теста из консоли.
        /// </summary>
        /// <returns>Длительность теста в минутах.</returns>
        private static double ReadDurationMinutes()
        {
            Console.Write("\nВведите длительность теста (минуты): ");

            if (!double.TryParse(Console.ReadLine(),
                out double minutes) || minutes <= 0)
            {
                Console.WriteLine("Неверная длительность! Используется " +
                    "значение по умолчанию: 5 минут");
                return 5;
            }

            return minutes;
        }

        /// <summary>
        /// Получает параметры бокала с минимальными значениями.
        /// </summary>
        /// <returns>Параметры с минимальными значениями.</returns>
        private static Parameters GetMinimalParameters()
        {
            var parameters = new Parameters();

            parameters.SetDependencies(50.0, ParameterType.StalkHeight);
            parameters.SetDependencies(1.0, ParameterType.SideHeight);
            parameters.SetDependencies(25.0, ParameterType.BowlRadius);
            parameters.SetDependencies(1.0, ParameterType.StalkRadius);
            parameters.SetDependencies(0.0, ParameterType.SideAngle);
            parameters.SetDependencies(20.0, ParameterType.StandRadius);

            return parameters;
        }

        /// <summary>
        /// Получает параметры бокала со средними значениями.
        /// </summary>
        /// <returns>Параметры со средними значениями.</returns>
        private static Parameters GetAverageParameters()
        {
            // Используем значения по умолчанию из конструктора Parameters
            return new Parameters();
        }

        /// <summary>
        /// Получает параметры бокала с максимальными значениями.
        /// </summary>
        /// <returns>Параметры с максимальными значениями.</returns>
        private static Parameters GetMaximalParameters()
        {
            var parameters = new Parameters();

            parameters.SetDependencies(100.0, ParameterType.StalkHeight);
            parameters.SetDependencies(50.0, ParameterType.SideHeight);
            parameters.SetDependencies(50.0, ParameterType.BowlRadius);
            parameters.SetDependencies(3.0, ParameterType.StalkRadius);
            parameters.SetDependencies(15.0, ParameterType.SideAngle);
            parameters.SetDependencies(75.0, ParameterType.StandRadius);

            return parameters;
        }
    }
}