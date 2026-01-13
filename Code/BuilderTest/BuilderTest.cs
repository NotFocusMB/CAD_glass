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

        [Test(Description = "Проверяет, что публичный ValidateParameters(params) возвращает true для корректных параметров")]
        public void ValidateParameters_WithValidParameters_ShouldReturnTrue()
        {
            var parameters = new Parameters();
            bool result = _builder.ValidateParameters(parameters);
            result.Should().BeTrue();
        }


        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает исключение, если BowlRadius < wallThickness")]
        public void PrivateValidate_WhenBowlRadiusIsLessThanWallThickness_ShouldThrow()
        {
            var privateValidateMethod = typeof(GlassBuilder).GetMethod("ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) },
                null);

            Action act = () => privateValidateMethod.Invoke(_builder, new object[] { 75.0, 40.0, 1.4, 3.0, 10.0, 30.0 });

            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>()
               .WithMessage("Радиус чаши (1,4) должен быть больше толщины стенок (1,50)");
        }

        [Test(Description = "Проверяет, что BuildGlass вызывает исключение, если параметры невалидны")]
        public void BuildGlass_WithInvalidParameters_ShouldThrowValidationError()
        {
            var parameters = new Parameters();
            parameters.NumericalParameters[ParameterType.StalkRadius].Value = 3;
            // Временно расширяем лимиты для теста
            parameters.NumericalParameters[ParameterType.BowlRadius].MinValue = 1.0;
            parameters.NumericalParameters[ParameterType.BowlRadius].Value = 1.4;

            Action act = () => _builder.BuildGlass(parameters);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Ошибка построения бокала:\nРадиус чаши (1,4) должен быть больше толщины стенок (1,50)");
        }


        [Test(Description = "Проверяет, что вызов BuildGlass с null параметрами вызывает исключение")]
        public void BuildGlass_WithNullParameters_ShouldThrow()
        {
            Action act = () => _builder.BuildGlass(null);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Ошибка построения бокала*");
        }

        [Test(Description = "Проверяет, что Reset() не падает при вызове")]
        public void Reset_ShouldNotThrowException()
        {
            Action act = () => _builder.Reset();
            act.Should().NotThrow();
        }

        // Внутри класса BuilderTest

        [Test(Description = "Проверяет, что приватный ValidateParameters вызывает исключение для отрицательной высоты ножки")]
        public void PrivateValidate_WithNegativeStalkHeight_ShouldThrow()
        {
            // Arrange: Получаем доступ к приватному методу
            var privateValidateMethod = typeof(GlassBuilder).GetMethod("ValidateParameters",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) },
                null);

            // Act: Вызываем с отрицательным stalkHeight (-75.0)
            Action act = () => privateValidateMethod.Invoke(_builder, new object[] { -75.0, 40.0, 35.0, 2.0, 10.0, 30.0 });

            // Assert: Ожидаем правильное сообщение об ошибке
            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ArgumentException>()
               .WithMessage("*Высота ножки должна быть положительной*");
        }
    }
}
