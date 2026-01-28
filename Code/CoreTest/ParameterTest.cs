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

        //TODO: remove
        [SetUp]
        public void Setup()
        {
            // Исправлено: Используем конструктор с параметрами
            _parameter = new Parameter(10, 100);
        }

        [Test(Description = "Проверяет, что конструктор выбрасывает исключение" +
                            " при minValue > maxValue")]
        public void Constructor_WhenMinGreaterThanMax_ShouldThrowArgumentException()
        {
            Action act = () => new Parameter(100, 10);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Начальное минимальное значение " +
                "не может быть больше максимального.");
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

            // Используем Contain для частичного совпадения
            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(e => e.Message.Contains("не может быть меньше минимального"));
        }

        [Test(Description = "Проверяет, что значение БОЛЬШЕ MaxValue" +
                            " вызывает правильное исключение")]
        public void Value_SetAboveMaxValue_ShouldThrowArgumentException_WithValueTooBig()
        {
            Action act = () => _parameter.Value = 100.1;

            // Используем Contain для частичного совпадения
            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(e => e.Message.Contains("не может быть больше максимального"));
        }

        [Test(Description = "Проверяет, что после неудачной установки" +
                            " значение остается прежним")]
        public void Value_AfterInvalidSet_ShouldKeepPreviousValue()
        {
            _parameter.Value = 75;
            double originalValue = _parameter.Value;

            Action invalidAction = () => _parameter.Value = 150;

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

        [Test(Description = "Проверяет, что MinValue нельзя установить больше MaxValue")]
        public void MinValue_SetGreaterThanMaxValue_ShouldThrowArgumentException()
        {
            Action act = () => _parameter.MinValue = 101;

            act.Should().Throw<ArgumentException>()
                .WithMessage("Минимальное значение не может быть больше максимального.");
        }

        [Test(Description = "Проверяет, что MaxValue нельзя установить меньше MinValue")]
        public void MaxValue_SetLessThanMinValue_ShouldThrowArgumentException()
        {
            Action act = () => _parameter.MaxValue = 9;

            act.Should().Throw<ArgumentException>()
                .WithMessage("Максимальное значение не может быть меньше минимального.");
        }

        [Test(Description = "Проверяет корректное изменение MinValue")]
        public void MinValue_SetValidValue_ShouldUpdateValue()
        {
            _parameter.MinValue = 20;
            _parameter.MinValue.Should().Be(20);
        }

        [Test(Description = "Проверяет корректное изменение MaxValue")]
        public void MaxValue_SetValidValue_ShouldUpdateValue()
        {
            _parameter.MaxValue = 80;
            _parameter.MaxValue.Should().Be(80);
        }

        [Test(Description = "Проверяет, что можно установить MinValue равное MaxValue")]
        public void MinValue_SetEqualToMaxValue_ShouldBeValid()
        {
            _parameter.MaxValue = 50;
            _parameter.MinValue = 50;

            _parameter.MinValue.Should().Be(50);
            _parameter.MaxValue.Should().Be(50);
        }

        [Test(Description = "Проверяет, что можно установить MaxValue равное MinValue")]
        public void MaxValue_SetEqualToMinValue_ShouldBeValid()
        {
            _parameter.MinValue = 50;
            _parameter.MaxValue = 50;

            _parameter.MinValue.Should().Be(50);
            _parameter.MaxValue.Should().Be(50);
        }

        [Test(Description = "Проверяет, что текущее значение сохраняется" +
                            " при изменении границ диапазона")]
        public void Value_WhenBoundsChanged_ShouldKeepValueIfStillValid()
        {
            _parameter.Value = 50;

            // Расширяем диапазон
            _parameter.MinValue = 40;
            _parameter.MaxValue = 60;

            _parameter.Value.Should().Be(50, "потому что значение 50 " +
                "все еще в диапазоне 40-60");
        }

        [Test(Description = "Проверяет, что при изменении границ, делающих" +
                    " текущее значение невалидным, выбрасывается исключение")]
        public void Value_WhenBoundsChangedToInvalidateCurrentValue_ShouldThrowOnNextAccess()
        {
            _parameter.Value = 50;

            // Сужаем диапазон так, чтобы текущее значение стало невалидным
            _parameter.MaxValue = 45;

            // Проверяем, что при обращении к Value выбрасывается исключение
            Action act = () => { var temp = _parameter.Value; };

            // Обновляем проверку согласно новому сообщению об ошибке
            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(e => e.Message.Contains("не может быть больше максимального"));
        }
    }
}