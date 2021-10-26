using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a celestial body with a gravitational field.
/// </summary>
/// <remarks>
/// Each instance of this class represents a simple celestial body with a name, radius, and gravitational field.
/// </remarks>
public class GravitationalBody : System.IEquatable<GravitationalBody>
{
    #region Fields

    // A list of all the gravitational bodies in the KSP solar system. Ordered by ID number.
    public static List<GravitationalBody> ListOfKSPBodies = new List<GravitationalBody> { Kerbol, Kerbin, Mun, Minmus, Moho, Eve, Duna, Ike, Jool, Laythe, Vall, Bop, Tylo, Gilly, Pol, Dres, Eeloo};

    // The gravitational parameter for the body. This is equal to the mass of the body multiplied by
    // the gravitational constant G. In writing, this is often represented by the Greek letter mu.
    protected double gravitationalParameter;

    // The amount of rotation of the body about its axis at an epoch time.
    protected Angled initialRotation;

    // The name of the gravitational body.
    protected string nameOfBody;

    // The orbit that the gravitational body is on.
    protected Orbit orbit;

    // The radius of the gravitational body, in metres. 
    // For KSP bodies, this is the radius quoted in game and in the wiki.
    // For Solar bodies, this value is the equatorial radius quoted on Wikipedia of the planet on the 9th of
    // October, 2021. 
    protected double radius;

    // Time time in seconds for the body to rotate once around its axis.
    protected double siderealRotationalPeriod;

    #endregion

    #region Constructors

    // Initialises a new GravitationalBody object.
    private GravitationalBody(string nameOfBody, double gravitationalParameter, double radius, double siderealRotationalPeriod, Angled initialRotation, Orbit orbit)
    {
        this.nameOfBody = nameOfBody;
        this.gravitationalParameter = gravitationalParameter;
        this.radius = radius;
        this.siderealRotationalPeriod = siderealRotationalPeriod;
        this.initialRotation = initialRotation;
        this.orbit = orbit;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Bop.
    /// </summary>
    public static GravitationalBody Bop
    {
        get
        {
            return new GravitationalBody("Bop", 2.4868349e+9, 65_000.0, 544_507.43, 23.0 / 18.0 * System.Math.PI, Orbit.BopOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Dres.
    /// </summary>
    public static GravitationalBody Dres
    {
        get
        {
            return new GravitationalBody("Dres", 2.1484489e+10, 138_000.0, 34_800.0, 5.0 / 36.0 * System.Math.PI, Orbit.DresOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Duna.
    /// </summary>
    public static GravitationalBody Duna
    {
        get
        {
            return new GravitationalBody("Duna", 3.0136321e+11, 320_000.0, 65_517.859, System.Math.PI / 2.0, Orbit.DunaOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the planet Earth.
    /// </summary>
    public static GravitationalBody Earth
    {
        get
        {
            return new GravitationalBody("Earth", 3.986e+14, 6371000.0, 86_164.10035, 0.0, null);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Eeloo.
    /// </summary>
    public static GravitationalBody Eeloo
    {
        get
        {
            return new GravitationalBody("Eeloo", 7.4410815e+10, 210_000.0, 19_460.0, 5.0 / 36.0 * System.Math.PI, Orbit.EelooOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody with all fields set to the default/null value for their type.
    /// </summary>
    public static GravitationalBody Empty
    {
        get
        {
            return new GravitationalBody("", 0.0, 0.0, 0.0, 0.0, null);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Eve.
    /// </summary>
    public static GravitationalBody Eve
    {
        get
        {
            return new GravitationalBody("Eve", 8.1717302e+12, 700_000.0, 80_500.0, 0.0, Orbit.EveOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Gilly.
    /// </summary>
    public static GravitationalBody Gilly
    {
        get
        {
            return new GravitationalBody("Gilly", 8_289_449.8, 13_000.0, 28_255.0, System.Math.PI / 36.0, Orbit.GillyOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Ike.
    /// </summary>
    public static GravitationalBody Ike
    {
        get
        {
            return new GravitationalBody("Ike", 1.8568369e+10, 130_000.0, 65_517.862, 0.0, Orbit.IkeOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Jool.
    /// </summary>
    public static GravitationalBody Jool
    {
        get
        {
            return new GravitationalBody("Jool", 2.8252800e+14, 6_000_000.0, 36_000.0, 0.0, Orbit.JoolOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the planet Jupiter.
    /// </summary>
    public static GravitationalBody Jupiter
    {
        get
        {
            return new GravitationalBody("Jupiter", 1.2669e+17, 69911000.0, 35_730.0, 0.0, null);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Kerbin.
    /// </summary>
    public static GravitationalBody Kerbin
    {
        get
        {
            return new GravitationalBody("Kerbin", 3.5316e+12, 6e+5, 21_549.425, System.Math.PI / 2.0, Orbit.KerbinOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP star Kerbol.
    /// </summary>
    public static GravitationalBody Kerbol
    {
        get
        {
            return new GravitationalBody("Kerbol", 1.1723328e+18, 261_600_000.0, 432_000.0, 0.0, null);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Laythe.
    /// </summary>
    public static GravitationalBody Laythe
    {
        get
        {
            return new GravitationalBody("Laythe", 1.9620000e+12, 500_000.0, 52_980.879, System.Math.PI / 2.0, Orbit.LaytheOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Minmus.
    /// </summary>
    public static GravitationalBody Minmus
    {
        get
        {
            return new GravitationalBody("Minmus", 1.7658000e+9, 60_000.0, 40_400.0, 23.0 / 18.0 * System.Math.PI, Orbit.MinmusOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP planet Moho.
    /// </summary>
    public static GravitationalBody Moho
    {
        get
        {
            return new GravitationalBody("Moho", 1.6860938e+11, 250_000.0, 1_210_000.0, 19.0 / 18.0 * System.Math.PI, Orbit.MohoOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Mun.
    /// </summary>
    public static GravitationalBody Mun
    {
        get
        {
            return new GravitationalBody("Mun", 6.5138398e+10, 200_000.0, 138_984.38, 23.0 / 18.0 * System.Math.PI, Orbit.MunOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Pol.
    /// </summary>
    public static GravitationalBody Pol
    {
        get
        {
            return new GravitationalBody("Pol", 7.2170208e+8, 44_000.0, 901_902.62, 5.0 / 36.0 * System.Math.PI, Orbit.PolOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Tylo.
    /// </summary>
    public static GravitationalBody Tylo
    {
        get
        {
            return new GravitationalBody("Tylo", 2.8252800e+12, 600_000.0, 211_926.36, 0.0, Orbit.TyloOrbit);
        }
    }

    /// <summary>
    /// Gets a GravitationalBody representing the KSP moon Vall.
    /// </summary>
    public static GravitationalBody Vall
    {
        get
        {
            return new GravitationalBody("Vall", 2.0748150e+11, 300_000.0, 105_962.09, 0.0, Orbit.VallOrbit);
        }
    }

    /// <summary>
    /// Gets the default orbit around a gravitational body.
    /// </summary>
    /// <remarks>
    /// The default orbit has a periapsis radius equal to the smallest multiple of 25,000 that is larger than 1.05 times the body's
    /// radius. The eccentricity, inclination, argument of periapsis, longitude of the ascending node, and time of periapsis passage
    /// for the default orbit is 0.0.
    /// </remarks>
    public Orbit DefaultOrbit
    {
        get
        {
            double periapsisRadius = Mathd.Ceil(radius * 1.2 / 25000.0) * 25000.0; // The Ceil() call and factors of 25,000 make the default radius round up to a multiple of 25km
            double eccentricity = 0.0;
            Angled inclination = 0f;
            Angled argumentOfPeriapsis = 0f;
            Angled longitudeOfAscendingNode = 0f;
            double timeOfPeriapsisPassage = 0.0;

            return new Orbit(periapsisRadius, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, timeOfPeriapsisPassage, this);
        }
    }

    /// <summary>
    /// Gets the value of the gravitational parameter for the body.
    /// </summary>
    public double GravParameter
    {
        get
        {
            return gravitationalParameter;
        }
    }

    /// <summary>
    /// Gets the name of the body.
    /// </summary>
    public string Name
    {
        get
        {
            return nameOfBody;
        }
    }

    /// <summary>
    /// Gets the radius of the body.
    /// </summary>
    public double Radius
    {
        get
        {
            return radius;
        }
    }

    /// <summary>
    /// Gets an Orbit around this gravitational body, with all the orbital elements set to zero.
    /// </summary>
    public Orbit ZeroOrbit
    {
        get
        {
            return new Orbit(0.0, 0.0, 0.0f, 0.0f, 0.0f, 0.0, this);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Checks to see to if two GravitationalBody objects are equal.
    /// </summary>
    /// <param name="bodyOne">The first GravitationalBody.</param>
    /// <param name="bodyTwo">The second GravitationalBody.</param>
    /// <returns>True if the bodies are the same. False otherwise.</returns>
    public static bool operator==(GravitationalBody bodyOne, GravitationalBody bodyTwo)
    {
        if (bodyOne is null && bodyTwo is null)
            return true;

        // This OR condition is unreachable if both conditions are true, so is an effective XOR.
        if (bodyOne is null || bodyTwo is null)
            return false;

        return bodyOne.Equals(bodyTwo);
    }

    /// <summary>
    /// Checks to see to if two GravitationalBody objects are not equal.
    /// </summary>
    /// <param name="bodyOne">The first GravitationalBody.</param>
    /// <param name="bodyTwo">The second GravitationalBody.</param>
    /// <returns>True if the bodies are not the same, or if either body is null. False otherwise.</returns>
    public static bool operator!=(GravitationalBody bodyOne, GravitationalBody bodyTwo)
    {
        if (bodyOne is null && bodyTwo is null)
            return false;

        // This OR condition is unreachable if both conditions are true, so is an effective XOR.
        if (bodyOne is null || bodyTwo is null)
            return true;

        return !bodyOne.Equals(bodyTwo);
    }

    /// <summary>
    /// Checks to see if an object is the same as this GravitationalBody.
    /// </summary>
    /// <param name="obj">The object to be checked for equality.</param>
    /// <returns>False if the other object is null, of a different type, or is a different GravitationalBody object.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || !GetType().Equals(obj.GetType()))
            return false;

        return Equals((GravitationalBody)obj);
    }

    /// <summary>
    /// Checks to see if another GravitationalBody is the same as this GravitationalBody.
    /// </summary>
    /// <param name="other">The other GravitationalBody to be checked for equality.</param>
    /// <returns>True is the other GravitationalBody has the same name. False otherwise.</returns>
    public bool Equals(GravitationalBody other)
    {
        if (other is null)
            return false;

        return nameOfBody == other.Name;
    }

    /// <summary>
    /// Calculates the position of the body at the given time.
    /// </summary>
    /// <param name="time">The time at which to find the position.</param>
    /// <returns>A Vector3d of the position of the centre of the body.</returns>
    public Vector3d FindPosition(double time)
    {
        if (double.IsNaN(time) || double.IsInfinity(time))
            throw new System.ArgumentException("The input time cannot be NaN or infinite", "time");

        return orbit.Time2Point(time);
    }

    /// <summary>
    /// Calculates what the rotation of the body will be at a given time.
    /// </summary>
    /// <param name="time">The time at which the rotation will be calculated.</param>
    /// <returns>The rotation of the body.</returns>
    /// <exception cref="System.ArgumentException">The input time cannot be NaN or infinite.</exception>
    public Angled FindRotation(double time)
    {
        if (double.IsNaN(time) || double.IsInfinity(time))
            throw new System.ArgumentException("time cannot be NaN or infinite.", "time");

        double percentRotation = time % siderealRotationalPeriod;
        return initialRotation + percentRotation * 2.0 * System.Math.PI;
    }

    /// <summary>
    /// Serves as the hash function for this GravitationalBody.
    /// </summary>
    /// <returns>A hask code for this GravitationalBody.</returns>
    public override int GetHashCode()
    {
        return (gravitationalParameter + radius).GetHashCode();
    }

    /// <summary>
    /// Converts a measurement of latitude, longitude, and altitude of a point at a particular time to a Vector3d.
    /// </summary>
    /// <param name="latitude">The lattitude of the point.</param>
    /// <param name="longitude">The longitude of the point.</param>
    /// <param name="altitude">The altitude of the point.</param>
    /// <param name="time">The time at which the measurements were made.</param>
    /// <returns>The Vector3d from the origin to the point.</returns>
    /// <exception cref="System.ArgumentException">The inputs cannot be NaN or infinite.</exception>
    public Vector3d MeasurementsToVector3d(Angled latitude, Angled longitude, double altitude, double time)
    {
        if (Angled.IsNaN(latitude))
            throw new System.ArgumentException("The input latitude cannot be NaN.", "latitude");

        if (Angled.IsNaN(longitude))
            throw new System.ArgumentException("The input longitude cannot be NaN.", "longitude");

        if (double.IsNaN(altitude) || double.IsInfinity(altitude))
            throw new System.ArgumentException("The input altitude cannot be NaN or infinite.", "altitude");

        if (double.IsNaN(time) || double.IsInfinity(time))
            throw new System.ArgumentException("The input time cannot be NaN or infinite.", "time");

        Angled bodyRotation = FindRotation(time);
        Vector3d zeroLatZeroLonDirection = Quaterniond.AngleAxis(bodyRotation.DegValue, Vector3d.forward) * Vector3d.right;
        Vector3d output = FindPosition(time) + zeroLatZeroLonDirection * (altitude + radius);
        return output;
    }

    #endregion
}