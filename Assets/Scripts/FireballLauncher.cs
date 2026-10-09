using UnityEngine;

public class FireballLauncher : MonoBehaviour
{
    public GameObject fireballPrefab;

    public Transform firePoint;

    public float fireRate = 1.5f;

    public float nextFireTime = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
    }
}
