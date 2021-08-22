using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Orbit
{
    // ********** Backing fields **********
    protected GravitationalBody attractingBody;

    protected double periapsisRadius;            // RPE: Strictly positive, measured in metres.
    protected double eccentricity;               // ECC: Strictly positive. Elliptical orbit: 0 <= ECC < 1. Parabolic orbit: ECC == 1. Hyperbolic orbit: ECC > 1. 
    protected Angle inclination;                // INC: 0 < INC < PI. Specifies the angle between the orbital plane and the reference XY plane
    protected Angle argumentOfPeriapsis;        // APE: 0 < APE < 2PI. Specifies the angle between the ascending node vector (NVector) and the point of periapsis 
    protected Angle longitudeOfAscendingNode;   // LAN: 0 < LAN < 2PI. Specifies the angle between the reference direction (X axis) and the ascending node vector (NVector)
    protected double timeOfPeriapsisPassage;     // TPP: Specifies the bodies position in its orbit.

    // ********** Properties **********
    // *** Properties of the orbit ***
    // Orbital elements
    // Periapsis radius
    public double RPE
    {
        get { return periapsisRadius; }
        set
        {
            if (value < 0.0)
                periapsisRadius = 0.0;
            else
                periapsisRadius = value;
        }
    }
    // Eccentricity
    public double ECC
    {
        get { return eccentricity; }
        set
        {
            if (value < 0.0)
                eccentricity = 0.0;
            else
                eccentricity = value;
        }
    }
    // Inclination
    public Angle INC { get { return inclination; } set { inclination = value % Mathf.PI; } }
    // Argument of periapsis
    public Angle APE { get { return argumentOfPeriapsis; } set { argumentOfPeriapsis = value; } }
    // Longitude of the ascending node
    public Angle LAN { get { return longitudeOfAscendingNode; } set { longitudeOfAscendingNode = value; } }
    // Time of periapsis passage
    public double TPP { get { return timeOfPeriapsisPassage; } set { timeOfPeriapsisPassage = value; } }

    // Alternative elements
    // Semi-major axis
    public double SMA
    {
        get
        {
            if (OrbitType == ConicSection.Parabolic)
                return float.PositiveInfinity;
            else
                return periapsisRadius / (1.0 - eccentricity);
        }
    }

    // Orbital vectors
    public Vector3 NVector
    {
        // The nodal vector points to the point where the orbiting body crosses the reference XY plane from below, with a magnitude of Sin(inclination).
        get
        {
            float magnitude = Mathf.Sin(inclination);
            Quaternion rotation = Quaternion.AngleAxis(longitudeOfAscendingNode.DegValue, Vector3.forward);
            return magnitude * (rotation * Vector3.right);
        }
    }

    public Vector3 HVector
    {
        // The specific angular momentum vector that is normal to the orbital plane, with a magnitude equal to the specific angular momentum h.
        get
        {
            float magnitude = (float)Math.Sqrt(SemiLatusRectum * MU);
            Quaternion rotation = Quaternion.AngleAxis(inclination.DegValue, Vector3.right) * Quaternion.AngleAxis(longitudeOfAscendingNode.DegValue, Vector3.forward);
            return magnitude * (rotation * Vector3.forward);
        }
    }

    public Vector3 EVector
    {
        // The eccentricty vector that points at the point of periapsis, with a magnitude equal to the eccentricity e.
        get
        {
            float magnitude = (float)eccentricity;
            Quaternion rotation = Quaternion.AngleAxis(longitudeOfAscendingNode.DegValue, Vector3.forward) * Quaternion.AngleAxis(argumentOfPeriapsis.DegValue, HVector);
            return magnitude * (rotation * Vector3.right);
        }
    }

    // Other properties of the orbit
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

    public double SpecificEnergy
    {
        get
        {
            return (1.0 - 2.0 / (1.0 + eccentricity)) * MU * (1.0 + eccentricity) * (1.0 + eccentricity) / (2.0 * SemiLatusRectum);
        }
    }

    public double MeanMotion
    {
        get
        {
            if (OrbitType == ConicSection.Parabolic)
            {
                // Parabolic case
                return Mathf.Sqrt(MU);
            }

            return Math.Sqrt(MU / (Math.Abs(SMA) * Math.Abs(SMA) * Math.Abs(SMA)));
        }
    }

    public double SemiLatusRectum
    {
        get
        {
            return periapsisRadius * (1.0 + eccentricity);
        }
    }

    public Vector3 PeriapsisPoint
    {
        get
        {
            return TrueAnomaly2Point(Angle.Zero);
        }
    }

    public Vector3? ApoapsisPoint
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
                return null;
            else
                return TrueAnomaly2Point(Angle.HalfTurn);
        }
    }

    public Vector3? AscendingNode
    {
        get
        {
            Vector3 node = TrueAnomaly2Point(-argumentOfPeriapsis);
            if (node.magnitude == float.PositiveInfinity)
                return null;
            else
                return node;
        }
    }

    public Vector3? DescendingNode
    {
        get
        {
            Vector3 node = TrueAnomaly2Point(-argumentOfPeriapsis + Angle.HalfTurn);
            if (node.magnitude == float.PositiveInfinity)
                return null;
            else
                return node;
        }
    }

    public double Period
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
                return float.PositiveInfinity;
            else
                return 2.0 * Math.PI * Math.Sqrt(SMA * SMA * SMA / MU);
        }
    }

    public double ApoapsisRadius
    {
        get
        {
            if (OrbitType != ConicSection.Elliptical)
                return float.PositiveInfinity;
            else
                return (1f + eccentricity) / (1f - eccentricity) * periapsisRadius;
        }
    }

    public double ExcessVelocity
    {
        get
        {
            return Math.Sqrt(MU / (-SMA));
        }
    }

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

    // *** Properties of the attracting body ***
    public GravitationalBody GravitationalBody { get { return attractingBody; } set { attractingBody = value; } }
    public float MU { get { return attractingBody.GravParameter; } }

    // ********** Methods **********
    public static Orbit StateVectors2Orbit(GravitationalBody body, Vector3 position, Vector3 velocity, double time)
    {
        double periapsisRadius;
        double eccentricity;
        float inclination;
        float argumentOfPeriapsis;
        float longitudeOfAscendingNode;
        double timeOfPeriapsisPassage;
        float trueAnomaly;

        Vector3 hVector = Vector3.Cross(position, velocity);
        Vector3 eVector = 1.0f / body.GravParameter * Vector3.Cross(velocity, hVector) - position.normalized;
        Vector3 nVector = Vector3.Cross(Vector3.forward, hVector.normalized);

        trueAnomaly = (float)Math.Acos(Vector3.Dot(eVector, position) / (eVector.magnitude * position.magnitude));
        if (Vector3.Dot(position, velocity) < 0)
            trueAnomaly = 2.0f * Mathf.PI - trueAnomaly;

        eccentricity = eVector.magnitude;

        double semiLatusRectum = position.magnitude * (1.0f + eccentricity * Mathf.Cos(trueAnomaly));
        periapsisRadius = semiLatusRectum / (1.0f + eccentricity);
        
        // If the orbit has an inclination of zero, then the nVector will equal the zero vector and the argumentOfPeriapsis and the
        // longitudeOfTheAscendingNode become undefined. Their sum remains well defined however. I have choosen that the argumentOfPeriapsis
        // will equal zero while the longitudeOfTheAscendingNode gets a value.
        if (nVector == Vector3.zero)
        {
            inclination = 0.0f;
            argumentOfPeriapsis = 0.0f;
            longitudeOfAscendingNode = Vector3.SignedAngle(Vector3.right, eVector, Vector3.forward) * Mathf.Deg2Rad;
        }
        else
        {
            inclination = (float)Math.Atan2(nVector.magnitude * hVector.magnitude, hVector.z);
            
            argumentOfPeriapsis = (float)Math.Acos(Vector3.Dot(nVector, eVector) / (nVector.magnitude * eVector.magnitude));
            if (eVector.z < 0)
                argumentOfPeriapsis = 2.0f * Mathf.PI - argumentOfPeriapsis;
            
            longitudeOfAscendingNode = (float)Math.Acos(nVector.x / nVector.magnitude);
            if (nVector.y < 0)
                longitudeOfAscendingNode = 2.0f * Mathf.PI - longitudeOfAscendingNode;
        }

        Orbit orbit = new Orbit(periapsisRadius, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, 0.0f, body);
        double timeFromPeriapsisToTrueAnomaly = orbit.TrueAnomaly2Time(trueAnomaly);
        timeOfPeriapsisPassage = time - timeFromPeriapsisToTrueAnomaly;
        orbit.TPP = timeOfPeriapsisPassage;

        return orbit;
    }

    public static Orbit FindTransferOrbit(GravitationalBody body, Vector3 positionOne, double timeOne, Vector3 positionTwo, double timeTwo)
    {
        Orbit orbit = LambertsProblemHelper.Solver(body, positionOne, timeOne, positionTwo, timeTwo);

        Debug.Log(orbit);

        return orbit;
    }

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

    private double Time2MeanAnomaly(double time)
    {
        return MeanMotion * (time - timeOfPeriapsisPassage);
    }

    private double MeanAnomaly2Time(double meanAnomaly)
    {
        return meanAnomaly / MeanMotion + timeOfPeriapsisPassage;
    }

    private double MeanAnomaly2EccentricAnomaly(double meanAnomaly)
    {
        // Kepler's equation relates the mean anomaly to the eccentric anomaly, but is transcendental in the unknown variable E
        // An initial guess is made for E (from Prussing & Conway), then iteration is used to find E.

        // Initial guess
        double u = meanAnomaly + ECC; // Used in the initial guess.
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

    private double EccentricAnomaly2MeanAnomaly(double eccentricAnomaly)
    {
        return eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly);
    }

    private double TrueAnomaly2EccentricAnomaly(Angle trueAnomaly)
    {
        return 2.0 * Math.Atan2(Math.Sqrt(1.0 - eccentricity) * Math.Tan(trueAnomaly / 2.0), Math.Sqrt(1.0 + eccentricity));
    }

    private Angle EccentricAnomaly2TrueAnomaly(double eccentricAnomaly)
    {
        return (float)(2.0 * Math.Atan2(Math.Sqrt(1.0 + eccentricity) * Math.Tan(eccentricAnomaly / 2.0), Math.Sqrt(1.0 - eccentricity)));
    }

    private double TrueAnomaly2ParabolicAnomaly(Angle trueAnomaly)
    {
        if (trueAnomaly.RadValue == Mathf.PI)
            return float.PositiveInfinity;
        return Math.Sqrt(2.0 * periapsisRadius) * Mathf.Tan(trueAnomaly / 2f);
    }

    private Angle ParabolicAnomaly2TrueAnomaly(double parabolicAnomaly)
    {
        return (float)(2.0f * Math.Atan(parabolicAnomaly / Math.Sqrt(2.0 * periapsisRadius)));
    }

    private double ParabolicAnomaly2MeanAnomaly(double parabolicAnomaly)
    {
        return periapsisRadius * parabolicAnomaly + parabolicAnomaly * parabolicAnomaly * parabolicAnomaly / 6.0;
    }

    private double TrueAnomaly2HyperbolicAnomaly(Angle trueAnomaly)
    {
        trueAnomaly = Angle.Expel(trueAnomaly, MaxTrueAnomaly ?? Angle.Zero, 2f * Mathf.PI - (MaxTrueAnomaly ?? Angle.Zero));

        if (trueAnomaly == MaxTrueAnomaly)
            return double.PositiveInfinity;

        if (trueAnomaly == 2f * Mathf.PI - MaxTrueAnomaly)
            return double.NegativeInfinity;

        return 2.0 * Atanh(Math.Sqrt((eccentricity - 1.0) / (ECC + 1.0)) * Math.Tan(trueAnomaly / 2.0));
    }

    private Angle HyperbolicAnomaly2TrueAnomaly(double hyperbolicAnomaly)
    {
        return (float)(2.0 * Math.Atan2(Math.Sqrt(eccentricity + 1.0) * Math.Tanh(hyperbolicAnomaly / 2.0), Math.Sqrt(eccentricity - 1.0)));
    }

    private double HyperbolicAnomaly2MeanAnomaly(double hyperbolicAnomaly)
    {
        if (hyperbolicAnomaly == double.PositiveInfinity)
            return double.PositiveInfinity;
        else if (hyperbolicAnomaly == double.NegativeInfinity)
            return double.NegativeInfinity;

        return eccentricity * Math.Sinh(hyperbolicAnomaly) - hyperbolicAnomaly;
    }

    private double MeanAnomaly2ParabolicAnomaly(double meanAnomaly)
    {
        // For the parabolic case Barker's equation can give us the exact time without needing to solve a transcendental equation.
        // We do still have to find the one real root of a depressed cubic polynomial using Cardano's method.
        double A = Math.Sqrt(9.0 * meanAnomaly * meanAnomaly + 8.0 * periapsisRadius * periapsisRadius * periapsisRadius);
        double termOne = Math.Pow(3.0 * meanAnomaly + A, 1.0 / 3.0);
        double termTwo = (3.0 * meanAnomaly - A > 0) ? Math.Pow(3.0 * meanAnomaly - A, 1.0 / 3.0) : - Math.Pow(3.0 * -meanAnomaly + A, 1.0 / 3.0); // Ugly but works without needing to define a custom cube root function that accepts negative inputs with fractional powers.
        return termOne + termTwo;
    }

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

        // If the input meanAnomaly is large enough, the Sinh and Cosh functions will output NaN. In this case the method should return Infinity with the same sign as meanAnomaly.
        if (double.IsNaN(hyperbolicAnomaly))
            return (meanAnomaly > 0.0) ? double.PositiveInfinity : double.NegativeInfinity;

        return hyperbolicAnomaly;
    }

    public Vector3 TrueAnomaly2Point(Angle trueAnomaly)
    {
        trueAnomaly = Angle.Expel(trueAnomaly, MaxTrueAnomaly ?? Angle.Zero, -(MaxTrueAnomaly ?? Angle.Zero));

        double radius;
        if (trueAnomaly >= MaxTrueAnomaly && trueAnomaly <= - MaxTrueAnomaly)
            return Vector3.positiveInfinity;
        
        radius = periapsisRadius * (1.0f + eccentricity) / (1.0f + eccentricity * Mathf.Cos(trueAnomaly));

        Vector3 output;
        output.x = (float)(radius * (Mathf.Cos(longitudeOfAscendingNode) * Mathf.Cos(argumentOfPeriapsis + trueAnomaly) - Mathf.Sin(longitudeOfAscendingNode) * Mathf.Sin(argumentOfPeriapsis + trueAnomaly) * Mathf.Cos(inclination)));
        output.y = (float)(radius * (Mathf.Sin(longitudeOfAscendingNode) * Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + Mathf.Cos(longitudeOfAscendingNode) * Mathf.Sin(argumentOfPeriapsis + trueAnomaly) * Mathf.Cos(inclination)));
        output.z = (float)(radius * Mathf.Sin(argumentOfPeriapsis + trueAnomaly) * Mathf.Sin(inclination));
        return output;
    }

    public Vector3 TrueAnomaly2Velocity(Angle trueAnomaly)
    {
        float angMomentum = HVector.magnitude;

        Vector3 output;
        output.x = (float)(-MU / angMomentum * (Mathf.Cos(longitudeOfAscendingNode) * (Mathf.Sin(argumentOfPeriapsis + trueAnomaly) + eccentricity * Mathf.Sin(argumentOfPeriapsis)) + Mathf.Sin(longitudeOfAscendingNode) * (Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + ECC * Mathf.Cos(argumentOfPeriapsis)) * Mathf.Cos(inclination)));
        output.y = (float)(-MU / angMomentum * (Mathf.Sin(longitudeOfAscendingNode) * (Mathf.Sin(argumentOfPeriapsis + trueAnomaly) + eccentricity * Mathf.Sin(argumentOfPeriapsis)) - Mathf.Cos(longitudeOfAscendingNode) * (Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + ECC * Mathf.Cos(argumentOfPeriapsis)) * Mathf.Cos(inclination)));
        output.z = (float)(MU / angMomentum * (Mathf.Cos(argumentOfPeriapsis + trueAnomaly) + eccentricity * Mathf.Cos(argumentOfPeriapsis)) * Mathf.Sin(inclination));
        return output;
    }

    public Vector3 Time2Point(double time)
    {
        Angle trueAnomaly = Time2TrueAnomaly(time);
        return TrueAnomaly2Point(trueAnomaly);
    }

    public Vector3 Time2Velocity(double time)
    {
        Angle trueAnomaly = Time2TrueAnomaly(time);
        return TrueAnomaly2Velocity(trueAnomaly);
    }

    public List<Vector3> OrbitalPoints(Angle? startTrueAnomaly, Angle? endTrueAnomaly)
    {
        // This method exists purely to avoid an optional paramter in OrbitalPoints(Angle?, Angle?, Angle, Angle[]). Read its documentation below.
        return OrbitalPoints(startTrueAnomaly, endTrueAnomaly, Constants.OrbitDefaultStepRad, out _);
    }

    public List<Vector3> OrbitalPoints(Angle? startTrueAnomaly, Angle? endTrueAnomaly, Angle stepRad)
    {
        // This method exists purely to avoid an optional paramter in OrbitalPoints(Angle?, Angle?, Angle, Angle[]). Read its documentation below.
        return OrbitalPoints(startTrueAnomaly, endTrueAnomaly, stepRad, out _);
    }

    public List<Vector3> OrbitalPoints(Angle? startTrueAnomaly, Angle? endTrueAnomaly, out List<Angle> trueAnomalies)
    {
        // This method exists purely to avoid an optional paramter in OrbitalPoints(Angle?, Angle?, Angle, Angle[]). Read its documentation below.
        return OrbitalPoints(startTrueAnomaly, endTrueAnomaly, Constants.OrbitDefaultStepRad, out trueAnomalies);
    }

    public List<Vector3> OrbitalPoints(Angle? startTrueAnomaly, Angle? endTrueAnomaly, Angle stepRad, out List<Angle> trueAnomalies)
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

        int numberOfPoints = Mathf.CeilToInt(angularRange.RadValue / stepRad.RadValue);

        // Adjust the angular spacing if 'stepRad' doesn't evenly divide the range of angles.
        Angle actualStep = angularRange.RadValue / numberOfPoints; 

        // Create the list to be output and fill it with points
        List<Vector3> outputPoints = new List<Vector3>(numberOfPoints);
        trueAnomalies = new List<Angle>(numberOfPoints);

        Angle angle = startTrueAnomaly ?? Angle.Zero;
        Vector3 newPoint;
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

    // Inverse hyperbolic tangent function
    private static double Atanh(double x)
    {
        return 0.5 * Math.Log((1.0 + x) / (1.0 - x));
    }

    public override string ToString()
    {
        return "RPE: " + periapsisRadius + "\nECC: " + eccentricity + "\nINCd: " + inclination.DegValue + "\nAPEd: " + argumentOfPeriapsis.DegValue + "\nLANd: " + longitudeOfAscendingNode.DegValue + "\nTPP: " + timeOfPeriapsisPassage + '\n' + base.ToString();
    }

    // ********** Constructors **********

    public Orbit(double periapsisRadius, double eccentricity, Angle inclination, Angle argumentOfPeriapsis, Angle longitudeOfAscendingNode, double timeOfPeriapsisPassage, GravitationalBody gravitationalBody)
    {
        RPE = periapsisRadius;
        ECC = eccentricity;
        INC = inclination;
        APE = argumentOfPeriapsis;
        LAN = longitudeOfAscendingNode;
        TPP = timeOfPeriapsisPassage;

        GravitationalBody = gravitationalBody;
    }

    // **********************************

    public enum ConicSection
    {
        Elliptical,
        Parabolic,
        Hyperbolic
    }
}
