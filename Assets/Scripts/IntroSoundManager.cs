using UnityEngine;

public class IntroSoundManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource introAudioSource;
    public AudioClip introClip;
    
    [Tooltip("Should the sound fade in?")]
    public bool useFadeIn = true;
    public float fadeDuration = 2.0f;

    void Start()
    {
        if (introAudioSource != null && introClip != null)
        {
            introAudioSource.clip = introClip;
            
            if (useFadeIn)
            {
                StartCoroutine(FadeInSound());
            }
            else
            {
                introAudioSource.volume = 1.0f;
                introAudioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("IntroSoundManager: Missing AudioSource or Clip!");
        }
    }

    private System.Collections.IEnumerator FadeInSound()
    {
        introAudioSource.volume = 0;
        introAudioSource.Play();

        float currentTime = 0;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            introAudioSource.volume = Mathf.Lerp(0, 1, currentTime / fadeDuration);
            yield return null;
        }
        introAudioSource.volume = 1;
    }
}