using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomOrbit
{
    public static Orbit Get(GravitationalBody body)
    {
        Orbit orbit = new Orbit(
            radiusOfPeriapsis: Random.Range(1.15f, 3f),
            eccentricity: Random.Range(0f, 0.1f),
            inclination: Random.Range(0f, Mathf.PI),
            argumentOfPeriapsis: Random.Range(0f, 2f * Mathf.PI),
            longitudeOfAscendingNode: Random.Range(0f, 2f * Mathf.PI),
            timeOfPeriapsisPassage: Random.Range(-1f, 1f),
            gravitationalBody: body
            );

        return orbit;
    }
}
