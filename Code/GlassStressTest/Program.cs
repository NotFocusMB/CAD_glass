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

            // Используем средние параметры как в методичке
            Parameters parameters = GetAverageParameters();

            Console.WriteLine("Выберите режим тестирования:");
            Console.WriteLine("1 - Бесконечное тестирование (остановка Ctrl+C)");
            Console.WriteLine("2 - Тестирование с фиксированным количеством итераций");
            Console.Write("Ваш выбор: ");

            var choice = Console.ReadLine();
            var tester = new StressTester();

            try
            {
                if (choice == "1")
                {
                    // Бесконечное тестирование
                    tester.RunInfiniteStressTest(parameters);
                }
                else if (choice == "2")
                {
                    // Тестирование с фиксированным количеством итераций
                    Console.Write("\nВведите количество итераций: ");

                    if (int.TryParse(Console.ReadLine(), out int iterations) && iterations > 0)
                    {
                        tester.RunFixedStressTest(parameters, iterations);
                    }
                    else
                    {
                        Console.WriteLine("Неверное число! Используется 100 итераций.");
                        tester.RunFixedStressTest(parameters, 100);
                    }
                }
                else
                {
                    Console.WriteLine("Неверный выбор! Запускается бесконечное тестирование.");
                    tester.RunInfiniteStressTest(parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nКритическая ошибка в программе: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Получает параметры бокала со средними значениями.
        /// </summary>
        /// <returns>Параметры со средними значениями.</returns>
        private static Parameters GetAverageParameters()
        {
            // Используем стандартные значения из конструктора Parameters
            // Это и есть средние значения
            return new Parameters();
        }
    }
}