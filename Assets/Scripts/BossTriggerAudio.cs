using UnityEngine;

public class BossTriggerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private float fadeTime = 1.5f;
    
    private bool hasTriggered = false;

    // Call this function from your existing detection logic
    public void TriggerBossMusic()
    {
        if (!hasTriggered && AudioManager.instance != null)
        {
            AudioManager.instance.ChangeMusic(bossMusic, fadeTime);
            hasTriggered = true; // Prevents the music from restarting
        }
    }
}