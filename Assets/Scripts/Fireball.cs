using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField] private float fireballSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 15f); 
    }

    // Update is called once per frame
    void Update()
    {
        body.linearVelocity = transform.right * fireballSpeed;
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.collider.tag);
        if (other.collider.CompareTag("forcefield"))
        {
            Debug.Log("Deflected");
            Vector2 newDir = -transform.right;
            transform.right = newDir;
            transform.position += (UnityEngine.Vector3)(newDir * 0.10f);
            body.linearVelocity = newDir * fireballSpeed;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
