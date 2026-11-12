using UnityEngine;

public class player_tracking : MonoBehaviour
{
    public follow_player camera_tracking;
    public BossController boss_controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "boss_spawn_point")
        {
            camera_tracking.follow_boss = true;
            boss_controller.SpawnBoss();
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.name == "boss_spawn_point")
        {
            camera_tracking.follow_boss = false;
        }
    }
}
