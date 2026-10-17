using System;
using UnityEngine;

public class Saw : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float spinSpeed = 200f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = spinSpeed;
    }
}
