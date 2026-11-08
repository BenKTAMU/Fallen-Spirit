using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossController : MonoBehaviour
{
    public GameObject player;
    public float min_distance = 3f;
    public float shield_activate_delay = 1f;
    public float shield_poweron_delay = 0.1f;
    public float attack_start_delay = 2f;
    public float attack_cooldown_delay = 3f;

    public int num_phase_1_attacks = 5;

    public int attack_phase = 0;
    public bool has_been_hit = false;

    public float attack1_speed = 1f;

    public GameObject boss_shield;
    public bool shield_active = false;

    public Light2D attack_flash_light;
    public GameObject attack1;
    public GameObject attack1_prefab;

    public Transform attack2_launch_point;
    public GameObject attack2_prefab;

    private bool started = false;

    private Coroutine attack1Coroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        boss_shield.SetActive(false);
        attack_flash_light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnBoss()
    {
        if (started) return;

        started = true;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Rigidbody2D>().gravityScale = 4;
        StartCoroutine(ActivateShield());
        attack1Coroutine = StartCoroutine(DoAttack1());
    }

    public void Hit()
    {
        has_been_hit = true;
        StartCoroutine(ActivateShield());
        attack_phase++;
        if (attack_phase == 1) StartCoroutine(DoAttack2(5));
    }

    public void ShieldHit()
    {
        Debug.Log("Shield Hit");
        StartCoroutine(DeactivateShield());
        if(attack_phase == 0)
        {
            StopCoroutine(attack1Coroutine);
            attack1Coroutine = null;
        }
    }

    IEnumerator ActivateShield()
    {
        yield return new WaitForSeconds(shield_activate_delay);
        boss_shield.SetActive(true);
        boss_shield.transform.localScale = new Vector2(0.0f, 0.0f);

        for (int i = 1; i <= 11; i++) // scale to 1.1
        {
            boss_shield.transform.localScale = new Vector2(0.1f * i, 0.1f * i);
            yield return new WaitForSeconds(shield_poweron_delay);
        }
        shield_active = true;
    }

    IEnumerator DeactivateShield()
    {
        for (int i = 11; i >= 0; i--) // scale to 1.1
        {
            boss_shield.transform.localScale = new Vector2(0.1f * i, 0.1f * i);
            yield return new WaitForSeconds(shield_poweron_delay);
        }

        boss_shield.SetActive(false);
        shield_active = false;
    }

    IEnumerator DoAttack2(float delay_increase = 0f)
    {
        yield return new WaitForSeconds(attack_start_delay + delay_increase);
        for (int i = 0; i < 10; i++)
        {
            GameObject attack2 = Instantiate(attack2_prefab, attack2_launch_point.position, attack2_launch_point.rotation);

            Vector2 to_player = (player.transform.position - transform.position).normalized;
            attack2.GetComponent<Rigidbody2D>().linearVelocity = to_player;
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator DoAttack1()
    {
        yield return new WaitForSeconds(attack_start_delay);

        for (int i = 0; i < num_phase_1_attacks; i++)
        {
            GameObject attack1 = Instantiate(attack1_prefab, attack2_launch_point.position, attack2_launch_point.rotation);
            attack1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * attack1_speed;
            yield return new WaitForSeconds(attack_cooldown_delay);
        }
    }

    void LaunchAttack1(bool deactivate_shield)
    {
        Debug.Log("Boss attack");
        StartCoroutine(MakeFlash(deactivate_shield));
        DoAttack1();
    }

    IEnumerator MakeFlash(bool deactivate_shield)
    {
        float x = attack1.transform.position.x;
        float y = attack1.transform.position.y;

        for (int i = 0; i < 20; i++)
        {
            attack_flash_light.intensity = i * 0.5f;
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 20; i++)
        {
            attack_flash_light.intensity = 10 - i * 0.5f;
            yield return new WaitForSeconds(0.02f);
        }

        // attack1.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2);
        // attack1.SetActive(true);
        // Vector2 velocity = attack1_speed * (player.transform.position - attack1.transform.position).normalized;
        // attack1.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(velocity.x, Math.Max(0, velocity.y));

        yield return new WaitForSeconds(2f);

        // attack1.SetActive(false);
        // attack1.transform.position = new Vector2(x, attack1.transform.position.y);

    }

    IEnumerator RelaunchAttack1()
    {
        yield return new WaitForSeconds(3);

        if (attack_phase == 0 && !has_been_hit)
        {
            StartCoroutine(ActivateShield());
            StartCoroutine(DoAttack1());
        }
    }
}
