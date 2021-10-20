using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AnglefTests
    {
        private readonly float allowedError = 0.01f;

        #region Constructor tests

        [Test]
        public void ConstructorInputValidation_Within2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Anglef angle = new Anglef(Mathf.PI / 2f);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI / 2f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Beyond2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Anglef angle = new Anglef(3f * Mathf.PI);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeWithin2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Anglef angle = new Anglef(-Mathf.PI);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeBeyond2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Anglef angle = new Anglef(-3f * Mathf.PI);

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Within360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Anglef angle = new Anglef(90f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(90f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Beyond360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Anglef angle = new Anglef(450f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(90f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeWithin360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Anglef angle = new Anglef(-180f, isDeg: true);

            // Act
            float actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(180f).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeBeyond360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Anglef angle = new Anglef(-450f, isDeg: true);

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
            Anglef angleVariable;
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
            Anglef angle = Mathf.PI;

            // Act
            float actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(Mathf.PI).Within(allowedError));
        }

        [Test]
        public void ImplicitToFloatConversion_MathfSinFunction_SinReturnsFloat()
        {
            // Arrange
            Anglef angle = Mathf.PI / 6f;

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
            Anglef angleOne = 0.0f;
            Anglef angleTwo = float.Epsilon;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_IfStatementEqualAngles_ReturnsTrue()
        {
            // Arrange
            Anglef angleOne = Anglef.HalfTurn;
            Anglef angleTwo = Anglef.HalfTurn;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(true));
        }

        [Test]
        public void EqualityOperator_IfStatementUnequalAngles_ReturnsFalse()
        {
            // Arrange
            Anglef angleOne = Anglef.Zero;
            Anglef angleTwo = Anglef.MaxAngle;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(false));
        }

        [Test]
        public void InequalityOperator_IfStatementUnequalAngles_ReturnsTrue()
        {
            // Arrange
            Anglef angleOne = Anglef.QuarterTurn;
            Anglef angleTwo = Anglef.HalfTurn;

            // Act
            bool areNotEqual = angleOne != angleTwo;

            // Assert
            Assert.That(areNotEqual, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_IfStatementEqualAngles_ReturnsFalse()
        {
            // Arrange
            Anglef angleOne = Anglef.QuarterTurn;
            Anglef angleTwo = Anglef.QuarterTurn;

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
            Anglef angleOne = Anglef.HalfTurn;
            Anglef angleTwo = Anglef.QuarterTurn;

            // Act
            bool actual = angleOne < angleTwo;

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void GreaterThanOperator_QuarterTurnGreaterThanHalfTurn_ReturnsFalse()
        {
            // Arrange
            Anglef angleOne = Anglef.QuarterTurn;
            Anglef angleTwo = Anglef.HalfTurn;

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
            Anglef angle = Anglef.QuarterTurn;

            // Act
            Anglef actual = -angle;

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(Anglef.ThreeQuartersTurn.RadValue));
        }

        #endregion

        #region Approximately() tests

        [Test]
        public void Approximately_NearlyEqualAngles_ReturnsTrue()
        {
            // Arrange
            Anglef angleOne = Anglef.Zero;
            Anglef angleTwo = Anglef.Epsilon;

            // Act
            bool areApproximatelyEqual = Anglef.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_NearlyEqualAnglesOnEitherSideOfTwoPIWrapAround_ReturnTrue()
        {
            // Arrange
            Anglef angleOne = Anglef.Epsilon;
            Anglef angleTwo = Anglef.MaxAngle;

            // Act
            bool areApproximatelyEqual = Anglef.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_NotEqualAngles_ReturnsFalse()
        {
            // Arrange
            Anglef angleOne = Anglef.QuarterTurn;
            Anglef angleTwo = Anglef.ThreeQuartersTurn;

            // Act
            bool areApproximatelyEqual = Anglef.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_SingleNullArgument_ReturnsFalse()
        {
            // Arrange
            Anglef angleOne = Anglef.Zero;
            Anglef? angleTwo = null;

            // Act
            bool areApproximatelyEqual = Anglef.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_TwoNullArguments_ReturnsFalse()
        {
            // Arrange
            Anglef? angleOne = null;
            Anglef? angleTwo = null;

            // Act
            bool areApproximatelyEqual = Anglef.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        #endregion

        #region Closer() tests

        [Test]
        public void Closer_QuarterTurn_IsCloserToHalfTurnThanThreeQuartersTurn()
        {
            // Arrange
            Anglef angle = Anglef.QuarterTurn;
            Anglef angleOne = Anglef.HalfTurn;
            Anglef angleTwo = Anglef.ThreeQuartersTurn;

            // Act
            Anglef actual = angle.Closer(angleOne, angleTwo);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angleOne.RadValue));
        }

        [Test]
        public void Closer_OneNullInputAngle_ReturnsOtherValidAngle()
        {
            // Arrange
            Anglef angle = Anglef.Zero;
            Anglef angleOne = Anglef.HalfTurn;
            Anglef? angleNull = null;

            // Act
            Anglef actual = angle.Closer(angleOne, angleNull);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angleOne.RadValue));
        }

        [Test]
        public void Closer_BothInputsNull_ReturnsOriginalAngle()
        {
            // Arrange
            Anglef angle = Anglef.Zero;
            Anglef? nullOne = null;
            Anglef? nullTwo = null;

            // Act
            Anglef actual = angle.Closer(nullOne, nullTwo);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angle.RadValue));
        }

        #endregion

        #region CompareTo() tests

        [Test]
        public void CompareTo_EqualValues_ReturnsZero([Values(float.NegativeInfinity, float.PositiveInfinity, float.NaN, 0.0f)] float value)
        {
            // Arrange
            Anglef angleOne = value;
            Anglef angleTwo = value;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_ThisValueIsGreaterThanOthersValue_ReturnsPositiveOne()
        {
            // Arrange
            Anglef angleOne = Anglef.HalfTurn;
            Anglef angleTwo = Anglef.QuarterTurn;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void CompareTo_ThisValueIsLessThanOthersValue_ReturnsNegativeOne()
        {
            // Arrange
            Anglef angleOne = Anglef.HalfTurn;
            Anglef angleTwo = Anglef.ThreeQuartersTurn;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(-1));
        }

        [Test]
        public void CompareTo_ThisIsNaNWhileOtherHasValue_ReturnNegativeOne()
        {
            // Arrange
            Anglef angleOne = float.NaN;
            Anglef angleTwo = Anglef.Zero;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(-1));
        }

        [Test]
        public void CompareTo_ThisHasValueWhileOtherIsNaN_ReturnPositiveOne()
        {
            // Arrange
            Anglef angleOne = Anglef.Zero;
            Anglef angleTwo = float.NaN;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void CompareTo_OtherIsNull_ReturnsPositiveOne()
        {
            // Arrange
            Anglef angleOne = Anglef.Zero;
            Anglef? angleTwo = null;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(1));
        }

        #endregion

        #region Expel() tests

        [Test]
        public void Expel_NoForbiddenRegion_ReturnsOriginalAngle()
        {
            // Arrange
            Anglef lower = Anglef.Zero;
            Anglef upper = Anglef.Zero;
            Anglef angle = Anglef.QuarterTurn;

            // Act
            Anglef actual = Anglef.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_OutsideForbiddenRegionNormalHierarchyOfRegionBoundaries_ReturnsOriginalAngle()
        {
            // Arrange
            Anglef angle = Mathf.PI / 4f;
            Anglef lower = Anglef.QuarterTurn;
            Anglef upper = Anglef.ThreeQuartersTurn;

            // Act
            Anglef actual = Anglef.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_OutsideForbiddenRegionInvertedHierarchyOfRegionBoundaries_ReturnsOriginalAngle()
        {
            // Arrange
            Anglef angle = Mathf.PI;
            Anglef lower = Anglef.ThreeQuartersTurn;
            Anglef upper = Anglef.QuarterTurn;

            // Act
            Anglef actual = Anglef.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_WithinCloserToLowerNormalHierarchyOfRegionBoundaries_ReturnsLowerBound()
        {
            // Arrange
            Anglef angle = Mathf.PI * 3f / 4f;
            Anglef lower = Anglef.QuarterTurn;
            Anglef upper = Anglef.ThreeQuartersTurn;

            // Act
            Anglef actual = Anglef.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(lower));
        }

        [Test]
        public void Expel_WithinCloserToUpperInvertedHierarchyOfRegionBoundaries_ReturnsUpperBound()
        {
            // Arrange
            Anglef angle = Mathf.PI / 4f;
            Anglef lower = Anglef.ThreeQuartersTurn;
            Anglef upper = Anglef.QuarterTurn;

            // Act
            Anglef actual = Anglef.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(upper));
        }

        #endregion

        #region Inbetween() tests

        [Test]
        public void IsBetween_HalfTurn_IsBetweenQuarterTurnAndThreeQuartersTurn()
        {
            // Arrange
            Anglef angle = Anglef.HalfTurn;
            Anglef lower = Anglef.QuarterTurn;
            Anglef upper = Anglef.ThreeQuartersTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void IsBetween_HalfTurn_IsNotBetweenThreeQuartersTurnAndQuarterTurn()
        {
            // Arrange
            Anglef angle = Anglef.HalfTurn;
            Anglef lower = Anglef.ThreeQuartersTurn;
            Anglef upper = Anglef.QuarterTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void IsBetween_ZeroTurn_IsBetweenThreeQuartersTurnAndQuarterTurn()
        {
            // Arrange
            Anglef angle = Anglef.Zero;
            Anglef lower = Anglef.ThreeQuartersTurn;
            Anglef upper = Anglef.QuarterTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void IsBetween_ZeroTurn_IsNotBetweenQuaterTurnAndThreeQuartersTurn()
        {
            // Arrange
            Anglef angle = Anglef.Zero;
            Anglef lower = Anglef.QuarterTurn;
            Anglef upper = Anglef.ThreeQuartersTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void IsBetween_NullInput_ReturnsFalse()
        {
            // Arrange
            Anglef angle = Anglef.Zero;
            Anglef? lower = Anglef.QuarterTurn;
            Anglef? upper = null;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        #endregion

    }
}
