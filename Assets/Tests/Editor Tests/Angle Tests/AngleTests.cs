using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AngleTests
    {
        private readonly float allowedError = 0.01f;

        #region Constructor tests

        [Test]
        public void ConstructorInputValidation_Within2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angle angle = new Angle(Mathf.PI / 2f);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI / 2f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Beyond2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angle angle = new Angle(3f * Mathf.PI);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeWithin2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angle angle = new Angle(-Mathf.PI);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeBeyond2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angle angle = new Angle(-3f * Mathf.PI);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Within360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angle angle = new Angle(90f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(90f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Beyond360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angle angle = new Angle(450f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(90f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeWithin360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angle angle = new Angle(-180f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(180f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeBeyond360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angle angle = new Angle(-450f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(270f).Within(allowedError));
        }

        #endregion

        #region Implicit conversion tests

        [Test]
        public void ImplicitFromFloatConversion_CopyAssignment_CallsConstructor()
        {
            // Arrange
            Angle angleVariable;
            float floatVariable = Mathf.PI / 3f;

            // Act
            angleVariable = floatVariable;
            float actual = angleVariable.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(floatVariable).Within(allowedError));
        }

        [Test]
        public void ImplicitFromFloatConversion_CopyInitialisation_CallsConstructor()
        {
            // Arrange
            Angle angle = Mathf.PI;

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ImplicitToFloatConversion_MathfSinFunction_SinReturnsFloat()
        {
            // Arrange
            Angle angle = Mathf.PI / 6f;

            // Act
            float actual = Mathf.Sin(angle);

            // Assert
            Assert.That(actual, Is.EqualTo(0.5f).Within(allowedError));
        }

        #endregion

        #region (In)Equality operator tests
        
        [Test]
        public void EqualityOperator_EpsilonDifferenceBetweenAngles_DeemedUnequal()
        {
            // Arrange
            Angle angleOne = 0.0f;
            Angle angleTwo = float.Epsilon;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_IfStatementEqualAngles_ReturnsTrue()
        {
            // Arrange
            Angle angleOne = Angle.HalfTurn;
            Angle angleTwo = Angle.HalfTurn;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(true));
        }

        [Test]
        public void EqualityOperator_IfStatementUnequalAngles_ReturnsFalse()
        {
            // Arrange
            Angle angleOne = Angle.Zero;
            Angle angleTwo = Angle.MaxAngle;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(false));
        }

        [Test]
        public void InequalityOperator_IfStatementUnequalAngles_ReturnsTrue()
        {
            // Arrange
            Angle angleOne = Angle.QuarterTurn;
            Angle angleTwo = Angle.HalfTurn;

            // Act
            bool areNotEqual = angleOne != angleTwo;

            // Assert
            Assert.That(areNotEqual, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_IfStatementEqualAngles_ReturnsFalse()
        {
            // Arrange
            Angle angleOne = Angle.QuarterTurn;
            Angle angleTwo = Angle.QuarterTurn;

            // Act
            bool areNotEqual = angleOne != angleTwo;

            // Assert
            Assert.That(areNotEqual, Is.EqualTo(false));
        }

        #endregion

        #region Comparison operator tests

        [Test]
        public void LessThanOperator_HalfTurnLessThanQuarterTurn_ReturnsFalse()
        {
            // Arrange
            Angle angleOne = Angle.HalfTurn;
            Angle angleTwo = Angle.QuarterTurn;

            // Act
            bool actual = angleOne < angleTwo;

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void GreaterThanOperator_QuarterTurnGreaterThanHalfTurn_ReturnsFalse()
        {
            // Arrange
            Angle angleOne = Angle.QuarterTurn;
            Angle angleTwo = Angle.HalfTurn;

            // Act
            bool actual = angleOne > angleTwo;

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        #endregion

        #region Unary minus operator tests

        [Test]
        public void UnaryMinusOperator_MinusQuarterTurn_ReturnsThreeQuartersTurn()
        {
            // Arrange
            Angle angle = Angle.QuarterTurn;

            // Act
            Angle actual = -angle;

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(Angle.ThreeQuartersTurn.RadValue));
        }

        #endregion

        #region Approximately() tests

        [Test]
        public void Approximately_NearlyEqualAngles_ReturnsTrue()
        {
            // Arrange
            Angle angleOne = Angle.Zero;
            Angle angleTwo = Angle.Epsilon;

            // Act
            bool areApproximatelyEqual = Angle.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_NearlyEqualAnglesOnEitherSideOfTwoPIWrapAround_ReturnTrue()
        {
            // Arrange
            Angle angleOne = Angle.Epsilon;
            Angle angleTwo = Angle.MaxAngle;

            // Act
            bool areApproximatelyEqual = Angle.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_NotEqualAngles_ReturnsFalse()
        {
            // Arrange
            Angle angleOne = Angle.QuarterTurn;
            Angle angleTwo = Angle.ThreeQuartersTurn;

            // Act
            bool areApproximatelyEqual = Angle.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_SingleNullArgument_ReturnsFalse()
        {
            // Arrange
            Angle angleOne = Angle.Zero;
            Angle? angleTwo = null;

            // Act
            bool areApproximatelyEqual = Angle.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_TwoNullArguments_ReturnsFalse()
        {
            // Arrange
            Angle? angleOne = null;
            Angle? angleTwo = null;

            // Act
            bool areApproximatelyEqual = Angle.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        #endregion

        #region Closer() tests

        [Test]
        public void Closer_QuarterTurn_IsCloserToHalfTurnThanThreeQuartersTurn()
        {
            // Arrange
            Angle angle = Angle.QuarterTurn;
            Angle angleOne = Angle.HalfTurn;
            Angle angleTwo = Angle.ThreeQuartersTurn;

            // Act
            Angle actual = angle.Closer(angleOne, angleTwo);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angleOne.RadValue));
        }

        [Test]
        public void Closer_OneNullInputAngle_ReturnsOtherValidAngle()
        {
            // Arrange
            Angle angle = Angle.Zero;
            Angle angleOne = Angle.HalfTurn;
            Angle? angleNull = null;

            // Act
            Angle actual = angle.Closer(angleOne, angleNull);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angleOne.RadValue));
        }

        [Test]
        public void Closer_BothInputsNull_ReturnsOriginalAngle()
        {
            // Arrange
            Angle angle = Angle.Zero;
            Angle? nullOne = null;
            Angle? nullTwo = null;

            // Act
            Angle actual = angle.Closer(nullOne, nullTwo);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angle.RadValue));
        }

        #endregion

        #region Expel() tests

        [Test]
        public void Expel_NoForbiddenRegion_ReturnsOriginalAngle()
        {
            // Arrange
            Angle lower = Angle.Zero;
            Angle upper = Angle.Zero;
            Angle angle = Angle.QuarterTurn;

            // Act
            Angle actual = Angle.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_OutsideForbiddenRegionNormalHierarchyOfRegionBoundaries_ReturnsOriginalAngle()
        {
            // Arrange
            Angle angle = Mathf.PI / 4f;
            Angle lower = Angle.QuarterTurn;
            Angle upper = Angle.ThreeQuartersTurn;

            // Act
            Angle actual = Angle.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_OutsideForbiddenRegionInvertedHierarchyOfRegionBoundaries_ReturnsOriginalAngle()
        {
            // Arrange
            Angle angle = Mathf.PI;
            Angle lower = Angle.ThreeQuartersTurn;
            Angle upper = Angle.QuarterTurn;

            // Act
            Angle actual = Angle.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_WithinCloserToLowerNormalHierarchyOfRegionBoundaries_ReturnsLowerBound()
        {
            // Arrange
            Angle angle = Mathf.PI * 3f / 4f;
            Angle lower = Angle.QuarterTurn;
            Angle upper = Angle.ThreeQuartersTurn;

            // Act
            Angle actual = Angle.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(lower));
        }

        [Test]
        public void Expel_WithinCloserToUpperInvertedHierarchyOfRegionBoundaries_ReturnsUpperBound()
        {
            // Arrange
            Angle angle = Mathf.PI / 4f;
            Angle lower = Angle.ThreeQuartersTurn;
            Angle upper = Angle.QuarterTurn;

            // Act
            Angle actual = Angle.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(upper));
        }

        #endregion

        #region Inbetween() tests

        [Test]
        public void IsBetween_HalfTurn_IsBetweenQuarterTurnAndThreeQuartersTurn()
        {
            // Arrange
            Angle angle = Angle.HalfTurn;
            Angle lower = Angle.QuarterTurn;
            Angle upper = Angle.ThreeQuartersTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void IsBetween_HalfTurn_IsNotBetweenThreeQuartersTurnAndQuarterTurn()
        {
            // Arrange
            Angle angle = Angle.HalfTurn;
            Angle lower = Angle.ThreeQuartersTurn;
            Angle upper = Angle.QuarterTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void IsBetween_ZeroTurn_IsBetweenThreeQuartersTurnAndQuarterTurn()
        {
            // Arrange
            Angle angle = Angle.Zero;
            Angle lower = Angle.ThreeQuartersTurn;
            Angle upper = Angle.QuarterTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void IsBetween_ZeroTurn_IsNotBetweenQuaterTurnAndThreeQuartersTurn()
        {
            // Arrange
            Angle angle = Angle.Zero;
            Angle lower = Angle.QuarterTurn;
            Angle upper = Angle.ThreeQuartersTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void IsBetween_NullInput_ReturnsFalse()
        {
            // Arrange
            Angle angle = Angle.Zero;
            Angle? lower = Angle.QuarterTurn;
            Angle? upper = null;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        #endregion

    }
}
