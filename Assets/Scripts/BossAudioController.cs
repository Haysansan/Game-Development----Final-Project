using UnityEngine;

public class BossAudioController : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Boss Sounds")]
    [SerializeField] private AudioClip roarClip;
    [SerializeField] private AudioClip lightAttackClip;
    [SerializeField] private AudioClip heavyAttackClip;
    [SerializeField] private AudioClip deathClip;

    private bool isDead = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    /// <summary>Call when boss performs a roar</summary>
    public void PlayRoar()
    {
        if (isDead) return;
        PlayOneShot(roarClip);
    }

    /// <summary>Call when boss performs a light attack</summary>
    public void PlayLightAttack()
    {
        if (isDead) return;
        PlayOneShot(lightAttackClip);
    }

    /// <summary>Call when boss performs a heavy attack</summary>
    public void PlayHeavyAttack()
    {
        if (isDead) return;
        PlayOneShot(heavyAttackClip);
    }

    /// <summary>Call when boss dies</summary>
    public void PlayDeath()
    {
        if (isDead) return;

        isDead = true;
        if (audioSource != null)
            audioSource.Stop();

        PlayOneShot(deathClip);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }
}
