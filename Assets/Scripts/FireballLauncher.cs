using System.Collections;
using UnityEngine;

public class FireballLauncher : MonoBehaviour
{
    public GameObject fireballPrefab;

    public Transform firePoint;

    public float fireRate = 1.5f;

    public float nextFireTime = 0f;

    public float fireballLifetime = 5f;

    public AudioClip fireball_soundeffect;
    AudioSource audio_source;

    [SerializeField] private GameObject toDestroy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
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
        toDestroy = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        StartCoroutine(DestroyFireball(toDestroy));
        audio_source.PlayOneShot(fireball_soundeffect);
    }

    IEnumerator DestroyFireball(GameObject toEliminate)
    {
        yield return new WaitForSeconds(fireballLifetime);
        Destroy(toEliminate);
    }
}
