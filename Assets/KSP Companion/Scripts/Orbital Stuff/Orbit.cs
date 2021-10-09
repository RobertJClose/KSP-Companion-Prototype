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
/// orbits between specified orbits.
/// <para>
/// A good understanding of the 'orbital elements' is helpful when using this class, in particular the meaning of the 
/// periapsis radius, eccentricity, inclination, argument of periapsis, longitude of the ascending node, 
/// time of periapsis passage, and the meaning of the true anomaly. 
/// </para>
/// </remarks>
[System.Serializable]
public class Orbit
{
    #region Fields

    // This orbital element specifies the angle in radians between the ascending node vector and the point of
    // periapsis of the orbit.
    protected Angle argumentOfPeriapsis;

    // This orbital element defines the shape of the orbit. This value is strictly positive and dimensionless.
    // Circular orbit:      Eccentricity = 0
    // Elliptical orbit:    0 < Eccentricity < 1
    // Parabolic orbit:     Eccentricity = 1
    // Hyperbolic orbit:    Eccentricity > 1
    protected double eccentricity;

    // This orbital element specifies the angle in radians between the orbital plane and the reference plane, which
    // this class takes to be the XY plane.
    protected Angle inclination;
    
    // The gravitational body that the satellite is orbiting.
    protected GravitationalBody gravitationalBody;

    // This orbital element specifies the angle in radians between the reference direction (X axis) and the ascending
    // node of the orbit.
    protected Angle longitudeOfAscendingNode;

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
    /// <see cref="RPE"/>
    /// <see cref="ECC"/>
    /// <see cref="INC"/>
    /// <see cref="APE"/>
    /// <see cref="LAN"/>
    /// <see cref="TPP"/>
    public Orbit(double radiusOfPeriapsis, double eccentricity, Angle inclination, Angle argumentOfPeriapsis, Angle longitudeOfAscendingNode, double timeOfPeriapsisPassage, GravitationalBody gravitationalBody)
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
    /// the orbit.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to set the argument of periapsis to NaN.</exception>
    /// <see cref="AscendingNode"/>
    /// <see cref="PeriapsisPoint"/>
    public Angle APE
    {
        get
        {
            return argumentOfPeriapsis;
        }
        set
        {
            if (float.IsNaN(value))
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
                return TrueAnomaly2Point(Angle.HalfTurn);
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
                return float.PositiveInfinity;
            else
                return (1f + eccentricity) / (1f - eccentricity) * radiusOfPeriapsis;
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
            if (node.magnitude == float.PositiveInfinity)
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
            Vector3d node = TrueAnomaly2Point(-argumentOfPeriapsis + Angle.HalfTurn);
            if (node.magnitude == float.PositiveInfinity)
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
    /// undefined, for parabolic orbits it is equal to zero, and for hyperbolic orbits it is strictly positive. 
    /// </remarks>
    public double ExcessVelocity
    {
        get
        {
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
    public Angle INC
    {
        get
        {
            return inclination;
        }
        set
        {
            if (float.IsNaN(value))
                throw new ArgumentException("INC should never be set to NaN", "value");

            inclination = value % Mathf.PI;
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
    public Angle LAN
    {
        get
        {
            return longitudeOfAscendingNode;
        }
        set
        {
            if (float.IsNaN(value))
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
    public Angle? MaxTrueAnomaly
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
            {
                return (float)Math.Acos(-1f / eccentricity);
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
                return Mathf.Sqrt(Mu);
            }

            return Math.Sqrt(Mu / (Math.Abs(SemiMajorAxis) * Math.Abs(SemiMajorAxis) * Math.Abs(SemiMajorAxis)));
        }
    }

    /// <summary>
    /// Gets the value of the gravitational parameter for the gravitational body that the satellite is orbiting. This is often
    /// represented in writing by the Greek letter mu.
    /// </summary>
    public float Mu
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
            double magnitude = Mathf.Sin(inclination);
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
            return TrueAnomaly2Point(Angle.Zero);
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
                return float.PositiveInfinity;
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
            if (eccentricity < 1.0f)
                return ConicSection.Elliptical;
            if (eccentricity > 1.0f)
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
            double magnitude = Math.Sqrt(SemiLatusRectum * Mu);
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
    /// this value are equivalent if the difference between the values is equal to the period of the orbit.
    /// </remarks>
    /// <exception cref="ArgumentException">An attempt was made to the time of periapsis passage to NaN.</exception>
    /// <see cref="PeriapsisPoint"/>
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
    /// there is no possible transfer orbit, null is returned.
    /// </para>
    /// </returns>
    public static Orbit FindTransferOrbit(Orbit initialOrbit, double departureTime, Orbit targetOrbit, double arrivalTime)
    {
        // First check that the initial and final orbits are around the same body. This should ALWAYS be the case, and if a
        // caller is failing to do this that's a developer error.
        //if (initialOrbit.attractingBody != targetOrbit.attractingBody)
        //    throw new ArgumentException("The initial and target orbits should be around the same gravitational body", "initialOrbit, targetOrbit");

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
        float inclination;
        float argumentOfPeriapsis;
        float longitudeOfAscendingNode;
        double timeOfPeriapsisPassage;
        float trueAnomaly;

        // Specific angular momentum vector.
        Vector3d hVector = Vector3d.Cross(position, velocity);
        // Eccentricity vector. Points at the periapsis point with a magnitude of the eccentricity.
        Vector3d eVector = 1.0 / body.GravParameter * Vector3d.Cross(velocity, hVector) - position.normalized;
        // Nodal vector. Points at the ascending node with a magnitude of Sin(inclination).
        Vector3d nVector = Vector3d.Cross(Vector3d.forward, hVector.normalized);

        trueAnomaly = (float)Math.Acos(Vector3d.Dot(eVector, position) / (eVector.magnitude * position.magnitude));
        if (Vector3d.Dot(position, velocity) < 0)
            trueAnomaly = 2.0f * Mathf.PI - trueAnomaly;

        eccentricity = eVector.magnitude;

        double semiLatusRectum = position.magnitude * (1.0 + eccentricity * Mathf.Cos(trueAnomaly));
        periapsisRadius = semiLatusRectum / (1.0 + eccentricity);

        // If the orbit has an inclination of zero, then the nVector will equal the zero vector and the argumentOfPeriapsis and the
        // longitudeOfTheAscendingNode become undefined. Their sum remains well defined however. I have choosen that the argumentOfPeriapsis
        // will equal zero while the longitudeOfTheAscendingNode gets a value.
        if (nVector == Vector3d.zero)
        {
            inclination = 0.0f;
            argumentOfPeriapsis = 0.0f;
            longitudeOfAscendingNode = Vector3.SignedAngle(Vector3.right, (Vector3)eVector, Vector3.forward) * Mathf.Deg2Rad;
        }
        else
        {
            inclination = (float)Math.Atan2(nVector.magnitude * hVector.magnitude, hVector.z);

            argumentOfPeriapsis = (float)Math.Acos(Vector3d.Dot(nVector, eVector) / (nVector.magnitude * eVector.magnitude));
            if (eVector.z < 0)
                argumentOfPeriapsis = 2.0f * Mathf.PI - argumentOfPeriapsis;

            longitudeOfAscendingNode = (float)Math.Acos(nVector.x / nVector.magnitude);
            if (nVector.y < 0)
                longitudeOfAscendingNode = 2.0f * Mathf.PI - longitudeOfAscendingNode;
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
        orbit.TPP = timeOfPeriapsisPassage;

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
    /// <param name="trueAnomalies">Out parameter containing the true anomaly values of the outputted points in the list.</param>
    /// <param name="maxTrueAnomalyStep">
    /// The maximum step in true anomaly between adjacent points in the list, in radians. The list requires an integer number of 
    /// points, so the actual spacing between adjacent points is adjusted be as close to the value of this parameter, but never
    /// larger than it.
    /// </param>
    /// <returns>
    /// A list of Vector3 points that sit on the orbital trajectory of the orbit. If <paramref name="startTrueAnomaly"/> is null,
    /// then the first point in the list has a true anomaly of 0.
    /// <para>
    /// When outputting a complete elliptical orbit, the final point in the list has a true anomaly slightly smaller than the first
    /// point in the list. That is, the first and last points in the list are not the same.
    /// </para>
    /// <para>
    /// When outputting a complete open orbit, if the true anomaly of a point is beyond the maximum/minimum true anomaly, then
    /// the Vector3.positiveInfinity point is output in the list. <see cref="MaxTrueAnomaly"/>
    /// </para>
    /// </returns>
    public List<Vector3d> OrbitalPoints(Angle? startTrueAnomaly, Angle? endTrueAnomaly, out List<Angle> trueAnomalies, Angle maxTrueAnomalyStep)
    {
        // This method returns a List containing 3D points that trace the trajectory of this orbit, starting at the 'startTrueAnomaly' 
        // and ending at the 'endTrueAnomaly'. If the start and end points are the same then the whole orbit is output.
        // The 'stepRad' parameter decides the angular spacing of the points in radians. An integer number of points must be output, so the actual
        // angular spacing may be slightly adjusted to ensure this. The actual spacing will never be larger than 'stepRad'.

        // If the start and end points are the same, or if either of the points are null, output the entire orbit
        Angle angularRange;
        if (startTrueAnomaly == null || endTrueAnomaly == null || (startTrueAnomaly == endTrueAnomaly))
            angularRange = Angle.MaxAngle;
        else
            angularRange = endTrueAnomaly.Value - startTrueAnomaly.Value;

        int numberOfPoints = Mathf.CeilToInt(angularRange.RadValue / maxTrueAnomalyStep.RadValue);

        // Adjust the angular spacing to ensure an integer number of points is output.
        Angle actualStep = angularRange.RadValue / numberOfPoints;

        // Create the list to be output and fill it with points
        List<Vector3d> outputPoints = new List<Vector3d>(numberOfPoints);
        trueAnomalies = new List<Angle>(numberOfPoints);

        Angle angle = startTrueAnomaly ?? Angle.Zero;
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

        return outputPoints;
    }

    /// <summary>
    /// Calculates the position of the satellite at a given time.
    /// </summary>
    /// <param name="time">The time in seconds to calculate the satellite's position.</param>
    /// <returns>The position of the satellite at the given time.</returns>
    public Vector3d Time2Point(double time)
    {
        Angle trueAnomaly = Time2TrueAnomaly(time);
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
    public Angle Time2TrueAnomaly(double time)
    {
        double meanAnomaly = Time2MeanAnomaly(time);
        Angle trueAnomaly;

        if (OrbitType == ConicSection.Elliptical)
        {
            // Elliptical orbit case
            double eccentricAnomaly = MeanAnomaly2EccentricAnomaly(meanAnomaly);
            trueAnomaly = (float)EccentricAnomaly2TrueAnomaly(eccentricAnomaly);
            return trueAnomaly;
        }
        else if (OrbitType == ConicSection.Hyperbolic)
        {
            // Hyperbolic orbit case
            if (time == double.PositiveInfinity)
                return MaxTrueAnomaly ?? Angle.Zero;

            if (time == double.NegativeInfinity)
                return 2f * Mathf.PI - (MaxTrueAnomaly ?? Angle.Zero).RadValue;

            double hyperbolicAnomaly = MeanAnomaly2HyperbolicAnomaly(meanAnomaly);
            trueAnomaly = HyperbolicAnomaly2TrueAnomaly(hyperbolicAnomaly);
            return trueAnomaly;
        }

        // Parabolic orbit case
        if (time == double.PositiveInfinity || time == double.NegativeInfinity)
            return Angle.HalfTurn;

        double parabolicAnomaly = MeanAnomaly2ParabolicAnomaly(meanAnomaly);
        trueAnomaly = ParabolicAnomaly2TrueAnomaly(parabolicAnomaly);
        return trueAnomaly;
    }

    /// <summary>
    /// Calculates the velocity of the satellite at a given time.
    /// </summary>
    /// <param name="time">The time in seconds at which to calculate the velocity of the satellite.</param>
    /// <returns>The velocity of the satellite at the given time.</returns>
    public Vector3d Time2Velocity(double time)
    {
        Angle trueAnomaly = Time2TrueAnomaly(time);
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
    public Vector3d TrueAnomaly2Point(Angle trueAnomaly)
    {
        trueAnomaly = Angle.Expel(trueAnomaly, MaxTrueAnomaly ?? Angle.Zero, -(MaxTrueAnomaly ?? Angle.Zero));

        double radius;
        if (trueAnomaly >= MaxTrueAnomaly && trueAnomaly <= -MaxTrueAnomaly)
            return new Vector3d(Vector3.positiveInfinity);

        radius = radiusOfPeriapsis * (1.0 + eccentricity) / (1.0 + eccentricity * Mathf.Cos(trueAnomaly));

        Vector3d output;
        output.x = radius * (Mathf.Cos(longitudeOfAscendingNode) * Mathf.Cos(argumentOfPeriapsis + trueAnomaly) - Mathf.Sin(longitudeOfAscendingNode) * Mathf.Sin(argumentOfPeriapsis + trueAnomaly) * Mathf.Cos(inclination));
        output.y = radius * (Mathf.Sin(longitudeOfAscendingNode) * Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + Mathf.Cos(longitudeOfAscendingNode) * Mathf.Sin(argumentOfPeriapsis + trueAnomaly) * Mathf.Cos(inclination));
        output.z = radius * Mathf.Sin(argumentOfPeriapsis + trueAnomaly) * Mathf.Sin(inclination);
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
    public double TrueAnomaly2Time(Angle trueAnomaly)
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
    public Vector3d TrueAnomaly2Velocity(Angle trueAnomaly)
    {
        double angMomentum = SpecificAngularMomentumVector.magnitude;

        Vector3d output;
        output.x = -Mu / angMomentum * (Mathf.Cos(longitudeOfAscendingNode) * (Mathf.Sin(argumentOfPeriapsis + trueAnomaly) + eccentricity * Mathf.Sin(argumentOfPeriapsis)) + Mathf.Sin(longitudeOfAscendingNode) * (Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + ECC * Mathf.Cos(argumentOfPeriapsis)) * Mathf.Cos(inclination));
        output.y = -Mu / angMomentum * (Mathf.Sin(longitudeOfAscendingNode) * (Mathf.Sin(argumentOfPeriapsis + trueAnomaly) + eccentricity * Mathf.Sin(argumentOfPeriapsis)) - Mathf.Cos(longitudeOfAscendingNode) * (Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + ECC * Mathf.Cos(argumentOfPeriapsis)) * Mathf.Cos(inclination));
        output.z = Mu / angMomentum * (Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + eccentricity * Mathf.Cos(argumentOfPeriapsis)) * Mathf.Sin(inclination);
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
    private Angle EccentricAnomaly2TrueAnomaly(double eccentricAnomaly)
    {
        return (float)(2.0 * Math.Atan2(Math.Sqrt(1.0 + eccentricity) * Math.Tan(eccentricAnomaly / 2.0), Math.Sqrt(1.0 - eccentricity)));
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
    private Angle HyperbolicAnomaly2TrueAnomaly(double hyperbolicAnomaly)
    {
        return (float)(2.0 * Math.Atan2(Math.Sqrt(eccentricity + 1.0) * Math.Tanh(hyperbolicAnomaly / 2.0), Math.Sqrt(eccentricity - 1.0)));
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
        double error = 1.0f;
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
    private Angle ParabolicAnomaly2TrueAnomaly(double parabolicAnomaly)
    {
        return (float)(2.0f * Math.Atan(parabolicAnomaly / Math.Sqrt(2.0 * radiusOfPeriapsis)));
    }

    // Used by Time2TrueAnomaly() for all types of orbit.
    private double Time2MeanAnomaly(double time)
    {
        return MeanMotion * (time - timeOfPeriapsisPassage);
    }

    // Used by TrueAnomaly2Time() in the elliptical orbit case.
    private double TrueAnomaly2EccentricAnomaly(Angle trueAnomaly)
    {
        return 2.0 * Math.Atan2(Math.Sqrt(1.0 - eccentricity) * Math.Tan(trueAnomaly / 2.0), Math.Sqrt(1.0 + eccentricity));
    }

    // Used by TrueAnomaly2Time() in the hyperbolic orbit case.
    private double TrueAnomaly2HyperbolicAnomaly(Angle trueAnomaly)
    {
        // If the input true anomaly is unreachable by this hyperbolic orbit, change the true anomaly to whichever of the
        // max/min true anomaly is closest.
        trueAnomaly = Angle.Expel(trueAnomaly, MaxTrueAnomaly ?? Angle.Zero, 2f * Mathf.PI - (MaxTrueAnomaly ?? Angle.Zero));

        if (trueAnomaly == MaxTrueAnomaly)
            return double.PositiveInfinity;

        if (trueAnomaly == 2f * Mathf.PI - MaxTrueAnomaly)
            return double.NegativeInfinity;

        return 2.0 * Atanh(Math.Sqrt((eccentricity - 1.0) / (ECC + 1.0)) * Math.Tan(trueAnomaly / 2.0));
    }

    // Used by TrueAnomaly2Time() in the parabolic orbit case.
    private double TrueAnomaly2ParabolicAnomaly(Angle trueAnomaly)
    {
        if (trueAnomaly.RadValue == Mathf.PI)
            return float.PositiveInfinity;
        return Math.Sqrt(2.0 * radiusOfPeriapsis) * Mathf.Tan(trueAnomaly / 2f);
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
}
