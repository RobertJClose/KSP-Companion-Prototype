using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteModel : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 10.0f * Time.deltaTime, 0.0f, Space.World);
    }
}
