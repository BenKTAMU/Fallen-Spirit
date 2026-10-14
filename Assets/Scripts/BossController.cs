using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class range_check : MonoBehaviour
{
    public GameObject player;
    public float min_distance = 3f;
    public float shield_activate_delay = 1f;
    public float shield_poweron_delay = 0.1f;
    public float attack_start_delay = 5f;
    public float attack_cooldown_delay = 5f;

    public GameObject boss_shield;

    public Light2D attack_flash_light;

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
        if (((player.transform.position - gameObject.transform.position).magnitude < min_distance) && !started)
        {
            started = true;
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Rigidbody2D>().gravityScale = 4;
            StartCoroutine(ActivateShield());
            StartCoroutine(DoAttack());
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
        for (int i = 0; i < 20; i++)
        {
            attack_flash_light.intensity = i * 0.5f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 20; i++)
        {
            attack_flash_light.intensity = 10 - i * 0.5f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
