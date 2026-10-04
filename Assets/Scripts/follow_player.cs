using UnityEngine;

public class follow_player : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    public float smoothSpeed = 0.125f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 target_pos = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target_pos, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
}
