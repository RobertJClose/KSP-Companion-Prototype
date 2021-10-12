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

    // The name of the gravitational body.
    protected string nameOfBody;

    // The radius of the gravitational body, in metres. 
    // For KSP bodies, this is the radius quoted in game and in the wiki.
    // For Solar bodies, this value is the equatorial radius quoted on Wikipedia of the planet on the 9th of
    // October, 2021. 
    protected double radius;

    #endregion

    #region Constructors

    // Initialises a new GravitationalBody object.
    private GravitationalBody(string bodyName, double bodyGravParameter, double bodyRadius)
    {
        nameOfBody = bodyName;
        gravitationalParameter = bodyGravParameter;
        radius = bodyRadius;
    }

    #endregion

    #region Properties

    public static GravitationalBody Bop
    {
        get
        {
            return new GravitationalBody("Bop", 2.4868349e+9, 65_000.0);
        }
    }

    public static GravitationalBody Dres
    {
        get
        {
            return new GravitationalBody("Dres", 2.1484489e+10, 138_000.0);
        }
    }

    public static GravitationalBody Duna
    {
        get
        {
            return new GravitationalBody("Duna", 3.0136321e+11, 320_000.0);
        }
    }

    public static GravitationalBody Earth
    {
        get
        {
            return new GravitationalBody("Earth", 3.986e+14, 6371000.0);
        }
    }

    public static GravitationalBody Eeloo
    {
        get
        {
            return new GravitationalBody("Eeloo", 7.4410815e+10, 210_000.0);
        }
    }

    public static GravitationalBody Eve
    {
        get
        {
            return new GravitationalBody("Eve", 8.1717302e+12, 700_000.0);
        }
    }

    public static GravitationalBody Gilly
    {
        get
        {
            return new GravitationalBody("Gilly", 8_289_449.8, 13_000.0);
        }
    }

    public static GravitationalBody Ike
    {
        get
        {
            return new GravitationalBody("Ike", 1.8568369e+10, 130_000.0);
        }
    }

    public static GravitationalBody Jool
    {
        get
        {
            return new GravitationalBody("Jool", 2.8252800e+14, 6_000_000.0);
        }
    }

    public static GravitationalBody Jupiter
    {
        get
        {
            return new GravitationalBody("Jupiter", 1.2669e+17, 69911000.0);
        }
    }

    public static GravitationalBody Kerbin
    {
        get
        {
            return new GravitationalBody("Kerbin", 3.5316e+12, 6e+5);
        }
    }

    public static GravitationalBody Kerbol
    {
        get
        {
            return new GravitationalBody("Kerbol", 1.1723328e+18, 261_600_000.0);
        }
    }

    public static GravitationalBody Laythe
    {
        get
        {
            return new GravitationalBody("Laythe", 1.9620000e+12, 500_000.0);
        }
    }

    public static GravitationalBody Minmus
    {
        get
        {
            return new GravitationalBody("Minmus", 1.7658000e+9, 60_000.0);
        }
    }

    public static GravitationalBody Moho
    {
        get
        {
            return new GravitationalBody("Moho", 1.6860938e+11, 250_000.0);
        }
    }

    public static GravitationalBody Mun
    {
        get
        {
            return new GravitationalBody("Mun", 6.5138398e+10, 200_000.0);
        }
    }

    public static GravitationalBody Pol
    {
        get
        {
            return new GravitationalBody("Pol", 7.2170208e+8, 44_000.0);
        }
    }

    public static GravitationalBody Tylo
    {
        get
        {
            return new GravitationalBody("Tylo", 2.8252800e+12, 600_000.0);
        }
    }

    public static GravitationalBody Vall
    {
        get
        {
            return new GravitationalBody("Vall", 2.0748150e+11, 300_000.0);
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
            double periapsisRadius = Mathd.Ceil(radius * 1.05 / 25000.0) * 25000.0; // The Ceil() call and factors of 25,000 make the default radius round up to a multiple of 25km
            double eccentricity = 0.0;
            Angle inclination = 0f;
            Angle argumentOfPeriapsis = 0f;
            Angle longitudeOfAscendingNode = 0f;
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
    /// Serves as the hash function for this GravitationalBody.
    /// </summary>
    /// <returns>A hask code for this GravitationalBody.</returns>
    public override int GetHashCode()
    {
        return (gravitationalParameter + radius).GetHashCode();
    }

    #endregion
}