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
    public float attack_start_delay = 5f;
    public float attack_cooldown_delay = 5f;

    public float attack1_speed = 0.25f;

    public GameObject boss_shield;

    public Light2D attack_flash_light;
    public GameObject attack1;

    private bool started = false;

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
        StartCoroutine(DoAttack());
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
    }

    IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(attack_start_delay);
        LaunchAttack();

        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(attack_cooldown_delay);
            LaunchAttack();
        }
    }

    void LaunchAttack()
    {
        Debug.Log("Boss attack");
        StartCoroutine(MakeFlash());
    }

    IEnumerator MakeFlash()
    {
        float x = attack1.transform.position.x;
        float y = attack1.transform.position.y;

        float direction = -1;
        if (player.transform.position.x > x) direction = 1;

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

        attack1.SetActive(true);
        for (int i = 0; i < 30; i++)
        {
            attack1.transform.position = new Vector2(x + direction * i * attack1_speed, y);
            yield return new WaitForSeconds(0.05f);
        }

        attack1.SetActive(false);
        attack1.transform.position = new Vector2(x, attack1.transform.position.y);
    }
}
