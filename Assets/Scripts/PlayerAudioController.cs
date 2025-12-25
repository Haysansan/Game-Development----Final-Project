using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Movement Sound")]
    public AudioClip footstepSound;
    [Range(0f, 1f)] public float footstepVolume = 0.3f;

    [Header("Action Clips")]
    public AudioClip swordDrawClip;
    public AudioClip swordSheathClip;
    [Range(0f, 1f)] public float actionVolume = 0.8f;

    [Header("Attack Clips")]
    public AudioClip attackClip1;
    public AudioClip attackClip2;

    [Header("Jump Clips")]
    public AudioClip jumpClip1;
    public AudioClip jumpClip2;

    [Header("Settings")]
    public float walkInterval = 0.5f;
    public float sprintInterval = 0.3f;

    private AudioSource movementSource;
    private AudioSource actionSource;

    private float stepTimer;
    private bool swordDrawn = false;
    private bool useFirstJump = true;
    private bool useFirstAttack = true;

    void Start()
    {
        movementSource = gameObject.AddComponent<AudioSource>();
        actionSource = gameObject.AddComponent<AudioSource>();

        movementSource.spatialBlend = 1.0f;
        actionSource.spatialBlend = 1.0f;
    }

    void Update()
    {
        HandleMovementAudio();
        HandleSwordAudio();
        HandleJumpAudio();
        HandleAttackAudio();
    }

    void HandleMovementAudio()
    {
        float move = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));

        if (move > 0.1f)
        {
            float currentInterval = Input.GetKey(KeyCode.LeftShift) ? sprintInterval : walkInterval;

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                if (footstepSound != null)
                {
                    movementSource.pitch = Random.Range(0.9f, 1.1f);
                    movementSource.PlayOneShot(footstepSound, footstepVolume);
                }
                stepTimer = currentInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void HandleSwordAudio()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioClip clip = swordDrawn ? swordSheathClip : swordDrawClip;
            if (clip != null) actionSource.PlayOneShot(clip, actionVolume);
            swordDrawn = !swordDrawn;
        }
    }

    void HandleJumpAudio()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioClip clip = useFirstJump ? jumpClip1 : jumpClip2;
            if (clip != null) actionSource.PlayOneShot(clip, actionVolume);
            useFirstJump = !useFirstJump;
        }
    }

    void HandleAttackAudio()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            AudioClip clip = useFirstAttack ? attackClip1 : attackClip2;
            if (clip != null) actionSource.PlayOneShot(clip, actionVolume);
            useFirstAttack = !useFirstAttack;
        }
    }
}
