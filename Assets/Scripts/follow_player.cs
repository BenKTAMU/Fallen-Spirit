using UnityEngine;

public class follow_player : MonoBehaviour
{
    public Transform player;
    public Transform boss;
    public Vector3 player_offset;
    public Vector3 boss_offset;

    public bool follow_boss = false;

    public float smoothSpeed = 0.125f;

    private Camera camera;
    private float zoomVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null && boss != null)
        {
            Vector3 target_pos;

            if (follow_boss)
            {
                camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, 10, ref zoomVelocity, smoothSpeed / 2);
                target_pos = new Vector3(boss.position.x + boss_offset.x, boss.position.y + boss_offset.y, -10);
            }
            else
            {
                camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, 8, ref zoomVelocity, smoothSpeed / 2);
                target_pos = new Vector3(player.position.x + player_offset.x, player.position.y + player_offset.y, -10);
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target_pos, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
}
