using System;
using TMPro;
using UnityEngine;
using System.Collections;

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
    
    [SerializeField] private GameObject respawnPoint;

    public GameObject forceField;

    public PowerUps powerUps;

    private bool isGrounded = true;
    public bool isGroundPounding = false;

    private int playerLayer;
    private int playerShieldedLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        playerShieldedLayer = LayerMask.NameToLayer("PlayerShielded");
    }

    public void Die()
    {
        Debug.Log("Game Over");
        transform.position = respawnPoint.transform.position;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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

        if (Input.GetKeyDown(KeyCode.S) && powerUps.hasBoots && !isGrounded)
        {
            body.linearVelocity = Vector2.down * Physics2D.gravity.y * (groundPoundForce);
            isGroundPounding = true;
        }

        if (Input.GetMouseButtonDown(0) && powerUps.hasArmor)
        {
            gameObject.layer = playerShieldedLayer;
            forceField.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            gameObject.layer = playerLayer;
            forceField.SetActive(false);
        }



    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" && isGroundPounding)
        {
            StartCoroutine(StopGroundPounding());
        }
        if(collision.gameObject.tag == "fireball"){
            Die();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "checkpoint")
        {
            Debug.Log("Checkpoint reached");
            respawnPoint = other.gameObject;
        }
    }
    

    IEnumerator StopGroundPounding()
    {
        yield return new WaitForSeconds(0.5f);
        isGroundPounding = false;
    }
}
