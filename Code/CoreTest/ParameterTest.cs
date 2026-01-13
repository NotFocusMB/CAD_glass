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
            _parameter = new Parameter
            {
                MinValue = 10,
                MaxValue = 100
            };
        }

        [Test(Description = "Проверяет, что можно установить корректное значение")]
        public void Value_SetValidValue_ShouldSetValue()
        {
            // Act
            _parameter.Value = 50;

            // Assert
            _parameter.Value.Should().Be(50);
        }

        [Test(Description = "Проверяет, что значение МЕНЬШЕ MinValue вызывает правильное исключение")]
        public void Value_SetBelowMinValue_ShouldThrowArgumentException_WithValueSmall()
        {
            // Arrange: создаем "действие", которое должно вызвать ошибку
            Action act = () => _parameter.Value = 9.9;

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value_small");
        }

        [Test(Description = "Проверяет, что значение БОЛЬШЕ MaxValue вызывает правильное исключение")]
        public void Value_SetAboveMaxValue_ShouldThrowArgumentException_WithValueTooBig()
        {
            // Arrange
            Action act = () => _parameter.Value = 100.1;

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value_TooBig");
        }

        [Test(Description = "Проверяет, что после неудачной установки значение остается прежним")]
        public void Value_AfterInvalidSet_ShouldKeepPreviousValue()
        {
            // Arrange: устанавливаем начальное корректное значение
            _parameter.Value = 75;
            double originalValue = _parameter.Value;

            // Act: пытаемся установить некорректное значение
            Action invalidAction = () => _parameter.Value = 150;

            // Assert: проверяем, что исключение было, а значение не изменилось
            invalidAction.Should().Throw<ArgumentException>();
            _parameter.Value.Should().Be(originalValue, "потому что новое значение было невалидным");
        }

        [Test(Description = "Проверяет граничные значения (установка MinValue и MaxValue)")]
        public void Value_SetToBoundaries_ShouldBeValid()
        {
            // Act & Assert
            Action setAtMin = () => _parameter.Value = 10;
            setAtMin.Should().NotThrow();
            _parameter.Value.Should().Be(10);

            Action setAtMax = () => _parameter.Value = 100;
            setAtMax.Should().NotThrow();
            _parameter.Value.Should().Be(100);
        }
    }
}
