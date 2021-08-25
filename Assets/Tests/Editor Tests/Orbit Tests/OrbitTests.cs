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
            orbit.RPE = -1.0;

            // Assert
            Assert.That(orbit.RPE, Is.EqualTo(0.0));
        }
        
        [Test]
        public void ECCSetterInputValidation_NegativeInput_ClampsToZero()
        {
            // Arrange
            Orbit closedOrbit = GravitationalBody.Kerbin.DefaultOrbit;

            // Act
            closedOrbit.ECC = -1.0;

            // Assert
            Assert.That(closedOrbit.ECC, Is.EqualTo(0.0));
        }

        [Test]
        public void TrueAnomaly2Time_EllipticalOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+8;
            orbit.TPP = 1000.0;
            orbit.ECC = 0.2;
            
            Angle trueAnomaly = 3f * Mathf.PI / 4f;
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

            Angle trueAnomaly = 3f * Mathf.PI / 4f;
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

            Angle trueAnomaly = Mathf.PI;
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

            Angle trueAnomaly = 3f * Mathf.PI / 4f;
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

            Angle trueAnomaly = orbit.MaxTrueAnomaly ?? Angle.Zero;

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

            Angle trueAnomaly = Angle.Zero - (orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue;

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

            Angle trueAnomaly = ((orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue + Mathf.PI) / 2f;

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

            Angle trueAnomaly = (3f * Mathf.PI - (orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue) / 2f;

            double result = double.NegativeInfinity;

            // Act
            double actual = orbit.TrueAnomaly2Time(trueAnomaly);

            // Assert
            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void Time2TrueAnomaly_EllipticalOrbitLowEccentricity_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 0.4;
            orbit.TPP = 1500.0;

            double time = -10000.0;
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
            orbit.RPE = 1e+7;
            orbit.ECC = 0.95;
            orbit.TPP = 1500.0;

            double time = -10000.0;
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
            orbit.RPE = 1e+7;
            orbit.ECC = 0.9999;
            orbit.TPP = 1_500.0;

            double time = -10000.0;
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
            orbit.RPE = 1e+7;
            orbit.ECC = 1.0;
            orbit.TPP = 3_000.0;

            double time = 5_000.0;
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
            orbit.ECC = 1.0;

            double time = double.PositiveInfinity;
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
            orbit.ECC = 1.0;

            double time = double.NegativeInfinity;
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
            orbit.ECC = 1.0;

            double time = 1e+35;
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
            orbit.ECC = 1.0;

            double time = -1e+35;
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
            orbit.RPE = 1e+7;
            orbit.ECC = 1.5;
            orbit.TPP = 2_000.0;

            double time = 100.0;
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
            orbit.ECC = 1.5;

            double time = double.PositiveInfinity;
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
            orbit.ECC = 1.5;

            double time = double.NegativeInfinity;
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
            orbit.ECC = 1.5;

            double time = 1e+30;
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
            orbit.ECC = 1.5;

            double time = -1e+30;
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
            orbit.RPE = 1e+6;
            orbit.ECC = 0.5;
            orbit.INC = Mathf.PI / 4.0f;
            orbit.APE = Mathf.PI / 2.0f;
            orbit.LAN = Mathf.PI;

            Angle trueAnomaly = 0.5f;

            float xResult = 499821.1467f;
            float yResult = -646943.6416f;
            float zResult = 646943.6416f;

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
            orbit.INC = Mathf.PI / 4.0f;
            orbit.APE = Mathf.PI / 2.0f;
            orbit.LAN = Mathf.PI;

            Angle trueAnomaly = 0.5f;

            float xResult = 510683.8424f;
            float yResult = -661003.7749f;
            float zResult = 661003.7749f;

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
            orbit.INC = Mathf.PI / 4.0f;
            orbit.APE = Mathf.PI / 2.0f;
            orbit.LAN = Mathf.PI;

            Angle trueAnomaly = 0.5f;

            float xResult = 527893.0221f;
            float yResult = -683278.4815f;
            float zResult = 683278.4815f;

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

            Angle trueAnomaly = Mathf.PI;

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

            Angle trueAnomaly = orbit.MaxTrueAnomaly ?? Angle.Zero;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIs2PiMinusMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0;

            Angle trueAnomaly = - (orbit.MaxTrueAnomaly ?? Angle.Zero);

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

            Angle trueAnomaly = (Mathf.PI + (orbit.MaxTrueAnomaly ?? Angle.Zero).RadValue) / 2.0f;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void TrueAnomaly2Point_HyperbolicOrbitTrueAnomalyIsBeyond2PiMinusMaxTrueAnomaly_InfiniteMagnitudeOutputVector()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Kerbin.DefaultOrbit;
            orbit.ECC = 3.0;

            Angle trueAnomaly = (Mathf.PI + - (orbit.MaxTrueAnomaly ?? Angle.Zero)) / 2.0f;

            // Act
            Vector3d actual = orbit.TrueAnomaly2Point(trueAnomaly);

            // Assert
            Assert.That(actual.magnitude, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsZeroAnglesElliptical_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000.0;
            orbit.ECC = 0.75;
            orbit.INC = 0.0f;
            orbit.APE = 0.0f;
            orbit.LAN = 0.0f;
            orbit.TPP = 6000.0;

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
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsNonZeroAnglesElliptical_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1_125_000.0;
            orbit.ECC = 0.5;
            orbit.INC = 6.0f;
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
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsOnlyLANAngleElliptical_ReproducesOriginalOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 2_500_000.0;
            orbit.ECC = 0.75;
            orbit.INC = 0.0f;
            orbit.APE = 0.0f;
            orbit.LAN = 2.5f;
            orbit.TPP = 6000.0;

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
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsParabolic_ReproducesOriginalOrbit()
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
        public void StateVectors2Orbit_InputFromTime2PointAndTime2VelocityMethodsHyperbolic_ReproducesOriginalOrbit()
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

        [Test]
        public void FindTransferOrbit_EllipticalOrbit_MatchesHandwrittenWorkAndReconstructsOrbit()
        {
            // Arrange
            Orbit orbit = GravitationalBody.Earth.DefaultOrbit;
            orbit.RPE = 1e+7;
            orbit.ECC = 0.25;
            orbit.INC = 0.4f;
            orbit.APE = 4.5f;
            orbit.LAN = 2.0f;
            orbit.TPP = 500.0;

            double timeOne = 1_000.0;
            double timeTwo = 3_236.312_736;

            Vector3d positionOne = orbit.Time2Point(timeOne);
            Vector3d positionTwo = orbit.Time2Point(timeTwo);

            // Act
            Orbit actual = Orbit.FindTransferOrbit(GravitationalBody.Earth, positionOne, timeOne, positionTwo, timeTwo);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(orbit.RPE).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(orbit.ECC).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(orbit.INC.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(orbit.APE.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(orbit.LAN.RadValueMinusPiToPiRange).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(orbit.TPP).Within(0.01).Percent);
        }

        [Test]
        public void FindTransferOrbit_HyperbolicOrbit_MatchesHandwrittenWork()
        {
            // Arrange
            Orbit initialOrbit = GravitationalBody.Kerbin.DefaultOrbit;
            Orbit finalOrbit = GravitationalBody.Kerbin.DefaultOrbit;
            finalOrbit.RPE = 1_250_000.0;
            finalOrbit.ECC = 1.5;
            finalOrbit.TPP = 7_200.0;

            double departureTime = 4_000.0;
            double arrivalTime = 5_330.0;

            Vector3d positionOne = initialOrbit.Time2Point(departureTime);
            Vector3d positionTwo = finalOrbit.Time2Point(arrivalTime);

            // Act
            Orbit actual = Orbit.FindTransferOrbit(GravitationalBody.Kerbin, positionOne, departureTime, positionTwo, arrivalTime);

            // Assert
            Assert.That(actual.RPE, Is.EqualTo(997_251.612).Within(0.01).Percent);
            Assert.That(actual.ECC, Is.EqualTo(2.75089362).Within(0.01));
            Assert.That(actual.INC.RadValueMinusPiToPiRange, Is.EqualTo(0.0).Within(0.01));
            Assert.That(actual.APE.RadValueMinusPiToPiRange, Is.EqualTo(0.0).Within(0.01));
            Assert.That(actual.LAN.RadValueMinusPiToPiRange, Is.EqualTo(3.039575).Within(0.01));
            Assert.That(actual.TPP, Is.EqualTo(4_111.428195).Within(0.01).Percent);
        }
    }
}