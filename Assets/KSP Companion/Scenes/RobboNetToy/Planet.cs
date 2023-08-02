using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public GravitationalBody body;

    private void Awake()
    {
        body = new GravitationalBody("Planet", 1, 1, 1, 1, new Orbit(0, 0, 0, 0, 0, 0, null));
    }
}
