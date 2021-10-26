using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class OrbitTests
    {
        private readonly double _absoluteAllowedErrorOnAngles = 0.001;

        #region Property tests

        [Test]
        public void APESetter_NaNInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void APESetter() => orbit.APE = double.NaN;

            // Assert
            Assert.That(APESetter, Throws.ArgumentException);
        }

        [Test]
        public void ApoapsisPointGetter_EllipticalOrbit_ReturnsApoapsisPoint()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;
            Vector3d apoapsisPoint = orbit.TrueAnomaly2Point(Angled.HalfTurn);

            // Act
            Vector3d actual = orbit.ApoapsisPoint.Value;

            // Assert
            Assert.That(actual.x, Is.EqualTo(apoapsisPoint.x).Within(0.01).Percent);
            Assert.That(actual.y, Is.EqualTo(apoapsisPoint.y).Within(0.01).Percent);
            Assert.That(actual.z, Is.EqualTo(apoapsisPoint.z).Within(0.01).Percent);
        }

        [Test]
        public void ApoapsisPointGetter_OpenOrbit_ReturnsNull([Values(1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;

            // Act
            Vector3d? actual = orbit.ApoapsisPoint;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void ApoapsisRadiusGetter_EllipticalOrbit_ReturnsApoapsisRadius()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;
            double apoapsisRadius = orbit.TrueAnomaly2Point(Angled.HalfTurn).magnitude;

            // Act
            double actual = orbit.ApoapsisRadius;

            // Assert
            Assert.That(actual, Is.EqualTo(apoapsisRadius).Within(0.01).Percent);
        }

        [Test]
        public void ApoapsisRadiusGetter_OpenOrbit_ReturnsPositiveInifinity([Values(1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;

            // Act
            double actual = orbit.ApoapsisRadius;

            // Assert
            Assert.That(actual, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void AscendingNodeGetter_NodeDoesExist_ReturnsAscendingNode()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;
            orbit.INC = 1.2;
            Vector3d ascendingNode = orbit.TrueAnomaly2Point(-orbit.APE);

            // Act
            Vector3d? actual = orbit.AscendingNode;

            // Assert
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value.x, Is.EqualTo(ascendingNode.x).Within(0.01).Percent);
            Assert.That(actual.Value.y, Is.EqualTo(ascendingNode.y).Within(0.01).Percent);
            Assert.That(actual.Value.z, Is.EqualTo(ascendingNode.z).Within(0.01).Percent);
        }

        [Test]
        public void AscendingNodeGetter_NodeDoesNotExist_ReturnsNull()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            orbit.INC = 1.2;
            orbit.APE = Angled.HalfTurn;

            // Act
            Vector3d? actual = orbit.AscendingNode;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void DescendingNodeGetter_NodeDoesExist_ReturnsAscendingNode()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;
            orbit.INC = 1.2;
            orbit.APE = 3.0;
            orbit.LAN = Angled.HalfTurn;
            Vector3d descendingNode = orbit.TrueAnomaly2Point(-orbit.APE + Angled.HalfTurn);

            // Act
            Vector3d? actual = orbit.DescendingNode;

            // Assert
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value.x, Is.EqualTo(descendingNode.x).Within(0.01).Percent);
            Assert.That(actual.Value.y, Is.EqualTo(descendingNode.y).Within(0.01).Percent);
            Assert.That(actual.Value.z, Is.EqualTo(descendingNode.z).Within(0.01).Percent);
        }

        [Test]
        public void DescendingNodeGetter_NodeDoesNotExist_ReturnsNull()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            orbit.INC = 1.2;

            // Act
            Vector3d? actual = orbit.DescendingNode;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void ECCSetter_InputLessThanZero_GetsClampedUpToZero()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            
            // Act
            orbit.ECC = -1.0;
            double actual = orbit.ECC;

            // Assert
            Assert.That(actual, Is.EqualTo(0.0));
        }

        [Test]
        public void ECCSetter_InputIsNaN_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void ECCSetter() => orbit.ECC = double.NaN;

            // Assert
            Assert.That(ECCSetter, Throws.ArgumentException);
        }

        [Test]
        public void EccentricityVectorGetter_ReturnsVectorThatPointsAtPeriapsis([Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;
            Vector3d periapsis = orbit.PeriapsisPoint;

            // Act
            Vector3d eccentricityVector = orbit.EccentricityVector;
            double angleBetween = Vector3d.Angle(eccentricityVector, periapsis);
            bool isAngleBetweenVectorsApproxZero = Mathd.Approximately(angleBetween, 0.0);

            // Assert
            Assert.That(isAngleBetweenVectorsApproxZero, Is.True);
        }

        [Test]
        public void EccentricityVectorGetter_ReturnsVectorWithMagnitudeEqualToEccentricity([Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;

            // Act
            Vector3d actual = orbit.EccentricityVector;
            bool isActualMagnitudeApproxEqualToEccentricity = Mathd.Approximately(actual.magnitude, orbit.ECC);

            // Assert
            Assert.That(isActualMagnitudeApproxEqualToEccentricity, Is.True);
        }

        [Test]
        public void ExcessVelocityGetter_EllipticalOrbit_ReturnsNull()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;

            // Act
            double? actual = orbit.ExcessVelocity;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void ExcessVelocityGetter_ParabolicOrbit_ReturnsZero()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.0;

            // Act
            double? excessVelocity = orbit.ExcessVelocity;

            // Assert
            Assert.That(excessVelocity.HasValue, Is.True);
            Assert.That(excessVelocity.Value, Is.EqualTo(0.0));
        }

        [Test]
        public void ExcessVelocityGetter_HyperbolicOrbit_ReturnsExcessVelocity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            double trueValue = System.Math.Sqrt(orbit.Mu / -orbit.SemiMajorAxis);

            // Act
            double? actual = orbit.ExcessVelocity;
            bool isActualAprroxCorrect = Mathd.Approximately(actual.Value, trueValue);

            // Assert
            Assert.That(isActualAprroxCorrect, Is.True);
        }

        [Test]
        public void INCSetter_NaNInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void INCSetterNaNInput() => orbit.INC = double.NaN;

            // Assert
            Assert.That(INCSetterNaNInput, Throws.ArgumentException);
        }

        [Test]
        public void INCSetter_InputIsLargerThanPI_ClampsDownToPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            orbit.INC = 3.0 / 2.0 * System.Math.PI;
            bool isInclinationApproxPI = Angled.Approximately(orbit.INC, Angled.HalfTurn);

            // Assert
            Assert.That(isInclinationApproxPI, Is.True);
        }

        [Test]
        public void LANSetter_NaNInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void LANSetterNaNInput() => orbit.LAN = double.NaN;

            // Assert
            Assert.That(LANSetterNaNInput, Throws.ArgumentException);
        }

        [Test]
        public void MaxTrueAnomalyGetter_EllipticalOrbit_ReturnsNull()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;

            // Act
            Angled? actual = orbit.MaxTrueAnomaly;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void MaxTrueAnomalyGetter_ParabolicOrbit_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.0;

            // Act
            Angled? actual = orbit.MaxTrueAnomaly;
            bool isActualApproxPI = Angled.Approximately(actual.Value, Angled.HalfTurn);

            // Assert
            Assert.That(isActualApproxPI, Is.True);
        }

        [Test]
        public void MaxTrueAnomalyGetter_HyperbolicOrbit_ReturnsMaxTrueAnomalyValue()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            Angled result = System.Math.Acos(-1.0 / orbit.ECC);

            // Act
            Angled actual = orbit.MaxTrueAnomaly.Value;
            bool isActualApproxCorrect = Angled.Approximately(actual, result);

            // Assert
            Assert.That(isActualApproxCorrect, Is.True);
        }

        [Test]
        public void MeanMotionGetter_ReturnsCorrectValues([Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;
            double trueMeanMotion = (orbit.ECC == 1) ? System.Math.Sqrt(orbit.Mu) : System.Math.Sqrt(orbit.Mu / System.Math.Pow(System.Math.Abs(orbit.SemiMajorAxis), 3));

            // Act
            double actual = orbit.MeanMotion;

            // Assert
            Assert.That(actual, Is.EqualTo(trueMeanMotion).Within(0.01).Percent);
        }

        [Test]
        public void NodalVectorGetter_NonZeroInclination_ReturnsVectorThatPointsAtAscendingNode([Values(0.0, 1.5, 2.5, 3.5, 4.5)] double longitudeOfAscendingNode)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.INC = 0.5;
            orbit.LAN = longitudeOfAscendingNode;
            Vector3d ascendingNode = orbit.AscendingNode.Value;

            // Act
            Vector3d actual = orbit.NodalVector;
            double angleBetween = Vector3d.Angle(actual, ascendingNode);

            // Assert
            Assert.That(angleBetween, Is.EqualTo(0.0).Within(1e-5));
        }

        [Test]
        public void NodalVectorGetter_NonZeroInclination_ReturnsVectorWithMagnitudeEqualToSinInclination([Values(0.5, 1.5, 2.5)] double inclination)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.INC = inclination;

            // Act
            Vector3d actual = orbit.NodalVector;

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(System.Math.Sin(orbit.INC)).Within(0.01).Percent);
        }

        [Test]
        public void NodalVectorGetter_ZeroInclination_ReturnsZeroVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.INC = Angled.Zero;

            // Act
            Vector3d actual = orbit.NodalVector;

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(0.0));
        }

        [Test]
        public void OrbitTypeGetter_ReturnsConicSectionEnumValueOfCorrectType([Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;

            // Act
            Orbit.ConicSection actual = orbit.OrbitType;

            // Assert
            if (orbit.ECC < 1.0)
                Assert.That(actual, Is.EqualTo(Orbit.ConicSection.Elliptical));
            else if (orbit.ECC == 1.0)
                Assert.That(actual, Is.EqualTo(Orbit.ConicSection.Parabolic));
            else
                Assert.That(actual, Is.EqualTo(Orbit.ConicSection.Hyperbolic));
        }

        [Test]
        public void PeriapsisPointGetter_ReturnsPeriapsisPoint([Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;
            orbit.INC = 0.5;
            orbit.LAN = 1.5;
            orbit.APE = 2.5;
            Vector3d periapsisPoint = orbit.EccentricityVector.normalized * orbit.RPE;

            // Act
            Vector3d actual = orbit.PeriapsisPoint;

            // Assert
            Assert.That(actual.x, Is.EqualTo(periapsisPoint.x).Within(0.01).Percent);
            Assert.That(actual.y, Is.EqualTo(periapsisPoint.y).Within(0.01).Percent);
            Assert.That(actual.z, Is.EqualTo(periapsisPoint.z).Within(0.01).Percent);
        }

        [Test]
        public void PeriodGetter_EllipticalOrbit_ReturnsOrbitalPeriod()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;
            double truePeriod = 2.0 * System.Math.PI / orbit.MeanMotion;

            // Act
            double actual = orbit.Period;

            // Assert
            Assert.That(actual, Is.EqualTo(truePeriod).Within(0.01).Percent);
        }

        [Test]
        public void PeriodGetter_OpenOrbit_ReturnsPositiveInfinity([Values(1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;

            // Act
            double actual = orbit.Period;

            // Assert
            Assert.That(actual, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void RPESetter_NaNInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void RPESetterNaNInput() => orbit.RPE = double.NaN;

            // Assert
            Assert.That(RPESetterNaNInput, Throws.ArgumentException);
        }

        [Test]
        public void RPESetter_NegativeInput_ClampsUpToZero()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            orbit.RPE = -1.0;

            // Assert
            Assert.That(orbit.RPE, Is.EqualTo(0.0));
        }

        [Test]
        public void SemiLatusRectumGetter_ReturnsCorrectValue()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            double trueSLR = orbit.RPE * (1.0 + orbit.ECC);

            // Act
            double actual = orbit.SemiLatusRectum;

            // Assert
            Assert.That(actual, Is.EqualTo(trueSLR).Within(0.01).Percent);
        }

        [Test]
        public void SemiMajorAxisGetter_ParabolicOrbit_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.0;

            // Act
            double actual = orbit.SemiMajorAxis;

            // Assert
            Assert.That(actual, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void SemiMajorAxisGetter_EllipticalOrbit_ReturnsCorrectValueThatIsGreaterThanZero()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;
            double trueSMA = orbit.RPE / (1.0 - orbit.ECC);

            // Act
            double actual = orbit.SemiMajorAxis;

            // Assert
            Assert.That(actual, Is.EqualTo(trueSMA).Within(0.01).Percent);
        }

        [Test]
        public void SemiMajorAxisGetter_HyperbolicOrbit_ReturnsCorrectValueThatIsLessThanZero()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            double trueSMA = orbit.RPE / (1.0 - orbit.ECC);

            // Act
            double actual = orbit.SemiMajorAxis;

            // Assert
            Assert.That(actual, Is.EqualTo(trueSMA).Within(0.01).Percent);
        }

        [Test]
        public void SpecificAngularMomentumVectorGetter_ReturnsVectorThatIsPerpendicularToOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            var (points, _) = orbit.OrbitalPoints(0.0, 0.0, 0.5);

            // Act
            Vector3d actual = orbit.SpecificAngularMomentumVector;

            // Assert
            foreach (var point in points)
            {
                double angleBetween = Vector3d.Angle(point, actual);
                Assert.That(angleBetween, Is.EqualTo(90.0).Within(0.01).Percent);
            }
        }

        [Test]
        public void SpecificAngularMomentumVectorGetter_ReturnsVectorWithMagnitudeEqualToSpecificAngularMomentumOfOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled trueAnomaly = Angled.QuarterTurn;
            Vector3d specificAngularMomentum = Vector3d.Cross(orbit.TrueAnomaly2Point(trueAnomaly), orbit.TrueAnomaly2Velocity(trueAnomaly));

            // Act
            Vector3d actual = orbit.SpecificAngularMomentumVector;

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(specificAngularMomentum.magnitude).Within(0.01).Percent);
        }

        [Test]
        public void SpecificEnergyGetter_ReturnsCorrectValue([Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = eccentricity;
            System.Collections.Generic.List<Angled> trueAnomalies = new System.Collections.Generic.List<Angled>() { 0.5, 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0, 5.5, 6.0 };

            // Act
            double actual = orbit.SpecificEnergy;

            // Assert
            foreach (var trueAnomaly in trueAnomalies)
            {
                double specificEnergy = 0.5 * System.Math.Pow(orbit.TrueAnomaly2Velocity(trueAnomaly).magnitude, 2) - orbit.Mu / orbit.TrueAnomaly2Point(trueAnomaly).magnitude;
                Assert.That(actual, Is.EqualTo(specificEnergy).Within(1e-5));
            }
        }

        [Test]
        public void TPPSetter_NaNInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void TPPSetterWithNaNInput() => orbit.TPP = double.NaN;

            // Assert
            Assert.That(TPPSetterWithNaNInput, Throws.ArgumentException);
        }

        #endregion

        #region (In)equality operator tests

        [Test]
        public void EqualityOperator_SameOrbit_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(true));
        }

        [Test]
        public void EqualityOperator_DifferentOrbitsSameGravitationalBody_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.RPE = 1_500_000.0;
            orbitTwo.RPE = 1_000_000.0;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_SameOrbitButDifferentGravitationalBody_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Mun.DefaultOrbit;
            orbitTwo.RPE = orbitOne.RPE;
            orbitTwo.ECC = orbitOne.ECC;
            orbitTwo.INC = orbitOne.INC;
            orbitTwo.APE = orbitOne.APE;
            orbitTwo.LAN = orbitOne.LAN;
            orbitTwo.TPP = orbitOne.TPP;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_OneOrbitIsNull_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_BothOrbitsAreNull_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = null;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(true));
        }

        [Test]
        public void EqualityOperator_ApproximatelyEqualOrbits_DeemedUnequal()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.ECC = 0.0;
            orbitTwo.ECC = double.Epsilon;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void EqualityOperator_EllipticalOrbitsWithTimeOfPeriapsisPassageDifferentByOnePeriod_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.ECC = 0.5;
            orbitTwo.ECC = 0.5;
            orbitOne.TPP = 100.0;
            orbitTwo.TPP = orbitOne.TPP + orbitOne.Period;

            // Act
            bool areOrbitsEqual = orbitOne == orbitTwo;

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_SameOrbit_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(false));
        }

        [Test]
        public void InequalityOperator_DifferentOrbitsSameGravitationalBody_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.RPE = 1_500_000.0;
            orbitTwo.RPE = 1_000_000.0;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_SameOrbitButDifferentGravitationalBody_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Mun.DefaultOrbit;
            orbitTwo.RPE = orbitOne.RPE;
            orbitTwo.ECC = orbitOne.ECC;
            orbitTwo.INC = orbitOne.INC;
            orbitTwo.APE = orbitOne.APE;
            orbitTwo.LAN = orbitOne.LAN;
            orbitTwo.TPP = orbitOne.TPP;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_OneOrbitIsNull_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_BothOrbitsAreNull_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = null;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(false));
        }

        [Test]
        public void InequalityOperator_ApproximatelyEqualOrbits_DeemedUnequal()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.ECC = 0.0;
            orbitTwo.ECC = double.Epsilon;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(true));
        }

        [Test]
        public void InequalityOperator_EllipticalOrbitsWithTimeOfPeriapsisPassageDifferentByOnePeriod_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.ECC = 0.5;
            orbitTwo.ECC = 0.5;
            orbitOne.TPP = 100.0;
            orbitTwo.TPP = orbitOne.TPP + orbitOne.Period;

            // Act
            bool areOrbitsDifferent = orbitOne != orbitTwo;

            // Assert
            Assert.That(areOrbitsDifferent, Is.EqualTo(false));
        }

        #endregion

        #region Approximately() tests

        [Test]
        public void Approximately_NearlyEqualOrbits_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitTwo.RPE = orbitOne.RPE + double.Epsilon;
            orbitTwo.ECC = orbitOne.ECC - double.Epsilon;
            orbitTwo.INC = orbitOne.INC + Anglef.Epsilon;
            orbitTwo.APE = orbitOne.APE - Anglef.Epsilon;
            orbitTwo.LAN = orbitOne.LAN + Anglef.Epsilon;
            orbitTwo.TPP = orbitOne.TPP - double.Epsilon;

            // Act
            bool areOrbitsApproximatelyEqual = Orbit.Approximately(orbitOne, orbitTwo);

            // Assert
            Assert.That(areOrbitsApproximatelyEqual, Is.EqualTo(true));
        }

        [Test]
        public void Approximately_VeryDifferentOrbits_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.RPE = 1_500_000.0;
            orbitTwo.RPE = 2_000_000.0;

            // Act
            bool areOrbitsApproximatelyEqual = Orbit.Approximately(orbitOne, orbitTwo);

            // Assert
            Assert.That(areOrbitsApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_EitherOrbitIsNull_ReturnsFalse()
        {
            // Arrange 
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsApproximatelyEqual = Orbit.Approximately(orbitOne, orbitTwo);

            // Assert
            Assert.That(areOrbitsApproximatelyEqual, Is.EqualTo(false));
        }

        [Test]
        public void Approximately_BothOrbitsAreNull_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = null;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsApproximatelyEqual = Orbit.Approximately(orbitOne, orbitTwo);

            // Assert
            Assert.That(areOrbitsApproximatelyEqual, Is.EqualTo(true));
        }

        #endregion

        #region Equals() tests

        [Test]
        public void Equals_SameOrbit_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            bool areOrbitsEqual = orbitOne.Equals(orbitTwo);

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(true));
        }

        [Test]
        public void Equals_DifferentOrbitsSameGravitationalBody_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.RPE = 1_500_000.0;
            orbitTwo.RPE = 1_000_000.0;

            // Act
            bool areOrbitsEqual = orbitOne.Equals(orbitTwo);

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_SameOrbitButDifferentGravitationalBody_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Mun.DefaultOrbit;
            orbitTwo.RPE = orbitOne.RPE;
            orbitTwo.ECC = orbitOne.ECC;
            orbitTwo.INC = orbitOne.INC;
            orbitTwo.APE = orbitOne.APE;
            orbitTwo.LAN = orbitOne.LAN;
            orbitTwo.TPP = orbitOne.TPP;

            // Act
            bool areOrbitsEqual = orbitOne.Equals(orbitTwo);

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_OtherOrbitIsNull_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = null;

            // Act
            bool areOrbitsEqual = orbitOne.Equals(orbitTwo);

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_OtherObjectIsNotAnOrbitObject_ReturnsFalse()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            double notAnOrbit = 1.0;

            // Act
            bool areObjectsEqual = orbitOne.Equals(notAnOrbit);

            // Assert
            Assert.That(areObjectsEqual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_ApproximatelyEqualOrbits_DeemedUnequal()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.ECC = 0.0;
            orbitTwo.ECC = double.Epsilon;

            // Act
            bool areOrbitsEqual = orbitOne.Equals(orbitTwo);

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(false));
        }

        [Test]
        public void Equals_EllipticalOrbitsWithTimeOfPeriapsisPassageDifferentByOnePeriod_ReturnsTrue()
        {
            // Arrange
            Orbit orbitOne = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit orbitTwo = GravitationalBody.Kerbin.DefaultOrbit;
            orbitOne.ECC = 0.5;
            orbitTwo.ECC = 0.5;
            orbitOne.TPP = 100.0;
            orbitTwo.TPP = orbitOne.TPP + orbitOne.Period;

            // Act
            bool areOrbitsEqual = orbitOne.Equals(orbitTwo);

            // Assert
            Assert.That(areOrbitsEqual, Is.EqualTo(true));
        }

        #endregion

        #region FindTransferOrbit() tests

        [Test]
        public void FindTransferOrbit_EllipticalOrbit_ReconstructsOrbit()
        {
            // Arrange
            Orbit original = new Orbit(725_000.0, 0.35, 0.436332313f, 0.6981317008f, 1.221730476f, 2_000.0, GravitationalBody.Kerbin);

            double timeOne = 1_000.0;
            double timeTwo = 3_200.0;

            // Act
            Orbit actual = Orbit.FindTransferOrbit(original, timeOne, original, timeTwo);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(original.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(original.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(original.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(original.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(original.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(original.TPP).Within(0.01).Percent);
        }

        [Test]
        public void FindTransferOrbit_HyperbolicOrbit_ReconstructsOrbit()
        {
            // Arrange
            Orbit original = new Orbit(725_000.0, 1.5, 0.436332313f, 0.6981317008f, 1.221730476f, 2_000.0, GravitationalBody.Kerbin);

            double departureTime = 4_000.0;
            double arrivalTime = 5_330.0;

            // Act
            Orbit actual = Orbit.FindTransferOrbit(original, departureTime, original, arrivalTime);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(original.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(original.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(original.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(original.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(original.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(4_111.428195).Within(original.TPP).Percent);
        }

        [Test]
        public void FindTransferOrbit_ParabolicOrbit_ReconstructsOrbit()
        {
            // Arrange
            Orbit original = new Orbit(725_000.0, 1.0, 0.436332313f, 0.6981317008f, 1.221730476f, 2_000.0, GravitationalBody.Kerbin);

            double departureTime = 500.0;
            double arrivalTime = 5_000.0;

            // Act
            Orbit actual = Orbit.FindTransferOrbit(original, departureTime, original, arrivalTime);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(original.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(original.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(original.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(original.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(original.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(4_111.428195).Within(original.TPP).Percent);
        }

        [Test]
        public void FindTransferOrbit_OrbitsAroundDifferentBodies_ReturnsNull()
        {
            // Arrange
            Orbit initial = GravitationalBody.Earth.DefaultOrbit;
            Orbit target = GravitationalBody.Kerbin.DefaultOrbit;
            double departureTime = 0.0;
            double arrivalTime = 1.0;

            // Act
            Orbit actual = Orbit.FindTransferOrbit(initial, departureTime, target, arrivalTime);
            
            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void FindTransferOrbit_ZeroTimeOfFlightWithDistinctDepartureAndArrivalPoints_ReturnsNull()
        {
            // Arrange
            Orbit initial = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit target = GravitationalBody.Kerbin.DefaultOrbit;
            target.RPE = 10_000_000.0;
            double departureTime = 0.0;
            double arrivalTime = 0.0;

            // Act
            Orbit actual = Orbit.FindTransferOrbit(initial, departureTime, target, arrivalTime);

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void FindTransferOrbit_ZeroTimeOfFlightWithEqualDepartureAndArrivalPoints_ReturnsInitialOrbit()
        {
            // Arrange
            Orbit initial = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit target = GravitationalBody.Kerbin.DefaultOrbit;
            target.INC = 0.5f;
            double departureTime = 0.0;
            double arrivalTime = 0.0;

            // Act
            Orbit actual = Orbit.FindTransferOrbit(initial, departureTime, target, arrivalTime);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(initial.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(initial.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(initial.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(initial.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(initial.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(initial.TPP).Within(0.01).Percent);
        }

        #endregion

        #region OrbitalPoints() tests

        [Test]
        public void OrbitalPoints_TwoPoints_MatchesTrueAnomaly2PointOutput()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled startTrueAnomaly = 0.1;
            Angled endTrueAnomaly = 0.2;
            Angled step = endTrueAnomaly - startTrueAnomaly;
            Vector3d pointOne = orbit.TrueAnomaly2Point(startTrueAnomaly);
            Vector3d pointTwo = orbit.TrueAnomaly2Point(endTrueAnomaly);
            System.Collections.Generic.List<Vector3d> resultPoints = new System.Collections.Generic.List<Vector3d>() { pointOne, pointTwo };

            // Act
            var (points, _) = orbit.OrbitalPoints(startTrueAnomaly, endTrueAnomaly, step);

            // Assert
            for (int i = 0; i < points.Count; i++)
            {
                Assert.That(points[i].x, Is.EqualTo(resultPoints[i].x).Within(0.01).Percent);
                Assert.That(points[i].y, Is.EqualTo(resultPoints[i].y).Within(0.01).Percent);
                Assert.That(points[i].z, Is.EqualTo(resultPoints[i].z).Within(0.01).Percent);
            }
        }

        [Test]
        public void OrbitalPoints_TwoPoints_FirstOutputPointTrueAnomalyIsInputStartTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.0;
            Angled end = 1.0;
            Angled step = 1.0;

            // Act
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, step);

            // Assert
            Assert.That(trueAnomalies[0].RadValue, Is.EqualTo(start.RadValue));
        }

        [Test]
        public void OrbitalPoints_TwoPoints_FinalOutputPointTrueAnomalyIsInputEndTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.0;
            Angled end = 1.0;
            Angled step = 1.0;

            // Act
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, step);

            // Assert
            Assert.That(trueAnomalies[1].RadValue, Is.EqualTo(end.RadValue));
        }

        [Test]
        public void OrbitalPoints_ManyPoints_OutputListOfPointsAndOutputListOfTrueAnomaliesMatch()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.0;
            Angled end = 1.0;
            int numPoints = 8;
            Angled step = (end - start) / numPoints;

            // Act
            var (points, trueAnomalies) = orbit.OrbitalPoints(start, end, step);
            System.Collections.Generic.List<Vector3d> trueAnomalies2Points = trueAnomalies.ConvertAll((trueAnomaly) => orbit.TrueAnomaly2Point(trueAnomaly)); 

            // Assert
            Assert.That(points.Count, Is.EqualTo(trueAnomalies.Count));
            for (int i = 0; i < points.Count; i++)
            {
                Assert.That(points[i].x, Is.EqualTo(trueAnomalies2Points[i].x).Within(0.01).Percent);
                Assert.That(points[i].y, Is.EqualTo(trueAnomalies2Points[i].y).Within(0.01).Percent);
                Assert.That(points[i].z, Is.EqualTo(trueAnomalies2Points[i].z).Within(0.01).Percent);
            }
        }

        [Test]
        public void OrbitalPoints_EntireOrbit_FinalPointTrueAnomalyIsDifferentFromFirstPointTrueAnomaly(
            [Values(0.5, 1.0, 1.5)] double eccentricity)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Bop.DefaultOrbit;
            orbit.ECC = eccentricity;
            var (_, trueAnomalies) = orbit.OrbitalPoints(null, null, Constants.OrbitDefaultStepRad);

            // Act
            double difference = trueAnomalies[trueAnomalies.Count - 1] - trueAnomalies[0];

            // Assert
            Assert.That(difference, Is.Not.EqualTo(0.0));
        }

        [Test]
        public void OrbitalPoints_HyperbolicOrbitPointsBeyondMaxTrueAnomaly_InfiniteMagnitudeOutputPoints()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            Angled startTrueAnomaly = orbit.MaxTrueAnomaly.Value + 0.1;
            Angled endTrueAnomaly = -orbit.MaxTrueAnomaly.Value - 0.1;
            int numOfPoints = 5;
            Angled step = (endTrueAnomaly - startTrueAnomaly) / numOfPoints;

            // Act
            var (points, _) = orbit.OrbitalPoints(startTrueAnomaly, endTrueAnomaly, step);

            // Assert
            foreach (var point in points)
            {
                Assert.That(point.magnitude, Is.EqualTo(double.PositiveInfinity));
            }
        }

        [Test]
        public void OrbitalPoints_AngularStepLargerThanDifferenceBetweenStartAndEndTrueAnomalies_OnlyStartPointAndEndPointIsOutput()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.1;
            Angled end = 0.2;
            Angled step = 0.3;

            // Act
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, step);

            // Assert
            Assert.That(trueAnomalies.Count, Is.EqualTo(2));
            Assert.That(trueAnomalies[0].RadValue, Is.EqualTo(start.RadValue));
            Assert.That(trueAnomalies[1].RadValue, Is.EqualTo(end.RadValue));
        }

        [Test]
        public void OrbitalPoints_InputAngularStepDoesNotWhollyDivideRange_ActualStepBetweenPointsIsLessThanInputStep()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.0;
            Angled end = 1.0;
            Angled inputStep = 0.3;
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, inputStep);

            // Act
            Angled actualStep = trueAnomalies[1] - trueAnomalies[0];

            // Assert
            Assert.That(actualStep.RadValue, Is.LessThan(inputStep.RadValue));
        }

        [Test]
        public void OrbitalPoints_InputAngularStepDoesNotWhollyDivideRange_TrueAnomalyOfFinalPointIsInputEndTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.0;
            Angled end = 1.0;
            Angled inputStep = 0.3;

            // Act
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, inputStep);
            Angled finalTrueAnomaly = trueAnomalies[trueAnomalies.Count - 1];

            // Assert
            Assert.That(finalTrueAnomaly.RadValue, Is.EqualTo(end.RadValue));
        }

        [Test]
        public void OrbitalPoints_OutputtingEntireOrbit_FirstAndLastOutputPointsAreApproximatelyEqual()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 1.0;
            Angled end = 1.0;
            Angled step = 0.1;

            // Act
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, step);
            double differenceBetweenFirstAndLastPointAngle = System.Math.Abs(trueAnomalies[0] - trueAnomalies[trueAnomalies.Count - 1]);

            // Assert
            Assert.That(differenceBetweenFirstAndLastPointAngle, Is.LessThanOrEqualTo(1e-10));
        }

        [TestCase(null, 0.0)]
        [TestCase(0.0, null)]
        [TestCase(double.NaN, 0.0)]
        [TestCase(0.0, double.NaN)]
        public void OrbitalPoints_InputStartOrEndTrueAnomalyIsNullOrNaN_OutputsEntireOrbit(double? start, double? end)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled step = 0.01;

            // Act
            var (_, trueAnomalies) = orbit.OrbitalPoints(start, end, step);
            double differenceBetweenFirstAndLastPointAngle = System.Math.Abs(trueAnomalies[0] - trueAnomalies[trueAnomalies.Count - 1]);

            // Assert
            Assert.That(differenceBetweenFirstAndLastPointAngle, Is.LessThanOrEqualTo(1e-10));
        }

        [Test]
        public void OrbitalPoints_NaNInputStepValue_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled start = 0.0;
            Angled end = 1.0;
            Angled step = double.NaN;

            // Act
            void OrbitalPoints() => orbit.OrbitalPoints(start, end, step);

            // Assert
            Assert.That(OrbitalPoints, Throws.ArgumentException);
        }

        #endregion

        #region StateVectors2Orbit() tests

        [Test]
        public void StateVectors2Orbit_EllipticalCaseInputFromTime2PointAndTime2VelocityMethods_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1_125_000.0;
            orbit.ECC = 0.5;
            orbit.INC = 2.0f;
            orbit.APE = 5.5f;
            orbit.LAN = 4.5f;
            orbit.TPP = 6000.0;

            double time = 6200.0;
            Vector3d position = orbit.Time2Point(time);
            Vector3d velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01).Percent);
        }

        [Test]
        public void StateVectors2Orbit_ParabolicCaseInputFromTime2PointAndTime2VelocityMethods_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000.0;
            orbit.ECC = 1.0;
            orbit.INC = 0.0f;
            orbit.APE = 0.0f;
            orbit.LAN = 0.0f;
            orbit.TPP = 6000.0;

            double time = 6200.0;
            Vector3d position = orbit.Time2Point(time);
            Vector3d velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01).Percent);
        }

        [Test]
        public void StateVectors2Orbit_HyperbolicCaseInputFromTime2PointAndTime2VelocityMethods_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000;
            orbit.ECC = 1.4;
            orbit.INC = 0.5f;
            orbit.APE = 3f;
            orbit.LAN = 5f;
            orbit.TPP = 2500.0;

            double time = 7500.0;
            Vector3d position = orbit.Time2Point(time);
            Vector3d velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01).Percent);
        }

        [Test]
        public void StateVectors2Orbit_ParabolicHandwrittenInput_MatchesHandwrittenWork()
        {
            // Arrange

            // The inputs
            Vector3d position = new Vector3d(1_000_000.0, 100_000.0, 10_000.0);
            Vector3d velocity = new Vector3d(1_500.0, 2_000.0, 881.929_010_7);
            double time = 500.0;

            // The correct final orbit
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 591_617.833_5;
            orbit.ECC = 1.0;
            orbit.INC = 0.4394082206f;
            orbit.APE = 4.914028278f;
            orbit.LAN = 0.07849867002f;
            orbit.TPP = 147.0558678;

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Kerbin, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01).Percent);
        }

        #endregion

        #region Time2Point() tests

        [Test]
        public void Time2Point_EllipticalOrbit_MatchesHandwrittenResult()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 0.5;
            orbit.INC = 0.5f;
            orbit.APE = 1.2f;
            orbit.LAN = 3.0f;
            orbit.TPP = 500;
            double time = 1e+4;

            // Act
            Vector3d actual = orbit.Time2Point(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(-26_298_864.41).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-84_814_355.77).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(47_898_093.35).Within(0.1).Percent);
        }

        [Test]
        public void Time2Point_ParabolicOrbit_MatchesHandwrittenResult()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.0;
            orbit.INC = 0.5f;
            orbit.APE = 1.2f;
            orbit.LAN = 3.0f;
            orbit.TPP = 500;
            double time = 1e+4;

            // Act
            Vector3d actual = orbit.Time2Point(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(-23_165_600.92).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-86_415_299.48).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(48_522_383.85).Within(0.1).Percent);
        }

        [Test]
        public void Time2Point_HyperbolicOrbit_MatchesHandwrittenResult()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5f;
            orbit.APE = 1.2f;
            orbit.LAN = 3.0f;
            orbit.TPP = 500;
            double time = 1e+4;

            // Act
            Vector3d actual = orbit.Time2Point(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(-20_405_151.09).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-87_826_547.68).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(49_072_822.08).Within(0.1).Percent);
        }

        [Test]
        public void Time2Point_EllipticalOrbitInfiniteTimeInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            void Time2PointTestDelegate() => orbit.Time2Point(double.PositiveInfinity);

            // Assert
            Assert.That(Time2PointTestDelegate, Throws.ArgumentException);
        }

        [Test]
        public void Time2Point_ParabolicOrbitInfiniteTimeInput_ReturnsPositiveInfinityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.0;
            double time = double.PositiveInfinity;

            // Act
            Vector3d actual = orbit.Time2Point(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(double.PositiveInfinity));
            Assert.That(actual.y, Is.EqualTo(double.PositiveInfinity));
            Assert.That(actual.z, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void Time2Point_HyperbolicOrbitInfiniteTimeInput_ReturnsPositiveInfinityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.5;
            double time = double.PositiveInfinity;

            // Act
            Vector3d actual = orbit.Time2Point(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(double.PositiveInfinity));
            Assert.That(actual.y, Is.EqualTo(double.PositiveInfinity));
            Assert.That(actual.z, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void Time2Point_NaNTimeInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            double time = double.NaN;

            // Act
            void Time2PointTestDelegate() => orbit.Time2Point(time);

            // Assert
            Assert.That(Time2PointTestDelegate, Throws.ArgumentException);
        }

        #endregion

        #region Time2TrueAnomaly() tests

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitLowEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 0.4;
            orbit.TPP = 1500.0;

            double time = -10000.0;
            Angled result = 3.032453601d;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitHighEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 0.95;
            orbit.TPP = 1500.0;

            double time = -10000.0;
            Angled result = -2.284620281;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitVeryHighEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 0.9999;
            orbit.TPP = 1_500.0;

            double time = -10000.0;
            Angled result = -2.249168754;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 1.0;
            orbit.TPP = 3_000.0;

            double time = 5_000.0;
            Angled result = 1.288831085;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitPositiveInfinity_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0;

            double time = double.PositiveInfinity;
            Angled result = Angled.HalfTurn;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitNegativeInfinity_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0;

            double time = double.NegativeInfinity;
            Angled result = Angled.HalfTurn;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitVeryLargePositiveTime_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0;

            double time = 1e+35;
            Angled result = Angled.HalfTurn;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitVeryLargeNegativeTime_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0;

            double time = -1e+35;
            Angled result = Angled.HalfTurn;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 1.5;
            orbit.TPP = 2_000.0;

            double time = 100.0;
            Angled result = -1.269606064;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitPositiveInfinity_ReturnsMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            double time = double.PositiveInfinity;
            Angled result = orbit.MaxTrueAnomaly.Value;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitNegativeInfinity_Returns2PIMinusMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            double time = double.NegativeInfinity;
            Angled result = 2.0 * System.Math.PI - orbit.MaxTrueAnomaly.Value;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitPositiveVeryLargeTime_ReturnsMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            double time = 1e+30;
            Angled result = orbit.MaxTrueAnomaly.Value;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitNegativeVeryLargeTime_Returns2PIMinusMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            double time = -1e+30;
            Angled result = 2.0 * System.Math.PI - orbit.MaxTrueAnomaly.Value;

            // Act
            Angled actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue).Within(_absoluteAllowedErrorOnAngles));
        }

        #endregion

        #region Time2Velocity() tests

        [Test]
        public void Time2Velocity_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 0.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            double time = 1e+4;

            // Act
            Vector3d actual = orbit.Time2Velocity(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(2285.40472).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-789.057048).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(250.5584854).Within(0.1).Percent);
        }

        [Test]
        public void Time2Velocity_ParabolicOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.0;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            double time = 1e+4;

            // Act
            Vector3d actual = orbit.Time2Velocity(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(2610.632316).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-957.648237).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(316.5065052).Within(0.1).Percent);
        }

        [Test]
        public void Time2Velocity_HyperbolicOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            double time = 1e+4;

            // Act
            Vector3d actual = orbit.Time2Velocity(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(2897.205314).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-1106.502648).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(375.0780042).Within(0.1).Percent);
        }

        [Test]
        public void Time2Velocity_EllipticalOrbitInfiniteTimeInput_ThrowsArgumentException(
            [Values(double.PositiveInfinity, double.NegativeInfinity)] double time)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 0.5;

            // Act
            void Time2VelocityTestDelegate() => orbit.Time2Velocity(time);

            // Assert
            Assert.That(Time2VelocityTestDelegate, Throws.ArgumentException);
        }

        [Test]
        public void Time2Velocity_ParabolicOrbitTimeIsInfinite_ReturnsZeroVector(
            [Values(double.PositiveInfinity, double.NegativeInfinity)] double time)
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.0;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;

            // Act
            Vector3d actual = orbit.Time2Velocity(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(Vector3d.zero.x).Within(1e-3));
            Assert.That(actual.y, Is.EqualTo(Vector3d.zero.y).Within(1e-3));
            Assert.That(actual.z, Is.EqualTo(Vector3d.zero.z).Within(1e-3));
        }

        [Test]
        public void Time2Velocity_HyperbolicOrbitTimeIsPositiveInfinity_ReturnsEscapeVelocityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            double time = double.PositiveInfinity;

            // Act
            Vector3d actual = orbit.Time2Velocity(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(1369.958045).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(244.3150799).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(-237.7500712).Within(0.1).Percent);
        }

        [Test]
        public void Time2Velocity_HyperbolicOrbitTimeIsNegativeInfinity_ReturnsEntryVelocityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            double time = double.NegativeInfinity;

            // Act
            Vector3d actual = orbit.Time2Velocity(time);

            // Assert
            Assert.That(actual.x, Is.EqualTo(477.4406487).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-1183.646565).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(603.3500122).Within(0.1).Percent);
        }

        [Test]
        public void Time2Velocity_NaNTimeInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            double time = double.NaN;

            // Act
            void Time2VelocityTestDelegate() => orbit.Time2Velocity(time);

            // Assert
            Assert.That(Time2VelocityTestDelegate, Throws.ArgumentException);
        }

        #endregion

        #region TrueAnomaly2Point() tests

        [Test]
        public void TrueAnomaly2Point_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange 
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 1e+6;
            orbit.ECC = 0.5;
            orbit.INC = System.Math.PI / 4.0;
            orbit.APE = System.Math.PI / 2.0;
            orbit.LAN = System.Math.PI;

            Anglef trueAnomaly = 0.5f;

            double xResult = 499821.1467;
            double yResult = -646943.6416;
            double zResult = 646943.6416;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(xResult).Within(0.01).Percent);
            Assert.That(actual.y, Is.EqualTo(yResult).Within(0.01).Percent);
            Assert.That(actual.z, Is.EqualTo(zResult).Within(0.01).Percent);
        }

        [Test]
        public void TrueAnomaly2Point_ParabolicOrbitNormalCase_MatchesHandwrittenWork()
        {
            // Arrange 
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 1e+6;
            orbit.ECC = 1.0;
            orbit.INC = System.Math.PI / 4.0;
            orbit.APE = System.Math.PI / 2.0;
            orbit.LAN = System.Math.PI;

            Angled trueAnomaly = 0.5;

            double xResult = 510683.8424;
            double yResult = -661003.7749;
            double zResult = 661003.7749;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(xResult).Within(0.01).Percent);
            Assert.That(actual.y, Is.EqualTo(yResult).Within(0.01).Percent);
            Assert.That(actual.z, Is.EqualTo(zResult).Within(0.01).Percent);
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitNormalCase_MatchesHandwrittenWork()
        {
            // Arrange 
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 1e+6;
            orbit.ECC = 3.0;
            orbit.INC = System.Math.PI / 4.0;
            orbit.APE = System.Math.PI / 2.0;
            orbit.LAN = System.Math.PI;

            Anglef trueAnomaly = 0.5f;

            double xResult = 527893.0221;
            double yResult = -683278.4815;
            double zResult = 683278.4815;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(xResult).Within(0.01).Percent);
            Assert.That(actual.y, Is.EqualTo(yResult).Within(0.01).Percent);
            Assert.That(actual.z, Is.EqualTo(zResult).Within(0.01).Percent);
        }

        [Test]
        public void TrueAnomaly2Point_ParabolicOrbitTrueAnomalyIsPI_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.0;

            Angled trueAnomaly = System.Math.PI;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0;

            Angled trueAnomaly = orbit.MaxTrueAnomaly.Value;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsMinTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0;

            Angled trueAnomaly = -orbit.MaxTrueAnomaly.Value;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsBeyondMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0;

            Angled trueAnomaly = (System.Math.PI + orbit.MaxTrueAnomaly.Value) / 2.0;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsBeyondMinTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0;

            Angled trueAnomaly = (System.Math.PI + -orbit.MaxTrueAnomaly.Value) / 2.0;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        #endregion
        
        #region TrueAnomaly2Time() tests

        [Test]
        public void TrueAnomaly2Time_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.TPP = 1000.0;
            orbit.ECC = 0.2;

            Angled trueAnomaly = 3.0 * System.Math.PI / 4.0;
            double result = 143887.2589;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result).Within(0.01).Percent);
        }

        [Test]
        public void TrueAnomaly2Time_ParabolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.TPP = 1000.0;
            orbit.ECC = 1.0;

            Angled trueAnomaly = 3.0 * System.Math.PI / 4.0;
            double result = 504250.3872;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result).Within(0.01).Percent);
        }

        [Test]
        public void TrueAnomaly2Time_ParabolicOrbitTrueAnomalyEqualsPI_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0;

            Angled trueAnomaly = System.Math.PI;
            double result = double.PositiveInfinity;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.TPP = 1000.0;
            orbit.ECC = 1.1;

            Angled trueAnomaly = 3.0 * System.Math.PI / 4.0;
            double result = 686502.5948;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result).Within(0.01).Percent);
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyEqualsMaxTrueAnomaly_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            Angled trueAnomaly = orbit.MaxTrueAnomaly.Value;

            double result = double.PositiveInfinity;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyEqualsMinTrueAnomaly_ReturnsNegativeInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            Angled trueAnomaly = -orbit.MaxTrueAnomaly.Value;

            double result = double.NegativeInfinity;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyBeyondMaxTrueAnomaly_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5;

            Angled trueAnomaly = (orbit.MaxTrueAnomaly.Value + System.Math.PI) / 2.0;

            double result = double.PositiveInfinity;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyBeyondMinTrueAnomaly_ReturnsNegativeInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            Angled trueAnomaly = (System.Math.PI + -orbit.MaxTrueAnomaly.Value) / 2.0;

            double result = double.NegativeInfinity;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        #endregion

        #region TrueAnomaly2Velocity() tests

        [Test]
        public void TrueAnomaly2Velocity_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 0.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            Angled trueAnomaly = 0.2309184684;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(2285.40472).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-789.0570548).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(250.5584854).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_ParabolicOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.0;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            Angled trueAnomaly = 0.2650866775;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(2610.632316).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-957.648237).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(316.5065052).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_HyperbolicOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;
            Angled trueAnomaly = 0.2946731859;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(2897.205314).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-1106.502648).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(375.0780042).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_ParabolicOrbitTrueAnomalyIsPI_ReturnsZeroVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.0;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;

            Angled trueAnomaly = System.Math.PI;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(Vector3d.zero.x).Within(1e-3));
            Assert.That(actual.y, Is.EqualTo(Vector3d.zero.y).Within(1e-3));
            Assert.That(actual.z, Is.EqualTo(Vector3d.zero.z).Within(1e-3));
        }

        [Test]
        public void TrueAnomaly2Velocity_HyperbolicOrbitTrueAnomalyIsMaxAnomaly_ReturnsEscapeVelocityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;

            Angled trueAnomaly = orbit.MaxTrueAnomaly.Value;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(1369.958045).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(244.3150799).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(-237.7500712).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_HyperbolicOrbitTrueAnomalyIsMinAnomaly_ReturnsEntryVelocityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;

            Angled trueAnomaly = -orbit.MaxTrueAnomaly.Value;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(477.4406487).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-1183.646565).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(603.3500122).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_HyperbolicOrbitTrueAnomalyIsBeyondMaxTrueAnomaly_ReturnsEscapeVelocityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;

            Angled trueAnomaly = orbit.MaxTrueAnomaly.Value + 0.1;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(1369.958045).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(244.3150799).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(-237.7500712).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_HyperbolicOrbitTrueAnomalyIsBeyondMinTrueAnomaly_ReturnsEntryVelocityVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.ECC = 1.5;
            orbit.INC = 0.5;
            orbit.APE = 1.2;
            orbit.LAN = 3.0;
            orbit.TPP = 500.0;

            Angled trueAnomaly = -(orbit.MaxTrueAnomaly.Value + 0.1);

            // Act
            Vector3d actual = orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(477.4406487).Within(0.1).Percent);
            Assert.That(actual.y, Is.EqualTo(-1183.646565).Within(0.1).Percent);
            Assert.That(actual.z, Is.EqualTo(603.3500122).Within(0.1).Percent);
        }

        [Test]
        public void TrueAnomaly2Velocity_NaNTrueAnomalyInput_ThrowsArgumentException()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            Angled trueAnomaly = double.NaN;

            // Act
            void TrueAnomaly2VelocityTestDelegate() => orbit.TrueAnomaly2Velocity(trueAnomaly);

            // Assert
            Assert.That(TrueAnomaly2VelocityTestDelegate, Throws.ArgumentException);
        }

        #endregion
    }
}