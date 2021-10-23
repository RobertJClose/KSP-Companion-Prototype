using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AngledTests
    {

        private readonly double allowedError = 0.01;

        #region Constructor tests

        [Test]
        public void ConstructorInputValidation_Within2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angled angle = new Angled(System.Math.PI / 2.0);

            // Act
            double actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(System.Math.PI / 2.0).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Beyond2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angled angle = new Angled(3.0 * System.Math.PI);

            // Act
            double actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(System.Math.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeWithin2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angled angle = new Angled(-System.Math.PI);

            // Act
            double actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(System.Math.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeBeyond2PI_ReturnsEquivalentWithin2PI()
        {
            // Arrange
            Angled angle = new Angled(-3.0 * System.Math.PI);

            // Act
            double actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(System.Math.PI).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Within360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angled angle = new Angled(90.0, isDeg: true);

            // Act
            double actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(90.0).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_Beyond360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angled angle = new Angled(450.0, isDeg: true);

            // Act
            double actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(90.0).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeWithin360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angled angle = new Angled(-180.0, isDeg: true);

            // Act
            double actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(180.0).Within(allowedError));
        }

        [Test]
        public void ConstructorInputValidation_NegativeBeyond360_ReturnsEquivalentWithin360()
        {
            // Arrange
            Angled angle = new Angled(-450.0, isDeg: true);

            // Act
            double actual = angle.DegValue;

            // Assert
            Assert.That(actual, Is.EqualTo(270.0).Within(allowedError));
        }

        #endregion

        #region Implicit conversion tests

        [Test]
        public void ImplicitFromFloatConversion_CopyAssignment_CallsConstructor()
        {
            // Arrange
            Angled angleVariable;
            double floatVariable = System.Math.PI / 3.0;

            // Act
            angleVariable = floatVariable;
            double actual = angleVariable.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(floatVariable).Within(allowedError));
        }

        [Test]
        public void ImplicitFromFloatConversion_CopyInitialisation_CallsConstructor()
        {
            // Arrange
            Angled angle = System.Math.PI;

            // Act
            double actual = angle.RadValue;

            // Assert
            Assert.That(actual, Is.EqualTo(System.Math.PI).Within(allowedError));
        }

        [Test]
        public void ImplicitToFloatConversion_MathfSinFunction_SinReturnsFloat()
        {
            // Arrange
            Angled angle = System.Math.PI / 6.0;

            // Act
            double actual = System.Math.Sin(angle);

            // Assert
            Assert.That(actual, Is.EqualTo(0.5).Within(allowedError));
        }

        [Test]
        public void ImplicitFromDoubleConversion_CopyAssignment_CallsConstructor()
        {
            // Arrange
            Angled angle;
            double d = 1.5;

            // Act
            angle = d;

            // Assert
            Assert.That(angle.RadValue, Is.EqualTo(d));
        }

        [Test]
        public void ImplicitToDoubleConversion_MathSinFunction_SinReturnsDouble()
        {
            // Arrange
            Angled angle = System.Math.PI / 6;

            // Act
            double actual = System.Math.Sin(angle);

            // Assert
            Assert.That(actual, Is.EqualTo(0.5).Within(0.01).Percent);
        }

        [Test]
        public void ImplicitFromAnglefConversion_CopyAssignment_CallsConstructor()
        {
            // Arrange
            Angled angleD;
            Anglef angleF = Anglef.HalfTurn;

            // Act
            angleD = angleF;

            // Assert
            Assert.That(angleD.RadValue, Is.EqualTo((double)angleF.RadValue));
        }

        #endregion

        #region Explicit conversion tests

        [Test]
        public void ExplicitToAnglefCast_CopyAssignment_CallsConstructor()
        {
            // Arrange
            Angled angleD = Angled.HalfTurn;
            Anglef angleF;

            // Act
            angleF = (Anglef)angleD;

            // Assert
            Assert.That(angleF.RadValue, Is.EqualTo((float)angleD.RadValue));
        }

        [Test]
        public void ExplicitToFloatCast_MathfSinFunction_SinReturnsFloat()
        {
            // Arrange
            Angled angle = System.Math.PI / 6.0;

            // Act
            float actual = UnityEngine.Mathf.Sin((float)angle);

            // Assert
            Assert.That(actual, Is.EqualTo(0.5f));
        }

        #endregion

        #region (In)Equality operator tests

        [Test]
        public void EqualityOperator_ApproximatelyEqualAngles_DeemedUnequal()
        {
            // Arrange
            Angled angleOne = 0.0;
            Angled angleTwo = double.Epsilon;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_EqualAngles_ReturnsTrue()
        {
            // Arrange
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.HalfTurn;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(true));
        }

        [Test]
        public void EqualityOperator_UnequalAngles_ReturnsFalse()
        {
            // Arrange
            Angled angleOne = Angled.Zero;
            Angled angleTwo = Angled.MaxAngle;

            // Act
            bool areEqual = angleOne == angleTwo;

            // Assert
            Assert.That(areEqual, Is.EqualTo(false));
        }

        [Test]
        public void InequalityOperator_UnequalAngles_ReturnsTrue()
        {
            // Arrange
            Angled angleOne = Angled.QuarterTurn;
            Angled angleTwo = Angled.HalfTurn;

            // Act
            bool areNotEqual = angleOne != angleTwo;

            // Assert
            Assert.That(areNotEqual, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_EqualAngles_ReturnsFalse()
        {
            // Arrange
            Angled angleOne = Angled.QuarterTurn;
            Angled angleTwo = Angled.QuarterTurn;

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
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.QuarterTurn;

            // Act
            bool actual = angleOne < angleTwo;

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void GreaterThanOperator_QuarterTurnGreaterThanHalfTurn_ReturnsFalse()
        {
            // Arrange
            Angled angleOne = Angled.QuarterTurn;
            Angled angleTwo = Angled.HalfTurn;

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
            Angled angle = Angled.QuarterTurn;

            // Act
            Angled actual = -angle;

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(Angled.ThreeQuartersTurn.RadValue));
        }

        #endregion

        #region Approximately() tests

        [Test]
        public void Approximately_NearlyEqualAngles_ReturnsTrue()
        {
            // Arrange
            Angled angleOne = Angled.Zero;
            Angled angleTwo = Angled.Epsilon;

            // Act
            bool areApproximatelyEqual = Angled.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_NearlyEqualAnglesOnEitherSideOfTwoPIWrapAround_ReturnTrue()
        {
            // Arrange
            Angled angleOne = Angled.Epsilon;
            Angled angleTwo = Angled.MaxAngle;

            // Act
            bool areApproximatelyEqual = Angled.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_NotEqualAngles_ReturnsFalse()
        {
            // Arrange
            Angled angleOne = Angled.QuarterTurn;
            Angled angleTwo = Angled.ThreeQuartersTurn;

            // Act
            bool areApproximatelyEqual = Angled.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_SingleNullArgument_ReturnsFalse()
        {
            // Arrange
            Angled angleOne = Angled.Zero;
            Angled? angleTwo = null;

            // Act
            bool areApproximatelyEqual = Angled.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_TwoNullArguments_ReturnsFalse()
        {
            // Arrange
            Angled? angleOne = null;
            Angled? angleTwo = null;

            // Act
            bool areApproximatelyEqual = Angled.Approximately(angleOne, angleTwo);

            // Assert
            Assert.That(areApproximatelyEqual, Is.EqualTo(false));
        }

        #endregion

        #region Closer() tests

        [Test]
        public void Closer_QuarterTurn_IsCloserToHalfTurnThanThreeQuartersTurn()
        {
            // Arrange
            Angled angle = Angled.QuarterTurn;
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.ThreeQuartersTurn;

            // Act
            Angled actual = angle.Closer(angleOne, angleTwo);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angleOne.RadValue));
        }

        [Test]
        public void Closer_OneNullInputAngle_ReturnsOtherValidAngle()
        {
            // Arrange
            Angled angle = Angled.Zero;
            Angled angleOne = Angled.HalfTurn;
            Angled? angleNull = null;

            // Act
            Angled actual = angle.Closer(angleOne, angleNull);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angleOne.RadValue));
        }

        [Test]
        public void Closer_BothInputsNull_ReturnsOriginalAngle()
        {
            // Arrange
            Angled angle = Angled.Zero;
            Angled? nullOne = null;
            Angled? nullTwo = null;

            // Act
            Angled actual = angle.Closer(nullOne, nullTwo);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(angle.RadValue));
        }

        #endregion

        #region CompareTo() tests

        [Test]
        public void CompareTo_EqualValues_ReturnsZero([Values(float.NegativeInfinity, float.PositiveInfinity, float.NaN, 0.0f)] float value)
        {
            // Arrange
            Angled angleOne = value;
            Angled angleTwo = value;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_ThisValueIsGreaterThanOthersValue_ReturnsPositiveOne()
        {
            // Arrange
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.QuarterTurn;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void CompareTo_ThisValueIsLessThanOthersValue_ReturnsNegativeOne()
        {
            // Arrange
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.ThreeQuartersTurn;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(-1));
        }

        [Test]
        public void CompareTo_ThisIsNaNWhileOtherHasValue_ReturnNegativeOne()
        {
            // Arrange
            Angled angleOne = double.NaN;
            Angled angleTwo = Angled.Zero;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(-1));
        }

        [Test]
        public void CompareTo_ThisHasValueWhileOtherIsNaN_ReturnPositiveOne()
        {
            // Arrange
            Angled angleOne = Angled.Zero;
            Angled angleTwo = double.NaN;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void CompareTo_OtherIsNull_ReturnsPositiveOne()
        {
            // Arrange
            Angled angleOne = Angled.Zero;
            Angled? angleTwo = null;

            // Act
            int actual = angleOne.CompareTo(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(1));
        }

        #endregion

        #region Equals() tests

        [Test]
        public void Equals_EqualAngles_ReturnsTrue()
        {
            // Arrange
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.HalfTurn;

            // Act
            bool actual = angleOne.Equals(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void Equals_DifferentAngles_ReturnsFalse()
        {
            // Arrange
            Angled angleOne = Angled.HalfTurn;
            Angled angleTwo = Angled.QuarterTurn;

            // Act
            bool actual = angleOne.Equals(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_DifferentTypeObject_ReturnsFalse()
        {
            // Arrange
            Angled angle = Angled.HalfTurn;
            string s = "asdf";

            // Act
            bool actual = angle.Equals(s);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_ApproximatelyEqualAngles_DeemedUnequal()
        {
            // Arrange
            Angled angleOne = 0.0f;
            Angled angleTwo = double.Epsilon;

            // Act
            bool actual = angleOne.Equals(angleTwo);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        #endregion

        #region Expel() tests

        [Test]
        public void Expel_NoForbiddenRegion_ReturnsOriginalAngle()
        {
            // Arrange
            Angled lower = Angled.Zero;
            Angled upper = Angled.Zero;
            Angled angle = Angled.QuarterTurn;

            // Act
            Angled actual = Angled.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_OutsideForbiddenRegionNormalHierarchyOfRegionBoundaries_ReturnsOriginalAngle()
        {
            // Arrange
            Angled angle = System.Math.PI / 4.0;
            Angled lower = Angled.QuarterTurn;
            Angled upper = Angled.ThreeQuartersTurn;

            // Act
            Angled actual = Angled.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_OutsideForbiddenRegionInvertedHierarchyOfRegionBoundaries_ReturnsOriginalAngle()
        {
            // Arrange
            Angled angle = System.Math.PI;
            Angled lower = Angled.ThreeQuartersTurn;
            Angled upper = Angled.QuarterTurn;

            // Act
            Angled actual = Angled.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(angle));
        }

        [Test]
        public void Expel_WithinCloserToLowerNormalHierarchyOfRegionBoundaries_ReturnsLowerBound()
        {
            // Arrange
            Angled angle = System.Math.PI * 3.0 / 4.0;
            Angled lower = Angled.QuarterTurn;
            Angled upper = Angled.ThreeQuartersTurn;

            // Act
            Angled actual = Angled.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(lower));
        }

        [Test]
        public void Expel_WithinCloserToUpperInvertedHierarchyOfRegionBoundaries_ReturnsUpperBound()
        {
            // Arrange
            Angled angle = System.Math.PI / 4.0;
            Angled lower = Angled.ThreeQuartersTurn;
            Angled upper = Angled.QuarterTurn;

            // Act
            Angled actual = Angled.Expel(angle, lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(upper));
        }

        #endregion

        #region Inbetween() tests

        [Test]
        public void IsBetween_HalfTurn_IsBetweenQuarterTurnAndThreeQuartersTurn()
        {
            // Arrange
            Angled angle = Angled.HalfTurn;
            Angled lower = Angled.QuarterTurn;
            Angled upper = Angled.ThreeQuartersTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void IsBetween_HalfTurn_IsNotBetweenThreeQuartersTurnAndQuarterTurn()
        {
            // Arrange
            Angled angle = Angled.HalfTurn;
            Angled lower = Angled.ThreeQuartersTurn;
            Angled upper = Angled.QuarterTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void IsBetween_ZeroTurn_IsBetweenThreeQuartersTurnAndQuarterTurn()
        {
            // Arrange
            Angled angle = Angled.Zero;
            Angled lower = Angled.ThreeQuartersTurn;
            Angled upper = Angled.QuarterTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void IsBetween_ZeroTurn_IsNotBetweenQuaterTurnAndThreeQuartersTurn()
        {
            // Arrange
            Angled angle = Angled.Zero;
            Angled lower = Angled.QuarterTurn;
            Angled upper = Angled.ThreeQuartersTurn;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        public void IsBetween_NullInput_ReturnsFalse()
        {
            // Arrange
            Angled angle = Angled.Zero;
            Angled? lower = Angled.QuarterTurn;
            Angled? upper = null;

            // Act
            bool actual = angle.IsBetween(lower, upper);

            // Assert
            Assert.That(actual, Is.EqualTo(false));
        }

        #endregion

        #region IsNaN() tests

        [Test]
        public void IsNaN_AngleIsNotNaN_ReturnsFalse()
        {
            // Arrange
            Angled angle = 0.0;

            // Act
            bool isNaN = Angled.IsNaN(angle);

            // Assert
            Assert.That(isNaN, Is.EqualTo(false));
        }

        [Test]
        public void IsNaN_AngleIsNaN_ReturnsTrue()
        {
            // Arrange
            Angled angle = double.NaN;

            // Act
            bool isNaN = Angled.IsNaN(angle);

            // Assert
            Assert.That(isNaN, Is.EqualTo(true));
        }

        #endregion
    }
}