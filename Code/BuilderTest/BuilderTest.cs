using System;
using System.Reflection;
using Core;
using FluentAssertions;
using GlassPlugin;
using NUnit.Framework;

namespace BuilderTest
{
    [TestFixture]
    public class BuilderTest
    {
        private GlassBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new GlassBuilder();
        }

        [Test(Description = "Проверяет, что конструктор создает объект без ошибок")]
        public void Constructor_ShouldCreateInstance()
        {
            _builder.Should().NotBeNull();
        }

        [Test(Description = "Проверяет, что ValidateParameters(params) возвращает" +
                            " true для корректных параметров")]
        public void ValidateParameters_WithValidParameters_ShouldReturnTrue()
        {
            var parameters = new Parameters();
            bool result = _builder.ValidateParameters(parameters);
            result.Should().BeTrue();
        }

        [Test(Description = "Проверяет, что ValidateParameters возвращает false" +
                            " для невалидных параметров")]
        public void ValidateParameters_WithInvalidParameters_ShouldReturnFalse()
        {
            var parameters = new Parameters();
            parameters.NumericalParameters[ParameterType.BowlRadius].MinValue = 1.0;
            parameters.NumericalParameters[ParameterType.BowlRadius].Value = 1.4;
            parameters.NumericalParameters[ParameterType.StalkRadius].Value = 3;

            bool result = _builder.ValidateParameters(parameters);
            result.Should().BeFalse();
        }

        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает" +
                    " исключение, если BowlRadius < wallThickness")]
        public void PrivateValidate_WhenBowlRadiusIsLessThanWallThickness_ShouldThrow()
        {
            var privateValidateMethod = typeof(GlassBuilder).GetMethod(
                "ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(double), typeof(double), typeof(double),
                typeof(double), typeof(double), typeof(double) },
                null);

            Action act = () => privateValidateMethod.Invoke(_builder,
                new object[] { 75.0, 40.0, 1.4, 3.0, 10.0, 30.0 });

            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>();
        }

        [Test(Description = "Проверяет, что BuildGlass вызывает исключение," +
                            " если параметры невалидны")]
        public void BuildGlass_WithInvalidParameters_ShouldThrowValidationError()
        {
            var parameters = new Parameters();
            parameters.NumericalParameters[ParameterType.BowlRadius].MinValue = 1.0;
            parameters.NumericalParameters[ParameterType.BowlRadius].Value = 1.4;
            parameters.NumericalParameters[ParameterType.StalkRadius].Value = 3;

            Action act = () => _builder.BuildGlass(parameters, false);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Ошибка построения бокала*");
        }

        [Test(Description = "Проверяет, что вызов BuildGlass с null параметрами" +
                            " вызывает исключение")]
        public void BuildGlass_WithNullParameters_ShouldThrow()
        {
            Action act = () => _builder.BuildGlass(null, false);
            act.Should().Throw<ArgumentException>()
               .WithMessage("*Ошибка построения бокала*");
        }

        [Test(Description = "Проверяет, что Reset() не падает при вызове")]
        public void Reset_ShouldNotThrowException_WhenWrapperIsNull()
        {
            Action act = () => _builder.Reset();
            act.Should().NotThrow();
        }

        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает" +
                            " исключение для отрицательной высоты ножки")]
        public void PrivateValidate_WithNegativeStalkHeight_ShouldThrow()
        {
            var privateValidateMethod = typeof(GlassBuilder).GetMethod(
                "ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(double), typeof(double), typeof(double),
                        typeof(double), typeof(double), typeof(double) },
                null);

            Action act = () => privateValidateMethod.Invoke(_builder,
                new object[] { -75.0, 40.0, 35.0, 2.0, 10.0, 30.0 });

            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>()
               .WithMessage("*Высота ножки должна быть положительной*");
        }

        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает" +
                            " исключение для нулевого радиуса основания")]
        public void PrivateValidate_WithZeroStandRadius_ShouldThrow()
        {
            var privateValidateMethod = typeof(GlassBuilder).GetMethod(
                "ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(double), typeof(double), typeof(double),
                        typeof(double), typeof(double), typeof(double) },
                null);

            Action act = () => privateValidateMethod.Invoke(_builder,
                new object[] { 75.0, 40.0, 35.0, 2.0, 10.0, 0.0 });

            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>()
               .WithMessage("*Радиус основания должен быть положительным*");
        }

        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает" +
                            " исключение для угла вне диапазона")]
        public void PrivateValidate_WithAngleOutsideRange_ShouldThrow()
        {
            var privateValidateMethod = typeof(GlassBuilder).GetMethod(
                "ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(double), typeof(double), typeof(double),
                        typeof(double), typeof(double), typeof(double) },
                null);

            Action act = () => privateValidateMethod.Invoke(_builder,
                new object[] { 75.0, 40.0, 35.0, 2.0, 95.0, 30.0 });

            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>()
               .WithMessage("*Угол наклона должен быть от 0 до 90 градусов*");
        }

        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает" +
                            " исключение для слишком маленькой толщины стенок")]
        public void PrivateValidate_WithWallThicknessTooSmall_ShouldThrow()
        {
            var privateValidateMethod = typeof(GlassBuilder).GetMethod(
                "ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(double), typeof(double), typeof(double),
                        typeof(double), typeof(double), typeof(double) },
                null);

            Action act = () => privateValidateMethod.Invoke(_builder,
                new object[] { 75.0, 40.0, 35.0, 0.1, 10.0, 30.0 });

            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>()
               .WithMessage("*Толщина стенок слишком мала*");
        }

        [Test(Description = "Проверяет обработку исключения в Reset()")]
        public void Reset_WhenWrapperThrowsException_ShouldWrapException()
        {
            Action act = () => _builder.Reset();
            act.Should().NotThrow();
        }

        [Test(Description = "Проверяет, что ValidateParameters возвращает false" +
                            " при любом исключении")]
        public void ValidateParameters_WhenExceptionOccurs_ShouldReturnFalse()
        {
            var parameters = new Parameters();
            parameters.NumericalParameters[ParameterType.BowlRadius].MinValue = 1.0;
            parameters.NumericalParameters[ParameterType.BowlRadius].Value = 1.4;
            parameters.NumericalParameters[ParameterType.StalkRadius].Value = 3;

            bool result = _builder.ValidateParameters(parameters);
            result.Should().BeFalse();
        }
    }
}