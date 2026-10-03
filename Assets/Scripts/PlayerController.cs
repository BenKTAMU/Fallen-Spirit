using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;
    private Rigidbody2D body;

    private bool isJumping;
    private float holdTimer;
    [SerializeField] private float maxHoldTime = 0.8f;
    [SerializeField] private float jumpForce = 4f;
    
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
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        Debug.Log(isGrounded);
        body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, body.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            holdTimer = 0f;
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        }
        

        if (Input.GetButtonUp("Jump") && body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.5f);
            isJumping = false;
        }
        
        
    }
}
