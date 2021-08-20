using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class OrbitTests
    {
        [Test]
        public void RPESetterInputValidation_NegativeInput_ClampsToZero()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            orbit.RPE = -1f;

            // Assert
            Assert.That(orbit.RPE, Is.EqualTo(0f));
        }
        
        [Test]
        public void ECCSetterInputValidation_NegativeInput_ClampsToZero()
        {
            // Arrange
            Orbit closedOrbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            closedOrbit.ECC = -1f;

            // Assert
            Assert.That(closedOrbit.ECC, Is.EqualTo(0f));
        }

        [Test]
        public void TrueAnomaly2Time_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8f;
            orbit.TPP = 1000f;
            orbit.ECC = 0.2f;
            
            Angle trueAnomaly = 3f * Mathf.PI / 4f;
            float result = 143887.2589f;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result).Within(0.01f).Percent);
        }

        [Test]
        public void TrueAnomaly2Time_ParabolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8f;
            orbit.TPP = 1000f;
            orbit.ECC = 1f;

            Angle trueAnomaly = 3f * Mathf.PI / 4f;
            float result = 504250.3872f;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result).Within(0.01f).Percent);
        }

        [Test]
        public void TrueAnomaly2Time_ParabolicOrbitTrueAnomalyEqualsPI_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1f;

            Angle trueAnomaly = Mathf.PI;
            float result = float.PositiveInfinity;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8f;
            orbit.TPP = 1000f;
            orbit.ECC = 1.1f;

            Angle trueAnomaly = 3f * Mathf.PI / 4f;
            float result = 686502.5948f;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result).Within(0.01f).Percent);
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyEqualsMaxTrueAnomaly_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            Angle trueAnomaly = orbit.MaxTrueAnomaly ?? Angle.Zero;

            float result = float.PositiveInfinity;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyEqualsMinTrueAnomaly_ReturnsNegativeInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            Angle trueAnomaly = Angle.Zero - (orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue;

            float result = float.NegativeInfinity;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyBeyondMaxTrueAnomaly_ReturnsPositiveInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            Angle trueAnomaly = ((orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue + Mathf.PI) / 2f;

            float result = float.PositiveInfinity;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TrueAnomaly2Time_HyperbolicOrbitTrueAnomalyBeyondMinTrueAnomaly_ReturnsNegativeInfinity()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            Angle trueAnomaly = (3f * Mathf.PI - (orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue) / 2f;

            float result = float.NegativeInfinity;

            // Act
            float actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitLowEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7f;
            orbit.ECC = 0.4f;
            orbit.TPP = 1500f;

            float time = -10000f;
            Angle result = 3.032453601f;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitHighEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7f;
            orbit.ECC = 0.95f;
            orbit.TPP = 1500f;

            float time = -10000f;
            Angle result = -2.284620281f;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitVeryHighEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7f;
            orbit.ECC = 0.9999f;
            orbit.TPP = 1500f;

            float time = -10000f;
            Angle result = -2.249168754f;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7f;
            orbit.ECC = 1.0f;
            orbit.TPP = 3_000f;

            float time = 5_000f;
            Angle result = 1.288831085f;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitPositiveInfinity_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0f;

            float time = float.PositiveInfinity;
            Angle result = Angle.HalfTurn;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitNegativeInfinity_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0f;

            float time = float.NegativeInfinity;
            Angle result = Angle.HalfTurn;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitVeryLargePositiveTime_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0f;

            float time = 1e+35f;
            Angle result = Angle.HalfTurn;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_ParabolicOrbitVeryLargeNegativeTime_ReturnsPI()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.0f;

            float time = -1e+35f;
            Angle result = Angle.HalfTurn;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitNormal_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7f;
            orbit.ECC = 1.5f;
            orbit.TPP = 2_000f;

            float time = 100f;
            Angle result = -1.269606064f;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitPositiveInfinity_ReturnsMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            float time = float.PositiveInfinity;
            Angle result = orbit.MaxTrueAnomaly ?? Angle.Zero;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitNegativeInfinity_Returns2PIMinusMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            float time = float.NegativeInfinity;
            Angle result = 2f * Mathf.PI - (orbit.MaxTrueAnomaly ?? Angle.Zero);

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitPositiveVeryLargeTime_ReturnsMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            float time = 1e+30f;
            Angle result = orbit.MaxTrueAnomaly ?? Angle.Zero;

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void Time2TrueAnomaly_HyperbolicOrbitNegativeVeryLargeTime_Returns2PIMinusMaxTrueAnomaly()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.ECC = 1.5f;

            float time = -1e+30f;
            Angle result = 2f * Mathf.PI - (orbit.MaxTrueAnomaly ?? Angle.Zero);

            // Act
            Angle actual = orbit.Time2TrueAnomaly(time);

            // Assert
            Assert.That(actual.RadValue, Is.EqualTo(result.RadValue));
        }

        [Test]
        public void TrueAnomaly2Point_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange 
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 1e+6f;
            orbit.ECC = 0.5f;
            orbit.INC = Mathf.PI / 4.0f;
            orbit.APE = Mathf.PI / 2.0f;
            orbit.LAN = Mathf.PI;

            Angle trueAnomaly = 0.5f;

            float xResult = 499821.1467f;
            float yResult = -646943.6416f;
            float zResult = 646943.6416f;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(xResult).Within(0.01f).Percent);
            Assert.That(actual.y, Is.EqualTo(yResult).Within(0.01f).Percent);
            Assert.That(actual.z, Is.EqualTo(zResult).Within(0.01f).Percent);
        }

        [Test]
        public void TrueAnomaly2Point_ParabolicOrbitNormalCase_MatchesHandwrittenWork()
        {
            // Arrange 
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 1e+6f;
            orbit.ECC = 1f;
            orbit.INC = Mathf.PI / 4.0f;
            orbit.APE = Mathf.PI / 2.0f;
            orbit.LAN = Mathf.PI;

            Angle trueAnomaly = 0.5f;

            float xResult = 510683.8424f;
            float yResult = -661003.7749f;
            float zResult = 661003.7749f;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(xResult).Within(0.01f).Percent);
            Assert.That(actual.y, Is.EqualTo(yResult).Within(0.01f).Percent);
            Assert.That(actual.z, Is.EqualTo(zResult).Within(0.01f).Percent);
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitNormalCase_MatchesHandwrittenWork()
        {
            // Arrange 
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 1e+6f;
            orbit.ECC = 3.0f;
            orbit.INC = Mathf.PI / 4.0f;
            orbit.APE = Mathf.PI / 2.0f;
            orbit.LAN = Mathf.PI;

            Angle trueAnomaly = 0.5f;

            float xResult = 527893.0221f;
            float yResult = -683278.4815f;
            float zResult = 683278.4815f;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.x, Is.EqualTo(xResult).Within(0.01f).Percent);
            Assert.That(actual.y, Is.EqualTo(yResult).Within(0.01f).Percent);
            Assert.That(actual.z, Is.EqualTo(zResult).Within(0.01f).Percent);
        }

        [Test]
        public void TrueAnomaly2Point_ParabolicOrbitTrueAnomalyIsPI_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 1.0f;

            Angle trueAnomaly = Mathf.PI;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0f;

            Angle trueAnomaly = orbit.MaxTrueAnomaly ?? Angle.Zero;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIs2PiMinusMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0f;

            Angle trueAnomaly = - (orbit.MaxTrueAnomaly ?? Angle.Zero);

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsBeyondMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0f;

            Angle trueAnomaly = (Mathf.PI + (orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue) / 2.0f;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsBeyond2PiMinusMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0f;

            Angle trueAnomaly = (Mathf.PI + - (orbit.MaxTrueAnomaly ?? Angle.Zero)) / 2.0f;

            // Act
            Vector3 actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsZeroAnglesElliptical_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000f;
            orbit.ECC = 0.75f;
            orbit.INC = 0.0f;
            orbit.APE = 0.0f;
            orbit.LAN = 0.0f;
            orbit.TPP = 6000f;

            float time = 7500f;
            Vector3 position = orbit.Time2Point(time);
            Vector3 velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }

        [Test]
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsNonZeroAnglesElliptical_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1_125_000f;
            orbit.ECC = 0.5f;
            orbit.INC = 6.0f;
            orbit.APE = 5.5f;
            orbit.LAN = 4.5f;
            orbit.TPP = 6000f;

            float time = 6200f;
            Vector3 position = orbit.Time2Point(time);
            Vector3 velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }

        [Test]
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsOnlyLANAngleElliptical_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000f;
            orbit.ECC = 0.75f;
            orbit.INC = 0.0f;
            orbit.APE = 0.0f;
            orbit.LAN = 2.5f;
            orbit.TPP = 6000f;

            float time = 7500f;
            Vector3 position = orbit.Time2Point(time);
            Vector3 velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }

        [Test]
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsParabolic_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000f;
            orbit.ECC = 1f;
            orbit.INC = 0.0f;
            orbit.APE = 0.0f;
            orbit.LAN = 0.0f;
            orbit.TPP = 6000f;

            float time = 6200f;
            Vector3 position = orbit.Time2Point(time);
            Vector3 velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }

        [Test]
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsHyperbolic_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000f;
            orbit.ECC = 1.4f;
            orbit.INC = 0.5f;
            orbit.APE = 3f;
            orbit.LAN = 5f;
            orbit.TPP = 2500f;

            float time = 7500f;
            Vector3 position = orbit.Time2Point(time);
            Vector3 velocity = orbit.Time2Velocity(time);

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Earth, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }

        [Test]
        public void StateVectors2Orbit_ParabolicHandwrittenInput_MatchesHandwrittenWork()
        {
            // Arrange

            // The inputs
            Vector3 position = new Vector3(1_000_000f, 100_000f, 10_000f);
            Vector3 velocity = new Vector3(1_500f, 2_000f, 881.929_010_7f);
            float time = 500f;

            // The correct final orbit
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.RPE = 591_617.833_5f;
            orbit.ECC = 1.0f;
            orbit.INC = 0.4394082206f;
            orbit.APE = 4.914028278f;
            orbit.LAN = 0.07849867002f;
            orbit.TPP = 147.0558678f;

            // Act
            Orbit actual = Orbit.StateVectors2Orbit(GravitationalBody.Kerbin, position, velocity, time);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }

        [Test]
        public void FindTransferOrbit_EllipticalOrbit_MatchesHandwrittenWorkAndReconstructsOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7f;
            orbit.ECC = 0.25f;
            orbit.INC = 0.4f;
            orbit.APE = 4.5f;
            orbit.LAN = 2.0f;
            orbit.TPP = 500f;

            float timeOne = 1_000f;
            float timeTwo = 3_236.312_736f;

            Vector3 positionOne = orbit.Time2Point(timeOne);
            Vector3 positionTwo = orbit.Time2Point(timeTwo);

            // Act
            Orbit actual = Orbit.FindTransferOrbit(GravitationalBody.Earth, positionOne, timeOne, positionTwo, timeTwo);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01f).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01f));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01f));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01f).Percent);
        }
    }
}