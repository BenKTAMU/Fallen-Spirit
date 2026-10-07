using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class lava_flash : MonoBehaviour
{
    private Light2D light;
    private float last_update = 0;
    
    public float radius_offset = 3;
    public float radius_amplitude = 2;
    public float rate = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.fixedTime - last_update > 0.1f) {
            light.pointLightOuterRadius = radius_offset + radius_amplitude * Mathf.Sin(rate * Time.fixedTime);
        }
    }
}
