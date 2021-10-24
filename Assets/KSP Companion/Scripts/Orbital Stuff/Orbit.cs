using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Keplerian orbit of a satellite.
/// </summary>
/// <remarks>
/// All types of Keplerian orbit (circular, elliptical, parabolic, and hyperbolic) may be represented by an 
/// instance of this class, with the exception of rectilinear orbits which are currently unsupported. 
/// Each instance stores data that specifies the size, shape, and orientation of the orbit; as well as data that 
/// specifies the position of the satellite along the orbit at a given time. This allows, for example, the 
/// calculation of the position or velocity of the satellite at any other time. There are also methods for 
/// converting measurements of a satellite's trajectory into the corresponding orbit and finding transfer 
/// trajectories between specified orbits.
/// <para>
/// A good understanding of the 'orbital elements' is helpful when using this class, in particular the meaning of the 
/// periapsis radius, eccentricity, inclination, argument of periapsis, longitude of the ascending node, 
/// time of periapsis passage, and the meaning of the true anomaly. This class does not use the traditional semi-major axis
/// as an orbital element, as the semi-major axis is degenerate for parabolic orbits. The semi-major axis is still accessible
/// through a read-only property.
/// </para>
/// </remarks>
[System.Serializable]
public partial class Orbit : IEquatable<Orbit>
{
    #region Fields

    // This orbital element specifies the angle in radians between the ascending node vector and the point of
    // periapsis of the orbit.
    protected Angled argumentOfPeriapsis;

    // This orbital element defines the shape of the orbit. This value is strictly positive and dimensionless.
    // Circular orbit:      Eccentricity = 0
    // Elliptical orbit:    0 < Eccentricity < 1
    // Parabolic orbit:     Eccentricity = 1
    // Hyperbolic orbit:    Eccentricity > 1
    protected double eccentricity;

    // This orbital element specifies the angle in radians between the orbital plane and the reference plane, which
    // this class takes to be the XY plane.
    protected Angled inclination;
    
    // The gravitational body that the satellite is orbiting.
    protected GravitationalBody gravitationalBody;

    // This orbital element specifies the angle in radians between the reference direction (X axis) and the ascending
    // node of the orbit.
    protected Angled longitudeOfAscendingNode;

    // This orbital element defines the size of the orbit, and has a value equal to the distance between the satellite and
    // the gravitational body when they are at their closest. The point at which this happens is known as the periapsis.
    // This value is strictly positive, and is measured in metres.
    protected double radiusOfPeriapsis;

    // This orbital element specifies the satellite's position in its orbit by specifying the time at which the satellite will
    // be at the periapsis point. For closed orbits (eccentricity < 1) the periodicity of the orbit means different choices for
    // this value are equivalent if the difference between the choices is equal to the period of the orbit. Measured in seconds.
    protected double timeOfPeriapsisPassage;

    #endregion

    #region Constructors

    /// <summary>
    /// Initialises a new instace of the Orbit class, which represents a Keplerian orbit.
    /// </summary>
    /// <param name="radiusOfPeriapsis">The radius of periapsis of the orbit.</param>
    /// <param name="eccentricity">The eccentricity of the orbit.</param>
    /// <param name="inclination">The inclincation of the orbit.</param>
    /// <param name="argumentOfPeriapsis">The argument of periapsis of the orbit.</param>
    /// <param name="longitudeOfAscendingNode">The longitude of the ascending node of the orbit.</param>
    /// <param name="timeOfPeriapsisPassage">The time of periapsis passage for the satellite on the orbit.</param>
    /// <param name="gravitationalBody">The gravitational body that the satellite is orbiting.</param>
    /// <remarks>
    /// All of the passed argument values will be validated by the corresponding property setters to prevent the creation of 
    /// an invalid orbit. For example, passing in <c><paramref name="eccentricity"/> = -1.0</c> will create an orbit with an
    /// eccentricity of 0.
    /// </remarks>
    public Orbit(double radiusOfPeriapsis, double eccentricity, Angled inclination, Angled argumentOfPeriapsis, Angled longitudeOfAscendingNode, double timeOfPeriapsisPassage, GravitationalBody gravitationalBody)
    {
        RPE = radiusOfPeriapsis;
        ECC = eccentricity;
        INC = inclination;
        APE = argumentOfPeriapsis;
        LAN = longitudeOfAscendingNode;
        TPP = timeOfPeriapsisPassage;

        GravitationalBody = gravitationalBody;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the argument of periapsis orbital element.
    /// </summary>
    /// <remarks>
    /// The value of this orbital element is the angle in radians between the ascending node and the point of periapsis of 
    /// the orbit. See <see cref="AscendingNode"/> and <see cref="PeriapsisPoint"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to set the argument of periapsis to NaN.</exception>
    public Angled APE
    {
        get
        {
            return argumentOfPeriapsis;
        }
        set
        {
            if (double.IsNaN(value))
                throw new ArgumentException("APE should never be set to NaN", "value");

            argumentOfPeriapsis = value;
        }
    }

    /// <summary>
    /// Gets the apoapsis point of the orbit.
    /// </summary>
    /// <remarks>
    /// The apoapsis point is the point at which the satellite and the attracting body have their maximum separation. For open
    /// orbits this point does not exist and this property returns null.
    /// </remarks>
    public Vector3d? ApoapsisPoint
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
                return null;
            else
                return TrueAnomaly2Point(Angled.HalfTurn);
        }
    }

    /// <summary>
    /// Gets the maximum separation between the satellite and the attracting body in metres.
    /// </summary>
    public double ApoapsisRadius
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
                return double.PositiveInfinity;
            else
                return (1.0 + eccentricity) / (1.0 - eccentricity) * radiusOfPeriapsis;
        }
    }

    /// <summary>
    /// Gets the position vector of the ascending node of the orbit.
    /// </summary>
    /// <remarks>
    /// The ascending node is the point at which the orbital trajectory crosses the reference XY plane from below. For open orbits 
    /// it is possible that this point does not exist, and in those cases null is returned.
    /// </remarks>
    public Vector3d? AscendingNode
    {
        get
        {
            Vector3d node = TrueAnomaly2Point(-argumentOfPeriapsis);
            if (node.magnitude == double.PositiveInfinity)
                return null;
            else
                return node;
        }
    }

    /// <summary>
    /// Gets the position vector of the descending node of the orbit.
    /// </summary>
    /// <remarks>
    /// The descending node is the point at which the orbital trajectory crosses the reference XY plane from above. For open orbits 
    /// it is possible that this point does not exist, and in those cases null is returned.
    /// </remarks>
    public Vector3d? DescendingNode
    {
        get
        {
            Vector3d node = TrueAnomaly2Point(-argumentOfPeriapsis + Angled.HalfTurn);
            if (node.magnitude == double.PositiveInfinity)
                return null;
            else
                return node;
        }
    }

    /// <summary>
    /// Gets or sets the eccentricity orbital element.
    /// </summary>
    /// <remarks>
    /// This value is strictly positive and dimensionless, and specifies what type of conic section the orbital trajectory is.
    /// Circular orbit: Eccentricity = 0.
    /// Elliptical orbit: 0 < Eccentricity < 1.
    /// Parabolic orbit: Eccentricity = 1.
    /// Hyperbolic orbit: Eccentricity > 1.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to set the eccentricity to NaN.</exception>
    public double ECC
    {
        get
        {
            return eccentricity;
        }
        set
        {
            if (double.IsNaN(value))
                throw new ArgumentException("ECC should never be set to NaN", "value");

            eccentricity = Mathd.Clamp(value, 0.0, double.PositiveInfinity);
        }
    }

    /// <summary>
    /// Gets the eccecentricity vector for this orbit.
    /// </summary>
    /// <remarks>
    /// The eccentricity vector points from the attracting body to the point of periapsis with a magnitude equal to the 
    /// eccentricity.
    /// </remarks>
    public Vector3d EccentricityVector
    {
        get
        {
            double magnitude = eccentricity;
            Quaterniond rotation = Quaterniond.AngleAxis(longitudeOfAscendingNode.DegValue, Vector3d.forward) * Quaterniond.AngleAxis(argumentOfPeriapsis.DegValue, SpecificAngularMomentumVector);
            return magnitude * (rotation * Vector3d.right);
        }
    }

    /// <summary>
    /// Gets the excess velocity for the orbit in metres/second.
    /// </summary>
    /// <remarks>
    /// The excess velocity is the speed of the satellite that is left after escaping to inifinity. For closed orbits it is 
    /// undefined and null is returned, for parabolic orbits it is equal to zero, and for hyperbolic orbits it is strictly positive. 
    /// </remarks>
    public double? ExcessVelocity
    {
        get
        {
            if (OrbitType == ConicSection.Elliptical)
                return null;
            else
                return Math.Sqrt(Mu / (-SemiMajorAxis));
        }
    }

    /// <summary>
    /// Gets or sets the gravitational body that the satellite is orbiting.
    /// </summary>
    public GravitationalBody GravitationalBody
    { 
        get
        { 
            return gravitationalBody; 
        } 
        set 
        { 
            gravitationalBody = value; 
        } 
    }

    /// <summary>
    /// Gets or sets the inclination orbital element.
    /// </summary>
    /// <remarks>
    /// The value of this orbital element is the angle in radians between the orbital plane and the reference XY plane.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to set the inclination to NaN.</exception>
    public Angled INC
    {
        get
        {
            return inclination;
        }
        set
        {
            if (double.IsNaN(value))
                throw new ArgumentException("INC should never be set to NaN", "value");

            inclination = Angled.Expel(value, Angled.QuarterTurn, Angled.ThreeQuartersTurn);
        }
    }

    /// <summary>
    /// Gets or sets the longitude of the ascending node orbital element. 
    /// </summary>
    /// <remarks>
    /// The value of this orbital element is the angle in radians between the reference direction (X axis) and the ascending node 
    /// of the orbit.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to set the longitude of the ascending node to NaN.</exception>
    /// <see cref="AscendingNode"/>
    public Angled LAN
    {
        get
        {
            return longitudeOfAscendingNode;
        }
        set
        {
            if (double.IsNaN(value))
                throw new ArgumentException("LAN should never be set to NaN", "value");

            longitudeOfAscendingNode = value;
        }
    }

    /// <summary>
    /// Gets the maximum possible value for the true anomaly of the satellite, measured in radians.
    /// </summary>
    /// <remarks>
    /// For open orbits not all values for the true anomaly are possible, as the satellite will have escaped to infinity. 
    /// For closed orbits all values are possible and this property returns null.
    /// </remarks>
    public Angled? MaxTrueAnomaly
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
            {
                return Math.Acos(-1.0 / eccentricity);
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the value of the mean motion for the orbit. Measured in radians/second.
    /// </summary>
    /// <remarks>
    /// The mean motion is the angular speed of a body in a circular orbit with the same angular momentum as the actual orbit
    /// of the satellite. This value is useful when calculating the position of the satellite at a given time.
    /// </remarks>
    public double MeanMotion
    {
        get
        {
            if (OrbitType == ConicSection.Parabolic)
            {
                // Parabolic case
                return Math.Sqrt(Mu);
            }

            return Math.Sqrt(Mu / (Math.Abs(SemiMajorAxis) * Math.Abs(SemiMajorAxis) * Math.Abs(SemiMajorAxis)));
        }
    }

    /// <summary>
    /// Gets the value of the gravitational parameter for the gravitational body that the satellite is orbiting. This is often
    /// represented in writing by the Greek letter mu.
    /// </summary>
    public double Mu
    { 
        get 
        { 
            return gravitationalBody.GravParameter; 
        } 
    }

    /// <summary>
    /// Gets the nodal vector for the orbit.
    /// </summary>
    /// <remarks>
    /// The nodal vector points to the ascending node, where the orbiting body crosses the reference XY plane from below.
    /// The magnitude of the vector is Sin(inclination).
    /// </remarks>
    public Vector3d NodalVector
    {
        get
        {
            double magnitude = Math.Sin(inclination);
            Quaterniond rotation = Quaterniond.AngleAxis(longitudeOfAscendingNode.DegValue, Vector3d.forward);
            return magnitude * (rotation * Vector3d.right);
        }
    }

    /// <summary>
    /// Gets the position vector for the periapsis point for the orbit.
    /// </summary>
    /// <remarks>
    /// The periapsis point is the point along the orbital trajectory at which the separation between the satellite and the 
    /// attracting body is at its minimum. 
    /// </remarks>
    public Vector3d PeriapsisPoint
    {
        get
        {
            return TrueAnomaly2Point(Angled.Zero);
        }
    }

    /// <summary>
    /// Gets the time in seconds for the satellite to complete one revolution of the orbit. For open orbits this returns 
    /// positive infinity.
    /// </summary>
    public double Period
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
                return double.PositiveInfinity;
            else
                return 2.0 * Math.PI * Math.Sqrt(SemiMajorAxis * SemiMajorAxis * SemiMajorAxis / Mu);
        }
    }

    /// <summary>
    /// Gets the type of conic section corresponding to the orbital trajectory of this orbit. Controlled by the eccentricity.
    /// </summary>
    /// <see cref="eccentricity"/>
    public ConicSection OrbitType
    {
        get
        {
            if (eccentricity < 1.0)
                return ConicSection.Elliptical;
            if (eccentricity > 1.0)
                return ConicSection.Hyperbolic;
            else
                return ConicSection.Parabolic;
        }
    }

    /// <summary>
    /// Gets or sets the radius of periapsis orbital element.
    /// </summary>
    /// <remarks>
    /// The value of this orbital element is equal to the distance between the satellite and the attracting body when they are at 
    /// their closest. This value is strictly positive and is measured in metres. Trying to set this to be less than zero will result 
    /// in clamping up to zero.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to set the radius of periapsis to NaN.</exception>
    public double RPE
    {
        get
        {
            return radiusOfPeriapsis;
        }
        set
        {
            if (double.IsNaN(value))
                throw new ArgumentException("RPE should never be set to NaN", "value");

            radiusOfPeriapsis = Mathd.Clamp(value, 0.0, double.PositiveInfinity);
        }
    }

    /// <summary>
    /// Gets the size of the semi-latus rectum for the orbit measured in metres.
    /// </summary>
    /// <remarks>
    /// The semi-latus rectum is the distance between the satellite and the attracting body when the satellite has a true 
    /// anomaly of PI/2. This value is often helpful for calculating other properties of the orbit.
    /// </remarks>
    public double SemiLatusRectum
    {
        get
        {
            return radiusOfPeriapsis * (1.0 + eccentricity);
        }
    }

    /// <summary>
    /// Gets the semi-major axis for the orbit in metres.
    /// </summary>
    /// <remarks>
    /// The semi-major axis is a measure of the size of the orbit. For closed orbits it is the average distance between the
    /// satellite and the attracting body over one revolution. For parabolic orbits the semi-major axis is infinite, and for 
    /// hyperbolic orbits the semi-major axis is negative.
    /// </remarks>
    public double SemiMajorAxis
    {
        get
        {
            if (OrbitType == ConicSection.Parabolic)
                return double.PositiveInfinity;
            else
                return radiusOfPeriapsis / (1.0 - eccentricity);
        }
    }

    /// <summary>
    /// Gets the specific angular momentum vector for the orbit.
    /// </summary>
    /// <remarks>
    /// The specific angular momentum vector is normal to the orbital plane, with a magnitude equal to the specific angular
    /// momentum of the satellite, measured in metres^2/second. "Specific" here means that it is the angular momentum per unit mass,
    /// not the actual total angular momentum.
    /// </remarks>
    public Vector3d SpecificAngularMomentumVector
    {
        get
        {
            double magnitude = Math.Sqrt(Mu * radiusOfPeriapsis * (1.0 + eccentricity));
            Quaterniond rotation = Quaterniond.AngleAxis(inclination.DegValue, Vector3d.right) * Quaterniond.AngleAxis(longitudeOfAscendingNode.DegValue, Vector3d.forward);
            return magnitude * (rotation * Vector3d.forward);
        }
    }

    /// <summary>
    /// Gets the specific energy of the orbit measured in Joules/kg.
    /// </summary>
    /// <remarks>
    /// The specific energy is the energy per unit mass of satellites travelling on the orbit. The total energy of the satellite
    /// can thus be found by multiplying by the mass of the satellite.
    /// </remarks>
    public double SpecificEnergy
    {
        get
        {
            return (1.0 - 2.0 / (1.0 + eccentricity)) * Mu * (1.0 + eccentricity) * (1.0 + eccentricity) / (2.0 * SemiLatusRectum);
        }
    }

    /// <summary>
    /// Gets or sets the time of periapsis passage orbital element.
    /// </summary>
    /// <remarks>
    /// The value of this orbital element is the time in seconds at which the satellite will be at the periapsis point. This 
    /// orbital element thus specifies the positional information of the satellite, while the other orbital elements specify the 
    /// orbit's geometry in space. For closed orbits (eccentricity < 1) the periodicity of the orbit means different choices for
    /// this value are equivalent if the difference between the values is equal to the period of the orbit. See <see cref="PeriapsisPoint"/>
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to the time of periapsis passage to NaN.</exception>
    public double TPP
    {
        get
        {
            return timeOfPeriapsisPassage;
        }
        set
        {
            if (double.IsNaN(value))
                throw new ArgumentException("TPP should never be set to NaN", "value");

            timeOfPeriapsisPassage = value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Indicates whether two orbits are approximately equal.
    /// </summary>
    /// <param name="orbitOne">The first Orbit to be compared.</param>
    /// <param name="orbitTwo">The second Orbit to be compared.</param>
    /// <returns>True if the Orbits are approximately equal, or if both Orbits are null. False if the Orbits are
    /// noticeably different, or if one of the Orbits is null.</returns>
    public static bool Approximately(Orbit orbitOne, Orbit orbitTwo)
    {
        if (orbitOne is null && orbitTwo is null)
            return true;

        // The previous conditional makes this OR a XOR.
        if (orbitOne is null || orbitTwo is null)
            return false;

        if (orbitOne.gravitationalBody != orbitTwo.gravitationalBody)
            return false;

        bool areTimesOfPeriapsisApproximatelyEqual = Angled.Approximately(orbitOne.Time2TrueAnomaly(orbitOne.timeOfPeriapsisPassage),
            orbitTwo.Time2TrueAnomaly(orbitTwo.timeOfPeriapsisPassage));

        return Mathd.Approximately(orbitOne.radiusOfPeriapsis, orbitTwo.radiusOfPeriapsis)
            && Mathd.Approximately(orbitOne.eccentricity, orbitTwo.eccentricity)
            && Angled.Approximately(orbitOne.inclination, orbitTwo.inclination)
            && Angled.Approximately(orbitOne.argumentOfPeriapsis, orbitTwo.argumentOfPeriapsis)
            && Angled.Approximately(orbitOne.longitudeOfAscendingNode, orbitTwo.longitudeOfAscendingNode)
            && areTimesOfPeriapsisApproximatelyEqual;
    }

    /// <summary>
    /// Calculates an orbit that transfers between two orbits at a given departure time and arrival time.
    /// </summary>
    /// <param name="initialOrbit">The orbit that the satellite starts on.</param>
    /// <param name="departureTime">The time at which the satellite maneuvers off the initial orbit.</param>
    /// <param name="targetOrbit">The orbit that the satellite will arrive at.</param>
    /// <param name="arrivalTime">The time at which the satellite arrives at the target orbit.</param>
    /// <returns>
    /// An orbit that transfers the satellite from the initial orbit to the target orbit if leaving the initial
    /// orbit at the departure time and arriving at the target orbit at the arrival time.
    /// <para>
    /// For almost all cases there is a unique choice for the transfer orbit. The exceptions to this occur when the angle between 
    /// the departure and arrival points is 0 or 2PI. In these cases, the transfer orbit's orientation in space is not 
    /// uniquely determined. The returned orbit is chosen to have the same orientation as the initial orbit. In the case that 
    /// no transfer orbit can be found, null is returned.
    /// </para>
    /// </returns>
    public static Orbit FindTransferOrbit(Orbit initialOrbit, double departureTime, Orbit targetOrbit, double arrivalTime)
    {
        // First check that the initial and final orbits are around the same body.
        if (initialOrbit.gravitationalBody != targetOrbit.gravitationalBody)
            return null;

        // Calculate the departure and arrival points in space
        Vector3d departurePoint = initialOrbit.Time2Point(departureTime);
        Vector3d arrivalPoint = targetOrbit.Time2Point(arrivalTime);

        if (departurePoint == arrivalPoint)
        {
            if (Mathd.Approximately(departureTime, arrivalTime))
            {
                return initialOrbit;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        Vector3d[] terminalVelocities = LambertsProblemHelper.Solver(initialOrbit.gravitationalBody, departurePoint, departureTime, arrivalPoint, arrivalTime);

        // Check if the solver was able to find valid terminal velocity vectors, and thus a transfer orbit may be found
        foreach (Vector3d velocityVector in terminalVelocities)
        {
            for (int i = 0; i < 3; i++)
            {
                if (double.IsNaN(velocityVector[i]))
                {
                    return null;
                }
            }
        }

        return StateVectors2Orbit(initialOrbit.gravitationalBody, departurePoint, terminalVelocities[0], departureTime);
    }

    /// <summary>
    /// Converts a measurement of the position and velocity of a satellite at a particular time into the orbit that the satellite
    /// must be on.
    /// </summary>
    /// <param name="body">The gravitational body that the satellite is orbiting around.</param>
    /// <param name="position">The position vector of the satellite.</param>
    /// <param name="velocity">The velocity vector of the satellite.</param>
    /// <param name="measurementTime">The time at which the position and velocity vectors were measured.</param>
    /// <returns>The orbit that the satellite must be travelling on.</returns>
    public static Orbit StateVectors2Orbit(GravitationalBody body, Vector3d position, Vector3d velocity, double measurementTime)
    {
        double periapsisRadius;
        double eccentricity;
        double inclination;
        double argumentOfPeriapsis;
        double longitudeOfAscendingNode;
        double timeOfPeriapsisPassage;
        double trueAnomaly;

        // Specific angular momentum vector.
        Vector3d hVector = Vector3d.Cross(position, velocity);
        // Eccentricity vector. Points at the periapsis point with a magnitude of the eccentricity.
        Vector3d eVector = 1.0 / body.GravParameter * Vector3d.Cross(velocity, hVector) - position.normalized;
        // Nodal vector. Points at the ascending node with a magnitude of Sin(inclination).
        Vector3d nVector = Vector3d.Cross(Vector3d.forward, hVector.normalized);

        trueAnomaly = Math.Acos(Vector3d.Dot(eVector, position) / (eVector.magnitude * position.magnitude));
        if (Vector3d.Dot(position, velocity) < 0)
            trueAnomaly = 2.0 * Math.PI - trueAnomaly;

        eccentricity = Mathd.Approximately(eVector.magnitude, 1.0) ? 1.0 : eVector.magnitude;

        double semiLatusRectum = position.magnitude * (1.0 + eccentricity * Math.Cos(trueAnomaly));
        periapsisRadius = semiLatusRectum / (1.0 + eccentricity);

        // If the orbit has an inclination of zero, then the nVector will equal the zero vector and the argumentOfPeriapsis and the
        // longitudeOfTheAscendingNode become undefined. Their sum remains well defined however. I have choosen that the argumentOfPeriapsis
        // will equal zero while the longitudeOfTheAscendingNode gets a value.
        if (nVector == Vector3d.zero)
        {
            inclination = 0.0;
            argumentOfPeriapsis = 0.0;
            longitudeOfAscendingNode = Vector3.SignedAngle(Vector3.right, (Vector3)eVector, Vector3.forward) * Mathd.Deg2Rad;
        }
        else
        {
            inclination = Math.Atan2(nVector.magnitude * hVector.magnitude, hVector.z);

            argumentOfPeriapsis = Math.Acos(Vector3d.Dot(nVector, eVector) / (nVector.magnitude * eVector.magnitude));
            if (eVector.z < 0.0)
                argumentOfPeriapsis = 2.0 * Math.PI - argumentOfPeriapsis;

            longitudeOfAscendingNode = Math.Acos(nVector.x / nVector.magnitude);
            if (nVector.y < 0.0)
                longitudeOfAscendingNode = 2.0 * Math.PI - longitudeOfAscendingNode;
        }

        // The code for calculating the time of periapsis passage is a bit awkward. 
        // By this point we know the size, shape, and orientation of the orbit, so we can make a new Orbit object that has
        // the correct geometry, but with the periapsis passage incorrectly set to 0.0s. The TrueAnomaly2Time() method, when
        // called on that object with the calculated value for the true anomaly, will then return how long it takes to travel
        // between the periapsis point and the position/velocity measurement point - for an orbit with this geometry.
        // This time can then be used to work out what the time of periapsis passage must've been for the actual measured satellite. 
        Orbit orbit = new Orbit(periapsisRadius, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, 0.0, body);
        double timeFromPeriapsisToTrueAnomaly = orbit.TrueAnomaly2Time(trueAnomaly);
        timeOfPeriapsisPassage = measurementTime - timeFromPeriapsisToTrueAnomaly;
        orbit.timeOfPeriapsisPassage = timeOfPeriapsisPassage;

        return orbit;
    }

    /// <summary>
    /// Produces a list of points along the orbital trajectory.
    /// </summary>
    /// <param name="startTrueAnomaly">
    /// The true anomaly of the first point in the list. If this value is null, or is equal to <paramref name="endTrueAnomaly"/>,
    /// then the entire orbit is output.
    /// </param>
    /// <param name="endTrueAnomaly">
    /// The true anomaly of the final point in the list. If this value is null, or is equal to <paramref name="startTrueAnomaly"/>,
    /// then the entire orbit is output.
    /// </param>
    /// <param name="maxTrueAnomalyStep">
    /// The maximum step in true anomaly between adjacent points in the list, in radians. The list requires an integer number of 
    /// points, so the actual spacing between adjacent points is adjusted be as close to the value of this parameter, but never
    /// larger than it.
    /// </param>
    /// <returns>
    /// A list of Vector3d points that sit on the orbital trajectory of the orbit, and a list of Angled objects whose values correspond to the true 
    /// anomaly values of the Vector3d points. If <paramref name="startTrueAnomaly"/> is null, then the first point in the list has a true 
    /// anomaly of 0.
    /// <para>
    /// When outputting a complete orbit, the final point in the list has approximately the same true anomaly as the first point in the list. 
    /// That is, the first and last points are the nearly the same. They are unlikely to be exactly the same, due to floating point rounding
    /// errors not allowing a perfect integer division of the range 2PI.
    /// </para>
    /// <para>
    /// When outputting a complete open orbit, if the true anomaly of a point is beyond the maximum/minimum true anomaly, then
    /// the Vector3.positiveInfinity point is output in the list. See <see cref="MaxTrueAnomaly"/>.
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentException">Step between output points cannot be NaN.</exception>
    public (List<Vector3d> points, List<Angled> trueAnomalies) OrbitalPoints(Angled? startTrueAnomaly, Angled? endTrueAnomaly, Angled maxTrueAnomalyStep)
    {
        if (Angled.IsNaN(maxTrueAnomalyStep))
            throw new ArgumentException("Step between points cannot be NaN.", "maxTrueAnomalyStep");

        // If the start and end points are the same, or if either of the points are null, output the entire orbit
        Angled angularRange;
        if (startTrueAnomaly == null || endTrueAnomaly == null ||
            Angled.IsNaN(startTrueAnomaly.Value) || Angled.IsNaN(endTrueAnomaly.Value) ||
            (startTrueAnomaly == endTrueAnomaly))
            angularRange = Angled.MaxAngle;
        else
            angularRange = endTrueAnomaly.Value.RadValue - startTrueAnomaly.Value.RadValue;

        int numberOfPoints = Mathd.CeilToInt(angularRange / maxTrueAnomalyStep.RadValue) + 1;

        // Adjust the angular spacing to ensure an integer number of points is output.
        Angled actualStep = angularRange / (numberOfPoints - 1);

        // Create the list to be output and fill it with points
        List<Vector3d> outputPoints = new List<Vector3d>(numberOfPoints);
        List<Angled> trueAnomalies = new List<Angled>(numberOfPoints);

        Angled angle;
        if (!startTrueAnomaly.HasValue || Angled.IsNaN(startTrueAnomaly.Value))
            angle = Angled.Zero;
        else
            angle = startTrueAnomaly.Value;
        Vector3d newPoint;
        for (int index = 0; index < outputPoints.Capacity; index++)
        {
            if (angle > MaxTrueAnomaly && angle < -MaxTrueAnomaly)
            {
                angle += actualStep;
                continue;
            }

            newPoint = TrueAnomaly2Point(angle);
            outputPoints.Add(newPoint);
            trueAnomalies.Add(angle);

            angle += actualStep;
        }

        return (outputPoints, trueAnomalies);
    }

    /// <summary>
    /// Calculates the position of the satellite at a given time.
    /// </summary>
    /// <param name="time">The time in seconds to calculate the satellite's position.</param>
    /// <returns>The position of the satellite at the given time.</returns>
    /// <exception cref="ArgumentException"><paramref name="time"/> was NaN, or the orbit is elliptical and <paramref name="time"/> is infinite.</exception>
    public Vector3d Time2Point(double time)
    {
        if (double.IsNaN(time))
            throw new ArgumentException("The input time can not be NaN.", "time");

        if (double.IsInfinity(time) && OrbitType == ConicSection.Elliptical)
            throw new ArgumentException("For elliptical orbits, the input time cannot be infinite.", "time");

        Angled trueAnomaly = Time2TrueAnomaly(time);
        return TrueAnomaly2Point(trueAnomaly);
    }

    /// <summary>
    /// Calculates the true anomaly of the satellite at a given time.
    /// </summary>
    /// <param name="time">The time at which the true anomaly is to be calculated. Measured in seconds.</param>
    /// <returns>
    /// The true anomaly of the satellite at the given time. Measured in radians.
    /// <para>
    /// For open orbits, passing in positive/negative infinity for <paramref name="time"/> will return either the 
    /// maximum or minimum true anomaly value. <see cref="MaxTrueAnomaly"/>
    /// </para>
    /// </returns>
    public Angled Time2TrueAnomaly(double time)
    {
        double meanAnomaly = Time2MeanAnomaly(time);
        Angled trueAnomaly;

        if (OrbitType == ConicSection.Elliptical)
        {
            // Elliptical orbit case
            double eccentricAnomaly = MeanAnomaly2EccentricAnomaly(meanAnomaly);
            trueAnomaly = EccentricAnomaly2TrueAnomaly(eccentricAnomaly);
            return trueAnomaly;
        }
        else if (OrbitType == ConicSection.Hyperbolic)
        {
            // Hyperbolic orbit case
            if (time == double.PositiveInfinity)
                return MaxTrueAnomaly.Value.RadValue;

            if (time == double.NegativeInfinity)
                return 2.0 * Math.PI - MaxTrueAnomaly.Value.RadValue;

            double hyperbolicAnomaly = MeanAnomaly2HyperbolicAnomaly(meanAnomaly);
            trueAnomaly = HyperbolicAnomaly2TrueAnomaly(hyperbolicAnomaly);
            return trueAnomaly;
        }

        // Parabolic orbit case
        if (time == double.PositiveInfinity || time == double.NegativeInfinity)
            return Angled.HalfTurn;

        double parabolicAnomaly = MeanAnomaly2ParabolicAnomaly(meanAnomaly);
        trueAnomaly = ParabolicAnomaly2TrueAnomaly(parabolicAnomaly);
        return trueAnomaly;
    }

    /// <summary>
    /// Calculates the velocity of the satellite at a given time.
    /// </summary>
    /// <param name="time">The time in seconds at which to calculate the velocity of the satellite.</param>
    /// <returns>The velocity of the satellite at the given time.</returns>
    /// <exception cref="ArgumentException"><paramref name="time"/> was NaN, or the orbit is elliptical and <paramref name="time"/> is infinite.</exception>
    public Vector3d Time2Velocity(double time)
    {
        if (double.IsNaN(time))
            throw new ArgumentException("The input time cannot be NaN", "time");

        if (double.IsInfinity(time) && OrbitType == ConicSection.Elliptical)
            throw new ArgumentException("For elliptical orbits, the input time cannot be infinite.", "time");

        Angled trueAnomaly = Time2TrueAnomaly(time);
        return TrueAnomaly2Velocity(trueAnomaly);
    }

    /// <summary>
    /// Returns a string containing the values of all the orbital elements of the orbit.
    /// </summary>
    /// <returns>A string containing the values of all the orbital elements of the orbit.</returns>
    public override string ToString()
    {
        return "RPE: " + radiusOfPeriapsis + "\nECC: " + eccentricity + "\nINCd: " + inclination.DegValue + "\nAPEd: " + argumentOfPeriapsis.DegValue + "\nLANd: " + longitudeOfAscendingNode.DegValue + "\nTPP: " + timeOfPeriapsisPassage + '\n' + base.ToString();
    }

    /// <summary>
    /// Calculates the position of the satellite for a given true anomaly.
    /// </summary>
    /// <param name="trueAnomaly">The true anomaly of the satellite, measured in radians.</param>
    /// <returns>
    /// The position of the satellite for the given true anomaly.
    /// <para>
    /// For open orbits, if <paramref name="trueAnomaly"/> is beyond the maximum/minimum true anomaly, then
    /// the positive infinity vector is returned. <see cref="MaxTrueAnomaly"/>
    /// </para>
    /// </returns>
    public Vector3d TrueAnomaly2Point(Angled trueAnomaly)
    {
        trueAnomaly = Angled.Expel(trueAnomaly, MaxTrueAnomaly ?? Angled.Zero, -(MaxTrueAnomaly ?? Angled.Zero));

        double radius;
        if (trueAnomaly >= MaxTrueAnomaly && trueAnomaly <= -MaxTrueAnomaly)
            return new Vector3d(Vector3.positiveInfinity);

        radius = radiusOfPeriapsis * (1.0 + eccentricity) / (1.0 + eccentricity * Math.Cos(trueAnomaly));

        Vector3d output;
        output.x = radius * (Math.Cos(longitudeOfAscendingNode) * Math.Cos(argumentOfPeriapsis + trueAnomaly) - Math.Sin(longitudeOfAscendingNode) * Math.Sin(argumentOfPeriapsis + trueAnomaly) * Math.Cos(inclination));
        output.y = radius * (Math.Sin(longitudeOfAscendingNode) * Math.Cos(argumentOfPeriapsis + trueAnomaly) + Math.Cos(longitudeOfAscendingNode) * Math.Sin(argumentOfPeriapsis + trueAnomaly) * Math.Cos(inclination));
        output.z = radius * Math.Sin(argumentOfPeriapsis + trueAnomaly) * Math.Sin(inclination);
        return output;
    }

    /// <summary>
    /// Calculates at what time the satellite will have a given true anomaly.
    /// </summary>
    /// <param name="trueAnomaly">The true anomaly of a point along the orbital trajectory. Measured in radians.</param>
    /// <returns>
    /// The time in seconds at which the satellite reaches the point with the given true anomaly.
    /// <para>
    /// Elliptical orbits: the returned time is always within one orbital period of the time of periapsis passage. 
    /// Parabolic orbits: if <paramref name="trueAnomaly"/> is PI, then positive infinity is returned. 
    /// Hyperbolic orbits: if <paramref name="trueAnomaly"/> is some unreachable value, then either positive or 
    /// negative infinity is returned depending on whether <paramref name="trueAnomaly"/> is closer to the 
    /// maximum true anomaly or the minimum true anomaly of the orbit. <see cref="MaxTrueAnomaly"/>
    /// </para>
    /// </returns>
    public double TrueAnomaly2Time(Angled trueAnomaly)
    {
        double meanAnomaly;
        double time;

        if (OrbitType == ConicSection.Elliptical)
        {
            // Elliptical orbit case
            double eccentricAnomaly = TrueAnomaly2EccentricAnomaly(trueAnomaly);
            meanAnomaly = EccentricAnomaly2MeanAnomaly(eccentricAnomaly);
            time = MeanAnomaly2Time(meanAnomaly);
            return time;
        }
        else if (OrbitType == ConicSection.Hyperbolic)
        {
            // Hyperbolic orbit case
            double hyperbolicAnomaly = TrueAnomaly2HyperbolicAnomaly(trueAnomaly);
            meanAnomaly = HyperbolicAnomaly2MeanAnomaly(hyperbolicAnomaly);
            time = MeanAnomaly2Time(meanAnomaly);
            return time;
        }

        // Parabolic orbit case
        double parabolicAnomaly = TrueAnomaly2ParabolicAnomaly(trueAnomaly);
        meanAnomaly = ParabolicAnomaly2MeanAnomaly(parabolicAnomaly);
        time = MeanAnomaly2Time(meanAnomaly);
        return time;
    }

    /// <summary>
    /// Calculates the velocity of the satellite at a given true anomaly.
    /// </summary>
    /// <param name="trueAnomaly">The true anomaly of a point along the orbital trajectory. Measured in radians.</param>
    /// <returns>The velocity of the satellite at the point with the given true anomaly.</returns>
    /// <exception cref="ArgumentException">The input true anomaly angle cannot be NaN.</exception>
    public Vector3d TrueAnomaly2Velocity(Angled trueAnomaly)
    {
        trueAnomaly = Angled.Expel(trueAnomaly, MaxTrueAnomaly ?? Angled.Zero, -(MaxTrueAnomaly ?? Angled.Zero));

        if (double.IsNaN(trueAnomaly))
            throw new ArgumentException("The true anomaly input angle cannot be NaN", "trueAnomaly");

        double angMomentum = SpecificAngularMomentumVector.magnitude;

        Vector3d output;
        output.x = -Mu / angMomentum * (Math.Cos(longitudeOfAscendingNode) * (Math.Sin(argumentOfPeriapsis + trueAnomaly) + eccentricity * Math.Sin(argumentOfPeriapsis)) + Math.Sin(longitudeOfAscendingNode) * (Math.Cos(argumentOfPeriapsis + trueAnomaly) + eccentricity * Math.Cos(argumentOfPeriapsis)) * Math.Cos(inclination));
        output.y = -Mu / angMomentum * (Math.Sin(longitudeOfAscendingNode) * (Math.Sin(argumentOfPeriapsis + trueAnomaly) + eccentricity * Math.Sin(argumentOfPeriapsis)) - Math.Cos(longitudeOfAscendingNode) * (Math.Cos(argumentOfPeriapsis + trueAnomaly) + eccentricity * Math.Cos(argumentOfPeriapsis)) * Math.Cos(inclination));
        output.z =  Mu / angMomentum * (Math.Cos(argumentOfPeriapsis + trueAnomaly) + eccentricity * Math.Cos(argumentOfPeriapsis)) * Math.Sin(inclination);
        return output;
    }

    #endregion

    #region Private Methods

    // Inverse hyperbolic tangent function.
    private static double Atanh(double x)
    {
        return 0.5 * Math.Log((1.0 + x) / (1.0 - x));
    }

    // Used by TrueAnomaly2Time() in the elliptical orbit case.
    private double EccentricAnomaly2MeanAnomaly(double eccentricAnomaly)
    {
        return eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly);
    }

    // Used by Time2TrueAnomaly() in the elliptical orbit case.
    private Angled EccentricAnomaly2TrueAnomaly(double eccentricAnomaly)
    {
        return 2.0 * Math.Atan2(Math.Sqrt(1.0 + eccentricity) * Math.Tan(eccentricAnomaly / 2.0), Math.Sqrt(1.0 - eccentricity));
    }

    // Used by TrueAnomaly2Time() in the hyperbolic orbit case.
    private double HyperbolicAnomaly2MeanAnomaly(double hyperbolicAnomaly)
    {
        if (hyperbolicAnomaly == double.PositiveInfinity)
            return double.PositiveInfinity;
        else if (hyperbolicAnomaly == double.NegativeInfinity)
            return double.NegativeInfinity;

        return eccentricity * Math.Sinh(hyperbolicAnomaly) - hyperbolicAnomaly;
    }

    // Used by Time2TrueAnomaly() in the hyperbolic orbit case.
    private Angled HyperbolicAnomaly2TrueAnomaly(double hyperbolicAnomaly)
    {
        return 2.0 * Math.Atan2(Math.Sqrt(eccentricity + 1.0) * Math.Tanh(hyperbolicAnomaly / 2.0), Math.Sqrt(eccentricity - 1.0));
    }

    // Used by Time2TrueAnomaly() in the elliptical orbit case.
    private double MeanAnomaly2EccentricAnomaly(double meanAnomaly)
    {
        // Kepler's equation relates the mean anomaly to the eccentric anomaly, but is transcendental in the unknown variable E
        // An initial guess is made for E (from Prussing & Conway), then Newton's method is used to find E.

        // Initial guess
        double u = meanAnomaly + eccentricity; // Used in the initial guess.
        double eccentricAnomaly = (meanAnomaly * (1.0 - Math.Sin(u)) + u * Math.Sin(meanAnomaly)) / (1.0 + Math.Sin(meanAnomaly) - Math.Sin(u));

        int numIterations = 0;
        double error = 1.0;
        while (error > Constants.KeplerEquationPrecision && numIterations < Constants.KeplerEquationMaxIterations)
        {
            // Fixed-point iteration method
            // eccentricAnomaly = meanAnomaly + eccentricity * Math.Sin(eccentricAnomaly);

            // Newton-Raphson method
            eccentricAnomaly = eccentricAnomaly - (eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) - meanAnomaly) / (1.0 - eccentricity * Math.Cos(eccentricAnomaly));

            error = Math.Abs(eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) - meanAnomaly);
            numIterations++;
        }

        return eccentricAnomaly;
    }

    // Used by Time2TrueAnomaly() in the hyperbolic orbit case.
    private double MeanAnomaly2HyperbolicAnomaly(double meanAnomaly)
    {
        // In the hyperbolic case we must solve the hyperbolic Kepler equation in the transcendental unknown hyperbolic anomaly H.
        // We start with an initial guess and use Newton's method.
        double hyperbolicAnomaly = meanAnomaly; // Initial guess

        int numIterations = 0;
        double error = 1.0;
        while (error > Constants.KeplerEquationPrecision && numIterations < Constants.KeplerEquationMaxIterations && !double.IsNaN(hyperbolicAnomaly))
        {
            // Newton's method
            hyperbolicAnomaly = hyperbolicAnomaly - (eccentricity * Math.Sinh(hyperbolicAnomaly) - hyperbolicAnomaly - meanAnomaly) / (eccentricity * Math.Cosh(hyperbolicAnomaly) - 1.0);

            error = Math.Abs(eccentricity * Math.Sinh(hyperbolicAnomaly) - hyperbolicAnomaly - meanAnomaly);
            numIterations++;
        }

        // If the input meanAnomaly is large enough, the Sinh and Cosh functions will output NaN. In this case the method should
        // return Infinity with the same sign as meanAnomaly.
        if (double.IsNaN(hyperbolicAnomaly))
            return (meanAnomaly > 0.0) ? double.PositiveInfinity : double.NegativeInfinity;

        return hyperbolicAnomaly;
    }

    // Used by Time2TrueAnomaly() in the parabolic orbit case.
    private double MeanAnomaly2ParabolicAnomaly(double meanAnomaly)
    {
        // For the parabolic case Barker's equation can give us the exact mean anomaly, unlike in the elliptical and hyperbolic case.
        // We have to find the one real root of a depressed cubic polynomial using Cardano's method.
        double A = Math.Sqrt(9.0 * meanAnomaly * meanAnomaly + 8.0 * radiusOfPeriapsis * radiusOfPeriapsis * radiusOfPeriapsis);
        double termOne = Math.Pow(3.0 * meanAnomaly + A, 1.0 / 3.0);
        double termTwo = (3.0 * meanAnomaly - A > 0) ? Math.Pow(3.0 * meanAnomaly - A, 1.0 / 3.0) : -Math.Pow(3.0 * -meanAnomaly + A, 1.0 / 3.0); // Ugly but works without needing to define a custom cube root function that accepts negative inputs with fractional powers.
        return termOne + termTwo;
    }

    // Used by TrueAnomaly2Time() for all types of orbit.
    private double MeanAnomaly2Time(double meanAnomaly)
    {
        return meanAnomaly / MeanMotion + timeOfPeriapsisPassage;
    }

    // Used by TrueAnomaly2Time() in the parabolic orbit case.
    private double ParabolicAnomaly2MeanAnomaly(double parabolicAnomaly)
    {
        if (double.IsInfinity(parabolicAnomaly))
            return parabolicAnomaly;
        else
            return radiusOfPeriapsis * parabolicAnomaly + parabolicAnomaly * parabolicAnomaly * parabolicAnomaly / 6.0;
    }

    // Used by Time2TrueAnomaly() in the parabolic orbit case.
    private Angled ParabolicAnomaly2TrueAnomaly(double parabolicAnomaly)
    {
        return 2.0f * Math.Atan(parabolicAnomaly / Math.Sqrt(2.0 * radiusOfPeriapsis));
    }

    // Used by Time2TrueAnomaly() for all types of orbit.
    private double Time2MeanAnomaly(double time)
    {
        return MeanMotion * (time - timeOfPeriapsisPassage);
    }

    // Used by TrueAnomaly2Time() in the elliptical orbit case.
    private double TrueAnomaly2EccentricAnomaly(Angled trueAnomaly)
    {
        return 2.0 * Math.Atan2(Math.Sqrt(1.0 - eccentricity) * Math.Tan(trueAnomaly / 2.0), Math.Sqrt(1.0 + eccentricity));
    }

    // Used by TrueAnomaly2Time() in the hyperbolic orbit case.
    private double TrueAnomaly2HyperbolicAnomaly(Angled trueAnomaly)
    {
        // If the input true anomaly is unreachable by this hyperbolic orbit, change the true anomaly to whichever of the
        // max/min true anomaly is closest.
        trueAnomaly = Angled.Expel(trueAnomaly, MaxTrueAnomaly ?? Angled.Zero, 2.0 * Math.PI - (MaxTrueAnomaly ?? Angled.Zero));

        if (trueAnomaly == MaxTrueAnomaly)
            return double.PositiveInfinity;

        if (trueAnomaly == 2.0 * Math.PI - MaxTrueAnomaly)
            return double.NegativeInfinity;

        return 2.0 * Atanh(Math.Sqrt((eccentricity - 1.0) / (eccentricity + 1.0)) * Math.Tan(trueAnomaly / 2.0));
    }

    // Used by TrueAnomaly2Time() in the parabolic orbit case.
    private double TrueAnomaly2ParabolicAnomaly(Angled trueAnomaly)
    {
        if (trueAnomaly.RadValue == Math.PI)
            return double.PositiveInfinity;
        return Math.Sqrt(2.0 * radiusOfPeriapsis) * Math.Tan(trueAnomaly / 2.0);
    }

    #endregion

    #region Enums

    /// <summary>
    /// Specifies what type of conic section matches the orbital trajectory curve for an orbit.
    /// </summary>
    public enum ConicSection
    {
        Elliptical,
        Parabolic,
        Hyperbolic
    }

    #endregion

    #region Classes

    // This static nested class provides a method for solving Lambert's problem.
    // The algorithm used to solve Lambert's problem comes from the paper "Revisiting Lambert's Problem" by 
    // Dario Izzo in 2014.
    protected static class LambertsProblemHelper
    {
        // This method solves Lambert's problem to produce the terminal velocity vectors of a satellite that has a position given 
        // by positionOne/Two at timeOne/Two. These may then be used, for example, to calculate the orbit of the satellite.
        public static Vector3d[] Solver(GravitationalBody body, Vector3d positionOne, double timeOne, Vector3d positionTwo, double timeTwo)
        {
            double chordLength = (positionTwo - positionOne).magnitude;
            double semiPerimeter = 0.5 * (positionOne.magnitude + positionTwo.magnitude + chordLength);
            double lambdaSquared = (semiPerimeter - chordLength) / semiPerimeter;
            double lambda;
            if (positionOne.x * positionTwo.y - positionOne.y * positionTwo.x < 0)
                lambda = -Math.Sqrt(lambdaSquared);
            else
                lambda = Math.Sqrt(lambdaSquared);

            double TStar = Math.Sqrt(2.0 * body.GravParameter / (semiPerimeter * semiPerimeter * semiPerimeter)) * (timeTwo - timeOne);

            double xFinal = FindXThatProducesTStar(lambda, TStar);
            double yFinal = X2Y(xFinal, lambda);

            // Compute the velocity vector at t = timeOne on the transfer orbit from (xFinal, yFinal)
            // First compute the necessary unit vectors
            Vector3d orbitalPlaneNormalVector = Vector3d.Cross(positionOne, positionTwo).normalized;
            Vector3d tangentialUnitVector = Vector3d.Cross(orbitalPlaneNormalVector, positionOne).normalized;
            if (lambda < 0.0)
                tangentialUnitVector = -tangentialUnitVector;

            double gamma = Math.Sqrt(body.GravParameter * semiPerimeter / 2.0);
            double rho = (positionOne.magnitude - positionTwo.magnitude) / chordLength;
            double sigma = Math.Sqrt(1.0 - rho * rho);

            double v1CcomponentOne = gamma / positionOne.magnitude * (lambda * yFinal - xFinal - rho * (lambda * yFinal + xFinal));
            double v1ComponentTwo = gamma * sigma / positionOne.magnitude * (yFinal + lambda * xFinal);

            double v2CcomponentOne = -gamma / positionTwo.magnitude * (lambda * yFinal - xFinal + rho * (lambda * yFinal + xFinal));
            double v2ComponentTwo = gamma * sigma / positionTwo.magnitude * (yFinal + lambda * xFinal);

            Vector3d velocityAtPositionOne = v1CcomponentOne * positionOne.normalized + v1ComponentTwo * tangentialUnitVector;
            Vector3d velocityAtPositionTwo = v2CcomponentOne * positionTwo.normalized + v2ComponentTwo * tangentialUnitVector;

            return new Vector3d[2] { velocityAtPositionOne, velocityAtPositionTwo };
        }

        private static double FindXThatProducesTStar(double lambda, double TStar)
        {
            // Calculate T for the minimum energy ellipse and the parabolic case
            double T0 = Math.Acos(lambda) + lambda * Math.Sqrt(1.0 - lambda * lambda);
            double T1 = 2.0 / 3.0 * (1.0 - lambda * lambda * lambda);

            // Make an initial guess for x based on TStar, T0, and T1.
            double x;
            if (TStar < T1)
            {
                x = 5.0 / 2.0 * (T1 * (T1 - TStar)) / (TStar * (1.0 - Math.Pow(lambda, 5.0))) + 1.0;
            }
            else if (Mathf.Approximately((float)TStar, (float)T1))
            {
                x = 1.0;
                return x;
            }
            else if (T1 < TStar && TStar < T0)
            {
                x = Math.Pow(T0 / TStar, Math.Log(T1 / T0, 2.0)) - 1.0;
            }
            else if (Mathf.Approximately((float)TStar, (float)T0))
            {
                x = 0.0;
                return x;
            }
            else
            {
                // TStar > T0
                x = Math.Pow(T0 / TStar, 2.0 / 3.0) - 1.0;
            }

            // Do 3rd order Householder iteration to find a value for x that solves T(x) - TStar = 0.
            double y;
            double T;       // The nondimensional time of flight T.
            double Td;      // First derivative.
            double Tdd;     // Second derivative.
            double Tddd;    // Third derivative.
            for (int i = 0; i < Constants.LambertSolverMaxIterations; i++)
            {
                y = X2Y(x, lambda);
                T = TOfX(x, y, lambda);
                Td = TFirstDerivative(x, y, lambda, T);
                Tdd = TSecondDerivative(x, y, lambda, T, Td);
                Tddd = TThirdDerivative(x, y, lambda, Td, Tdd);

                // Iterate the next guess for x
                x = x - (T - TStar) * (Td * Td - 0.5 * (T - TStar) * Tdd) / (Td * (Td * Td - (T - TStar) * Tdd) + 1.0 / 6.0 * (T - TStar) * (T - TStar) * Tddd);
            }

            // Debug.Log("Lambert Solver Householder iteration output: \nx = " + x + "\nT(x) = " + TOfX(x, X2Y(x, lambda), lambda) + "\nTStar = " + TStar);
            return x;
        }

        private static double X2Y(double x, double lambda)
        {
            return Math.Sqrt(1.0 - lambda * lambda * (1.0 - x * x));
        }

        private static double TOfX(double x, double y, double lambda, int numOfRevolutions = 0)
        {
            return 1.0 / (1.0 - x * x) * ((Psi(x, y, lambda) + numOfRevolutions * Math.PI) / Math.Sqrt(Math.Abs(1.0 - x * x)) - x + lambda * y);
        }

        private static double TFirstDerivative(double x, double y, double lambda, double T)
        {
            return 1.0 / (1.0 - x * x) * (3.0 * x * T - 2.0 + 2.0 * lambda * lambda * lambda * x / y);
        }

        private static double TSecondDerivative(double x, double y, double lambda, double T, double TfirstDeriv)
        {
            return 1.0 / (1.0 - x * x) * (3.0 * T + 5.0 * x * TfirstDeriv + 2.0 * (1.0 - lambda * lambda) * lambda * lambda * lambda / y / y / y);
        }

        private static double TThirdDerivative(double x, double y, double lambda, double TFirstDeriv, double TSecondDeriv)
        {
            return 1.0 / (1.0 - x * x) * (7.0 * x * TSecondDeriv + 8.0 * TFirstDeriv - 6.0 * (1.0 - lambda * lambda) * Math.Pow(lambda, 5.0) * x / Math.Pow(y, 5.0));
        }

        private static double Psi(double x, double y, double lambda)
        {
            if (x < 1.0)
            {
                return Math.Acos(x * y + lambda * (1.0 - x * x));
            }
            else
                return ACosh(x * y - lambda * (x * x - 1.0));
        }

        // Inverse hyperbolic cosine function
        private static double ACosh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x - 1.0));
        }
    }

    #endregion
}

// This partial section of the Orbit class implements the IEquatable<Orbit> interface.
public partial class Orbit : IEquatable<Orbit>
{
    /// <summary>
    /// Checks to see if two Orbit objects are equal.
    /// </summary>
    /// <param name="orbitOne">The first Orbit object.</param>
    /// <param name="orbitTwo">The second Orbit object.</param>
    /// <returns>True if the Orbit objects are the same. False if either Orbit is null, or the Orbits are not the same.</returns>
    public static bool operator ==(Orbit orbitOne, Orbit orbitTwo)
    {
        if (orbitOne is null && orbitTwo is null)
            return true;

        // This OR condition is unreachable if both conditions are true, so is an effective XOR.
        if (orbitOne is null || orbitTwo is null)
            return false;

        return orbitOne.Equals(orbitTwo);
    }

    /// <summary>
    /// Checks to see if two Orbit objects are not equal.
    /// </summary>
    /// <param name="orbitOne">The first Orbit object.</param>
    /// <param name="orbitTwo">The second Orbit object.</param>
    /// <returns>True if the Orbits are not the same, or if either Orbit is null. False if the Orbits are the same.</returns>
    public static bool operator !=(Orbit orbitOne, Orbit orbitTwo)
    {
        if (orbitOne is null && orbitTwo is null)
            return false;

        // This OR condition is unreachable if both conditions are true, so is an effective XOR.
        if (orbitOne is null || orbitTwo is null)
            return true;

        return !orbitOne.Equals(orbitTwo);
    }

    /// <summary>
    /// Checks to see if another object is equal to this Orbit.
    /// </summary>
    /// <param name="obj">The other object to checked for equality.</param>
    /// <returns>True if the other object is equal to this Orbit. False otherwise.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || !GetType().Equals(obj.GetType()))
            return false;

        return Equals((Orbit)obj);
    }

    /// <summary>
    /// Checks to see if another Orbit is equal to this Orbit.
    /// </summary>
    /// <param name="other">The other Orbit to be checked for equality.</param>
    /// <returns>True if the orbits are the same. False otherwise.</returns>
    public bool Equals(Orbit other)
    {
        if (other is null)
            return false;

        bool areTimesOfPeriapsisPassageEquivalent = Time2TrueAnomaly(timeOfPeriapsisPassage) == other.Time2TrueAnomaly(other.timeOfPeriapsisPassage);

        return (gravitationalBody == other.GravitationalBody) && (radiusOfPeriapsis == other.radiusOfPeriapsis) 
            && (eccentricity == other.eccentricity) && (inclination == other.inclination) && (argumentOfPeriapsis == other.APE)
            && (longitudeOfAscendingNode == other.LAN) && areTimesOfPeriapsisPassageEquivalent;
    }

    /// <summary>
    /// Serves as the hash function for this Orbit.
    /// </summary>
    /// <returns>A hash code for this Orbit.</returns>
    public override int GetHashCode()
    {
        return (gravitationalBody.GravParameter + gravitationalBody.Radius + radiusOfPeriapsis + eccentricity
            + inclination + argumentOfPeriapsis + longitudeOfAscendingNode + timeOfPeriapsisPassage).GetHashCode();
    }

}