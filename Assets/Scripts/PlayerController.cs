using System;
using TMPro;
using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class PlayerController : MonoBehaviour
{
    public BossController boss;
    public follow_player player_follower;

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;

    [SerializeField] private float groundAcceleration = 60f;
    [SerializeField] private float groundDeceleration = 90f;

    [SerializeField] private float airAcceleration = 20f;
    [SerializeField] private float airDeceleration = 10f;

    [SerializeField] private float airSpeed = 4f;

    [SerializeField] private float maxFallSpeed;

    private Rigidbody2D body;


    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float groundPoundForce = -2f;

    [SerializeField] private GameObject respawnPoint;

    public GameObject forceField;

    public PowerUps powerUps;

    private bool isGrounded = true;
    public bool isGroundPounding = false;

    private int playerLayer;
    private int playerShieldedLayer;

    private Animator anim;
    private SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        playerShieldedLayer = LayerMask.NameToLayer("PlayerShielded");
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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

        float targetSpeed = isGrounded ? Input.GetAxis("Horizontal") * moveSpeed : Input.GetAxis("Horizontal") * airSpeed;
        float acceleration;
        float newXVelocity;
        if (isGrounded)
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.0f ? groundAcceleration : groundDeceleration;
            newXVelocity = Mathf.MoveTowards(body.linearVelocityX, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            acceleration = Mathf.Abs(airSpeed) > 0.0f ? airAcceleration : airDeceleration;
            newXVelocity = Mathf.MoveTowards(body.linearVelocityX, targetSpeed, acceleration * Time.fixedDeltaTime);
        }


        body.linearVelocity = new Vector2(newXVelocity, body.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(newXVelocity));
        anim.SetBool("IsGrounded", isGrounded);
        

        if (newXVelocity > 0)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }

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
        
        anim.SetFloat("yVelocity", body.linearVelocityY);

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
        Collider2D collision = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (collision != null)
        {
            Vector2 origin = groundCheck.position;
            Vector2 collisionPoint = collision.ClosestPoint(origin);

            Vector2 collisionVector = (collisionPoint - origin).normalized;

            if (Vector2.Dot(collisionVector, Vector2.down) > 0.9f) isGrounded = true;
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }

        if (body.linearVelocityY < 0 && !isGroundPounding)
        {
            body.linearVelocityY = -Math.Min(Math.Abs(body.linearVelocityY), Math.Abs(maxFallSpeed));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" && isGroundPounding)
        {
            StartCoroutine(StopGroundPounding());
        }

        if (collision.gameObject.tag == "fireball" && !forceField.activeSelf)
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
                BossController bc = collision.gameObject.GetComponent<BossController>();

                if (bc.boss_shield.activeSelf)
                {
                    Die();
                }
                else
                {
                    bc.Hit();
                    isGrounded = true;
                }

            }
            else
            {
                Die();
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
        else if (other.gameObject.name == "attack1")
        {
            other.gameObject.SetActive(false);
            Die();
        }
        if (other.gameObject.tag == "saw")
        {
            Die();
        }
    }


    IEnumerator StopGroundPounding()
    {
        yield return new WaitForSeconds(0.5f);
        isGroundPounding = false;
    }
}
