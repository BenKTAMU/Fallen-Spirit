using System.Collections;
using UnityEngine;

public class BackgroundAudioPlayer : MonoBehaviour
{
    public AudioClip intro_clip;
    public AudioClip repeat_clip;

    public AudioSource player;

    public bool play_audio = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.PlayOneShot(intro_clip);

        StartCoroutine(PlayMain());
    }

    IEnumerator PlayMain()
    {
        yield return new WaitForSeconds(intro_clip.length);

        while (play_audio)
        {
            player.clip = repeat_clip;
            player.time = 0f;
            player.Play();
            yield return new WaitForSeconds(repeat_clip.length);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
