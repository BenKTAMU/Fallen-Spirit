using UnityEngine;

public class lava_touch : MonoBehaviour
{
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject != player) return;

        player.GetComponent<PlayerController>().Die();
    }
}
