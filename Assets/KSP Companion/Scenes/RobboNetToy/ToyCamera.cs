using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyCamera : MonoBehaviour
{
    public float rotateSpeed = 10f;
    public Transform planet;
    public Transform cam;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed, Space.Self);
        cam.transform.LookAt(planet, Vector3.forward);  
    }
}
