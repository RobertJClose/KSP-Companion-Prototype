using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalBody
{
    [SerializeField]
    private int id;
    public int ID { get { return id; } }
    [SerializeField]
    private string nameOfBody;
    public string Name { get { return nameOfBody; } }
    [SerializeField]
    private float gravitationalParameter;
    public float GravParameter { get { return gravitationalParameter; } }
    [SerializeField]
    private float radius;
    public float Radius { get { return radius; } }
    [SerializeField]
    private bool hasAtmosphere;
    public bool HasAtmosphere { get { return hasAtmosphere; } }
    [SerializeField]
    private float atmosphereHeight;
    public float AtmosphereHeight { get { return (hasAtmosphere == true) ? (atmosphereHeight) : (0f); } }

    public Orbit DefaultOrbit
    {
        get
        {
            double periapsisRadius = Mathf.Ceil((Radius + AtmosphereHeight) * 1.05f / 25000f) * 25000f; // The Ceil() call and factors of 25,000 make the default radius round up to a multiple of 25km
            double eccentricity = 0.2f;
            Angle inclination = 0f;
            Angle argumentOfPeriapsis = 0f;
            Angle longitudeOfAscendingNode = 0f;
            double timeOfPeriapsisPassage = 0f;

            return new Orbit(periapsisRadius, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, timeOfPeriapsisPassage, this);
        }
    }

    public Orbit ZeroOrbit
    {
        get
        {
            return new Orbit(0.0, 0.0, 0.0f, 0.0f, 0.0f, 0.0, this);
        }
    }

    // *************** Static Members ***************
    public static GravitationalBody Kerbin { get { return new GravitationalBody(1, "Kerbin", 3.5316e+12f, 6e+5f, true, 7e+4f); } }

    public static GravitationalBody Earth { get { return new GravitationalBody(1, "Earth", 3.986e+14f, 6371000f); } }
    public static GravitationalBody Jupiter { get { return new GravitationalBody(2, "Jupiter", 1.2669e+17f, 69911000f); } }

    public static List<GravitationalBody> ListOfBodies = new List<GravitationalBody> { Kerbin };
    public static List<string> ListOfBodyNames = new List<string> { Kerbin.Name };

    // *************** Methods ***************

    // *************** Constructors ***************

    private GravitationalBody(int bodyID, string bodyName, float bodyGravParameter, float bodyRadius, bool hasAtmos = false, float atmosHeight = 0f)
    {
        id = bodyID;
        nameOfBody = bodyName;
        gravitationalParameter = bodyGravParameter;
        radius = bodyRadius;
        hasAtmosphere = hasAtmos;
        atmosphereHeight = atmosHeight;
    }
}