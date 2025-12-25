using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;

    private void Awake()
    {
        // Singleton setup: Keeps this object alive between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this to change music smoothly
    public void ChangeMusic(AudioClip newClip, float fadeDuration = 1.5f)
    {
        if (musicSource.clip == newClip) return; // Already playing
        StartCoroutine(FadeTrack(newClip, fadeDuration));
    }

    private IEnumerator FadeTrack(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }
        musicSource.volume = startVolume;
    }
}