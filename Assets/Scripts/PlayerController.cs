using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;
    private Rigidbody2D body;

    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float groundPoundForce = 2f;

    public PowerUps powerUps;

    private bool isGrounded = true;
    public bool isGroundPounding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Debug.Log("Awake");
    }

    // Update is called once per frame
    void Update()
    {
        body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, body.linearVelocity.y);

        if (body.linearVelocityY < 0)
        {
            body.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (body.linearVelocityY > 0 && !Input.GetButton("Jump"))
        {
            body.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.S) && powerUps.hasBoots)
        {
            body.linearVelocity = Vector2.down * Physics2D.gravity.y * (groundPoundForce);
            isGroundPounding = true;
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
            isGroundPounding = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground") isGrounded = false;
    }
}
