using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class OrbitLine : MonoBehaviour
{
    Orbit orbit;
    Polyline polyline;

    public Planet planet;
    public Transform satellite;

    private void Awake()
    {
        polyline = GetComponent<Polyline>();
    }

    private void Update()
    {
        if (orbit != null)
            satellite.position = (Vector3)orbit.Time2Point(Time.time);
    }

    public void SetOrbit()
    {
        orbit = RandomOrbit.Get(planet.body);
        SetOrbit(orbit);
    }

    public void SetOrbit(Orbit orbit)
    {
        polyline.points = new List<PolylinePoint>();
        polyline.meshOutOfDate = true;

        (List<Vector3d> points, _) = orbit.OrbitalPoints(null, null, 0.1f);

        foreach (Vector3d point in points)
        {
            polyline.AddPoint((Vector3)point);
        }
    }    
}
