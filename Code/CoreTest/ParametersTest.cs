using System;
using Core;
using FluentAssertions;
using NUnit.Framework;

namespace CoreTest
{
    [TestFixture]
    public class ParametersTest
    {
        private Parameters _parameters;

        [SetUp]
        public void Setup()
        {
            _parameters = new Parameters();
        }

        [Test(Description = "Проверяет, что конструктор правильно устанавливает начальные значения")]
        public void Constructor_ShouldInitializeCorrectly()
        {
            var bowlRadius = _parameters.NumericalParameters[ParameterType.BowlRadius];
            bowlRadius.Value.Should().Be(35.0);
        }

        [Test(Description = "Проверяет, что SetDependencies правильно обновляет значение")]
        public void SetDependencies_ShouldChangeParameterValue()
        {
            _parameters.SetDependencies(40.0, ParameterType.BowlRadius);
            _parameters.NumericalParameters[ParameterType.BowlRadius].Value.Should().Be(40.0);
        }

        [Test(Description = "Проверяет, что зависимости СУЖАЮТ диапазон")]
        public void SetDependencies_ShouldNarrowLimits()
        {
            _parameters.SetDependencies(30.0, ParameterType.StandRadius);
            var bowlRadius = _parameters.NumericalParameters[ParameterType.BowlRadius];
            bowlRadius.MaxValue.Should().BeApproximately(45.0, 0.1);
        }

        [Test(Description = "Проверяет, что ResetToDefaults возвращает все к 'заводским' настройкам")]
        public void ResetToDefaults_ShouldRestoreInitialValuesAndLimits()
        {
            _parameters.SetDependencies(25.0, ParameterType.StandRadius);
            _parameters.ResetToDefaults();
            var bowlRadiusAfterReset = _parameters.NumericalParameters[ParameterType.BowlRadius];
            bowlRadiusAfterReset.MaxValue.Should().Be(45.0);
        }

        [Test(Description = "Проверяет, что GetLimitsString возвращает правильный формат")]
        public void GetLimitsString_ShouldReturnCorrectFormat()
        {
            var param = _parameters.NumericalParameters[ParameterType.SideAngle];
            param.MinValue = 5.12;
            param.MaxValue = 12.89;
            string result = _parameters.GetLimitsString(ParameterType.SideAngle);
            result.Should().Be("5,1 - 12,9");
        }

        [Test(Description = "Проверяет, что GetLimitsString возвращает 'Конфликт!', если Min > Max")]
        public void GetLimitsString_WhenMinGreaterThanMax_ShouldReturnConflict()
        {
            var param = _parameters.NumericalParameters[ParameterType.SideAngle];
            param.MinValue = 15;
            param.MaxValue = 10;
            string result = _parameters.GetLimitsString(ParameterType.SideAngle);
            result.Should().Be("Конфликт!");
        }

        [Test(Description = "Проверяет, что SetDependencies пробрасывает исключение от Parameter.Value")]
        public void SetDependencies_WhenValueIsInvalid_ShouldThrowArgumentException()
        {
            Action act = () => _parameters.SetDependencies(200.0, ParameterType.StalkHeight);
            act.Should().Throw<ArgumentException>().WithMessage("Value_TooBig");
        }

        [Test(Description = "Проверяет, что зависимости не ломаются при нулевом угле")]
        public void UpdateAllLimits_WhenAngleIsZero_ShouldNotLimitHeight()
        {
            // Arrange
            _parameters.SetDependencies(0, ParameterType.SideAngle);

            // Act
            var sideHeightParam = _parameters.NumericalParameters[ParameterType.SideHeight];

            sideHeightParam.MaxValue.Should().Be(50.0);
        }

        [Test(Description = "Проверяет, что GetLimitsString возвращает N/A для несуществующего ключа")]
        public void GetLimitsString_WithInvalidKey_ShouldReturnNA()
        {
            var invalidType = (ParameterType)999;
            string result = _parameters.GetLimitsString(invalidType);
            result.Should().Be("N/A");
        }
    }
}
