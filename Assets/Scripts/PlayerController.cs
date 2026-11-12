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

    public AudioClip item_pickup_audio;
    public AudioClip jump_audio;
    public AudioClip land_audio;
    public AudioClip shield_up_audio;
    public AudioClip walking_audio;
    private Rigidbody2D body;
    private AudioSource audio_source;
    public AudioSource shield_audio_source;
    private BoxCollider2D playerCollider;

    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float groundPoundForce = -2f;

    [SerializeField] private GameObject respawnPoint;

    public GameObject forceField;

    public PowerUps powerUps;

    private bool played_land_audio = false;
    [SerializeField] private bool isGrounded = true;
    public bool isGroundPounding = false;

    private int playerLayer;
    private int playerShieldedLayer;

    private Animator anim;
    private SpriteRenderer sr;

    private Transform wallCheck;
    private float wallCheckDistance = 0.2f;

    public float shieldDuaration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        playerLayer = LayerMask.NameToLayer("Player");
        playerShieldedLayer = LayerMask.NameToLayer("PlayerShielded");
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        audio_source = GetComponent<AudioSource>();
    }

    public void Die()
    {
        GameObject.Find("Main Camera").GetComponent<BackgroundAudioPlayer>().PlayDieSound();
        Debug.Log("Game Over");
        player_follower.follow_boss = false;
        transform.position = respawnPoint.transform.position;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (!isGrounded && isTouchingWall(horizontalInput))
        {
            horizontalInput = 0f;
        }

        float targetSpeed = isGrounded ? horizontalInput * moveSpeed : horizontalInput * airSpeed;
        float acceleration;
        float newXVelocity;
        if (isGrounded)
        {
            if (Math.Abs(horizontalInput) > 0.1f)
            {
                if (!audio_source.isPlaying)
                {
                    audio_source.PlayOneShot(walking_audio);
                }
            }
            else
            {
                audio_source.Stop();
            }
            played_land_audio = false;
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
        anim.SetBool("IsGroundPounding", isGroundPounding);

        Vector2 origin = new Vector2(
            playerCollider.bounds.center.x,
            playerCollider.bounds.min.y
        );

        float distance_to_ground = 2.0f;
        RaycastHit2D hit = Physics2D.Raycast(
            origin, Vector2.down,
            distance_to_ground,
            groundLayer
        );

        bool near_ground = hit.collider != null;

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
            audio_source.Stop();
            audio_source.PlayOneShot(jump_audio);
            isGrounded = false;
        }

        if (near_ground && body.linearVelocityY < 0)
        {
            if (!played_land_audio)
            {
                played_land_audio = true;
                audio_source.PlayOneShot(land_audio);
            }
            anim.SetFloat("yVelocity", 0);
        }
        else anim.SetFloat("yVelocity", body.linearVelocityY);

        if (Input.GetKeyDown(KeyCode.S) && powerUps.hasBoots && !isGrounded)
        {
            body.linearVelocity = Vector2.down * Physics2D.gravity.y * (groundPoundForce);
            isGroundPounding = true;
        }

        if (Input.GetMouseButtonDown(0) && powerUps.hasArmor)
        {
            // gameObject.layer = playerShieldedLayer;
            // forceField.SetActive(true);
            StartCoroutine(CastShield());
        }

        if (Input.GetMouseButtonUp(0))
        {
            // gameObject.layer = playerLayer;
            // forceField.SetActive(false);
        }




    }

    private void FixedUpdate()
    {
        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (groundedNow)
        {
            if (!isGrounded && body.linearVelocityY <= 0.1f)
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" && isGroundPounding && body.linearVelocityY <= 0)
        {
            StartCoroutine(StopGroundPounding());
        }

        if (collision.gameObject.tag == "fireball" && !forceField.activeSelf)
        {
            Die();
        }

        if (collision.gameObject.tag == "Attack" && !forceField.activeSelf)
        {
            Die();
        }

        if (collision.gameObject.tag == "enemy")
        {
            Debug.Log("Hit Enemy");
        }

        if (collision.gameObject.tag == "enemy" && collision.gameObject.GetComponentInParent<BossController>().shield_active == false)
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

            if (isGroundPounding)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x + (jumpForce * 0.5f), jumpForce * 3f);
                Debug.Log("Pounded enemy");
                BossController bc = collision.gameObject.GetComponent<BossController>();
                StartCoroutine(StopGroundPounding());
                bc.Hit();

                // if (bc.boss_shield.activeSelf)
                // {
                //     Die();
                // }
                // else
                // {
                //     bc.Hit();
                //     isGrounded = true;
                //     if (!played_land_audio) {
                //         audio_source.PlayOneShot(land_audio);
                //         played_land_audio = true;
                //     }
                // }

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

    private bool isTouchingWall(float direction)
    {
        if (Mathf.Abs(direction) < 0.01f)
        {
            return false;
        }

        Vector2 rayDirection;
        if (direction > 0)
        {
            rayDirection = Vector2.right;
        }
        else
        {
            rayDirection = Vector2.left;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }


    IEnumerator CastShield()
    {
        gameObject.layer = playerShieldedLayer;
        forceField.SetActive(true);
        shield_audio_source.PlayOneShot(shield_up_audio);

        yield return new WaitForSeconds(shieldDuaration);
        gameObject.layer = playerLayer;
        forceField.SetActive(false);
    }


    IEnumerator StopGroundPounding()
    {
        yield return new WaitForSeconds(0.5f);
        isGroundPounding = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
