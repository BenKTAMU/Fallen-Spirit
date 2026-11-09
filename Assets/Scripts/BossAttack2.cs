using UnityEngine;
using System.Collections;

public class BossAttack2 : MonoBehaviour
{

    private Rigidbody2D body;

    public float speed = 5f;
    public float delayTime = 1f;

    public GameObject explosionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(SpawnAndDelay());
    }

    IEnumerator SpawnAndDelay()
    {
        yield return new WaitForSeconds(delayTime);
        body.linearVelocity = -transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
