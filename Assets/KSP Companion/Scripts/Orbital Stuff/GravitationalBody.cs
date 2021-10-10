using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalBody
{
    #region Fields

    // A list of all the gravitational bodies in the KSP solar system.
    public static List<GravitationalBody> ListOfKSPBodies = new List<GravitationalBody> { Kerbin };

    // The gravitational parameter for the body. This is equal to the mass of the body multiplied by
    // the gravitational constant G. In writing, this is often represented by the Greek letter mu.
    protected double gravitationalParameter;

    // The ID number for the gravitational body.
    // For KSP bodies, this ID number matches the reference code used by KSP in the save files. See the "Orbit" page on the wiki.
    // For Solar bodies, the ID number is a three digit number: XYY.
    // X corresponds to the planet's/parent planet's order out from the Sun.
    // YY is 00 for planets, and for moons corresponds to the moon's order away from the planet as of semi-major axis
    // measurements on the 10th of October, 2021.
    // So Earth: 300. Jupiter: 500. Bergelmir: 451.
    protected int id;

    // The name of the gravitational body.
    protected string nameOfBody;

    // The radius of the gravitational body, in metres. 
    // For KSP bodies, this is the radius quoted in game and in the wiki.
    // For Solar bodies, this value is the equatorial radius quoted on Wikipedia of the planet on the 9th of
    // October, 2021. 
    protected double radius;

    #endregion

    #region Constructors

    private GravitationalBody(int bodyID, string bodyName, double bodyGravParameter, double bodyRadius)
    {
        id = bodyID;
        nameOfBody = bodyName;
        gravitationalParameter = bodyGravParameter;
        radius = bodyRadius;
    }

    #endregion

    #region Properties

    public static GravitationalBody Earth
    {
        get
        {
            return new GravitationalBody(1, "Earth", 3.986e+14f, 6371000f);
        }
    }

    public static GravitationalBody Jupiter
    {
        get
        {
            return new GravitationalBody(2, "Jupiter", 1.2669e+17f, 69911000f);
        }
    }

    public static GravitationalBody Kerbin
    {
        get
        {
            return new GravitationalBody(1, "Kerbin", 3.5316e+12f, 6e+5f);
        }
    }

    public Orbit DefaultOrbit
    {
        get
        {
            double periapsisRadius = Mathd.Ceil(radius * 1.05 / 25000.0) * 25000.0; // The Ceil() call and factors of 25,000 make the default radius round up to a multiple of 25km
            double eccentricity = 0.2f;
            Angle inclination = 0f;
            Angle argumentOfPeriapsis = 0f;
            Angle longitudeOfAscendingNode = 0f;
            double timeOfPeriapsisPassage = 0f;

            return new Orbit(periapsisRadius, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, timeOfPeriapsisPassage, this);
        }
    }

    public double GravParameter
    {
        get
        {
            return gravitationalParameter;
        }
    }

    public int ID
    {
        get
        {
            return id;
        }
    }

    public string Name
    {
        get
        {
            return nameOfBody;
        }
    }

    public double Radius
    {
        get
        {
            return radius;
        }
    }

    public Orbit ZeroOrbit
    {
        get
        {
            return new Orbit(0.0, 0.0, 0.0f, 0.0f, 0.0f, 0.0, this);
        }
    }

    #endregion
}