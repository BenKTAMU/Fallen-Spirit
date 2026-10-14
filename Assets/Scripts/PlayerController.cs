using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public BossController boss;
    public follow_player player_follower;

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
        if (Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer))
        {
            isGrounded = true;
            StartCoroutine(StopGroundPounding());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" && isGroundPounding)
        {
            StartCoroutine(StopGroundPounding());
        }
        else if (collision.gameObject.tag == "fireball")
        {
            Die();
        }
        else if (collision.gameObject.tag == "enemy")
        {
            bool on_top = false;

            foreach (ContactPoint2D point in collision.contacts)
            {
                if (Vector2.Dot(Vector2.up, point.normal) > 0.4f)
                {
                    on_top = true;
                    break;
                }
            }

            if (on_top && isGroundPounding)
            {
                Debug.Log("Pounded enemy");
                isGrounded = true;
            }
            else
            {
                Debug.Log("Game Over (touched enemy)");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "checkpoint")
        {
            Debug.Log("Checkpoint reached");
            respawnPoint = other.gameObject;
        }
        else if (other.gameObject.name == "boss_spawn_point")
        {
            boss.SpawnBoss();

            float relativeX = other.bounds.center.x - transform.position.x;
            bool to_the_left = relativeX < 0f;

            if (to_the_left) player_follower.follow_boss = false;
            else player_follower.follow_boss = true;
        }
    }


    IEnumerator StopGroundPounding()
    {
        yield return new WaitForSeconds(0.5f);
        isGroundPounding = false;
    }
}
