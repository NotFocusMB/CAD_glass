using System;
using Core;
using FluentAssertions;
using NUnit.Framework;

namespace CoreTest
{
    [TestFixture]
    public class ParameterTest
    {
        private Parameter _parameter;

        [SetUp]
        public void Setup()
        {
            // Исправлено: Используем конструктор с параметрами
            _parameter = new Parameter(10, 100);
        }

        [Test(Description = "Проверяет, что можно установить корректное значение")]
        public void Value_SetValidValue_ShouldSetValue()
        {
            _parameter.Value = 50;
            _parameter.Value.Should().Be(50);
        }

        [Test(Description = "Проверяет, что значение МЕНЬШЕ MinValue" +
                            " вызывает правильное исключение")]
        public void Value_SetBelowMinValue_ShouldThrowArgumentException_WithValueSmall()
        {
            Action act = () => _parameter.Value = 9.9;

            // Исправлено: Ожидаем ArgumentOutOfRangeException и новое сообщение
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Значение (9.9) не может быть меньше" +
                           " минимального (10).*");
        }

        [Test(Description = "Проверяет, что значение БОЛЬШЕ MaxValue" +
                            " вызывает правильное исключение")]
        public void Value_SetAboveMaxValue_ShouldThrowArgumentException_WithValueTooBig()
        {
            Action act = () => _parameter.Value = 100.1;

            // Исправлено: Ожидаем ArgumentOutOfRangeException и новое сообщение
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Значение (100.1) не может быть больше" +
                           " максимального (100).*");
        }

        [Test(Description = "Проверяет, что после неудачной установки" +
                            " значение остается прежним")]
        public void Value_AfterInvalidSet_ShouldKeepPreviousValue()
        {
            _parameter.Value = 75;
            double originalValue = _parameter.Value;

            Action invalidAction = () => _parameter.Value = 150;

            // Исправлено: Ожидаем ArgumentOutOfRangeException
            invalidAction.Should().Throw<ArgumentOutOfRangeException>();
            _parameter.Value.Should().Be(originalValue,
                "потому что новое значение было невалидным");
        }

        [Test(Description = "Проверяет граничные значения (установка" +
                            " MinValue и MaxValue)")]
        public void Value_SetToBoundaries_ShouldBeValid()
        {
            Action setAtMin = () => _parameter.Value = 10;
            setAtMin.Should().NotThrow();
            _parameter.Value.Should().Be(10);

            Action setAtMax = () => _parameter.Value = 100;
            setAtMax.Should().NotThrow();
            _parameter.Value.Should().Be(100);
        }
    }
}
