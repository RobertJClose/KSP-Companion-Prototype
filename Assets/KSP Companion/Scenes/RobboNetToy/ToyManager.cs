using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ToyManager : MonoBehaviour
{
    private float currentTime = 0;

    public float lifetime = 15f;

    public OrbitLine[] lines;

    private void Awake()
    {
        lines = GetComponentsInChildren<OrbitLine>();
    }

    private void Update()
    {
        if (currentTime < 0)
        {
            foreach(var line in lines)
            {
                line.SetOrbit();
            }

            currentTime = lifetime;
        }

        currentTime -= Time.deltaTime;
    }
}
