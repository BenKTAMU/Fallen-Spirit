using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class torch_flash : MonoBehaviour
{
    public float rate = 1;
    public float amplitude = 1.2f;
    public float offset = 1.8f;

    private float last_update = 0;

    private float phase_offset;
    private float offset_offset;

    private Light2D light;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        phase_offset = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        offset_offset = UnityEngine.Random.Range(0, 0.5f);

        light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.fixedTime - last_update > 0.1f) {
            last_update = Time.fixedTime;
            light.intensity = (offset + offset_offset) + amplitude * (float)Math.Abs(Math.Sin(rate * Time.fixedTime + phase_offset));
        }
    }
}
