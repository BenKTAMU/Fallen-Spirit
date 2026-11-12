using System.Collections;
using System.Threading;
using UnityEngine;

public class BackgroundAudioPlayer : MonoBehaviour
{
    public AudioClip intro_clip;
    public AudioClip repeat_clip;

    public AudioClip die_clip;

    public AudioSource player;
    public AudioSource die_player;

    private int death_clips_playing = 0;

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

        player.loop = true;
        player.Play();

        die_player.clip = die_clip;
    }

    public void PlayDieSound()
    {
        StartCoroutine(PlayDieSoundAsync());
    }

    IEnumerator PlayDieSoundAsync()
    {
        Interlocked.Increment(ref death_clips_playing);
        try
        {
            if (Volatile.Read(ref death_clips_playing) == 1)
                player.Pause();
            die_player.PlayOneShot(die_clip);
            yield return new WaitForSeconds(die_clip.length);
            if (Volatile.Read(ref death_clips_playing) == 1)
            {
                die_player.Stop();
                player.volume = 0;
                player.UnPause();
                for (int i = 1; i <= 20; i++)
                {
                    player.volume = i * 0.05f;
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        finally
        {
            Interlocked.Decrement(ref death_clips_playing);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
