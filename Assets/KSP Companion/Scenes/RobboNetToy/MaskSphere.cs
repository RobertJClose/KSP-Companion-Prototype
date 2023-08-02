using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class MaskSphere : MonoBehaviour
{
    float currentTime = 0f;

    public Sphere sphere;

    public ToyManager manager;
    
    // Update is called once per frame
    void Update()
    {
        if (currentTime < 0f)
            currentTime = manager.lifetime;

        float alpha = Mathf.Cos((currentTime / manager.lifetime) * 2f * Mathf.PI);
        sphere.Color = new Color(sphere.Color.r, sphere.Color.g, sphere.Color.b, alpha);

        currentTime -= Time.deltaTime;
    }
}
