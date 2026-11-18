using UnityEngine;

public class trapdoor_control : MonoBehaviour
{
    private bool isOnTrapdoor;

    public Collider2D player;

    public PlayerController playerController;

    private AudioSource audio_source;
    public AudioClip ground_break;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio_source = playerController.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnTrapdoor && (playerController.isGroundPounding))
        {
            Debug.Log("Trapdoor Deactivated");
            gameObject.SetActive(false);
            audio_source.PlayOneShot(ground_break);
            Debug.Log("Play ground break");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider != player) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.down) > 0.9f)
            {

                Debug.Log("isOnTrapdoor");
                isOnTrapdoor = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == player)
        {
            Debug.Log("is not OnTrapdoor");
            isOnTrapdoor = false;
        }
    }
}
