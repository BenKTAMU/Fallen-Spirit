using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class BossAttack : MonoBehaviour
{
    private Rigidbody2D body;
    private Collider2D attackCollider;
    private SpriteRenderer sr;
    public Light2D attackLight;

    public GameObject explosionPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        attackCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        attackLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("forcefield"))
        {
            Debug.Log("Deflected");
            Vector2 newDir = -transform.right;
            transform.right = newDir;
            transform.position += (UnityEngine.Vector3)(newDir * 0.10f);
            body.linearVelocity = newDir * body.linearVelocity.magnitude * 3f;
            attackCollider.excludeLayers = LayerMask.GetMask("PlayerShielded", "Player");
            sr.color = new Color(0.5f, 0f, 0.5f);
            attackLight.enabled = true;

        }
        else
        {
            if (other.collider.CompareTag("enemy"))
            {
                Debug.Log("Hit Boss");
                other.collider.GetComponentInParent<BossController>().ShieldHit();

                if(explosionPrefab != null)
                {
                    Vector2 contactPoint = other.contacts.Length > 0 ? other.contacts[0].point : (Vector2)transform.position;
                    GameObject explosion = Instantiate(explosionPrefab, contactPoint, Quaternion.identity);
                    Destroy(explosion, 0.9f); 
                }
            }
            Destroy(gameObject);
        }
        
    }
}
