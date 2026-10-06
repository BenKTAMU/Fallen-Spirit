using UnityEngine;

public class trapdoor_control : MonoBehaviour
{
    private bool isOnTrapdoor;

    public Collider2D player;

    public PlayerController playerController;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOnTrapdoor && (playerController.isGroundPounding))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider != player) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.down) > 0.9f)
            {
                isOnTrapdoor = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == player)
        {
            isOnTrapdoor = false;
        }
    }
}
