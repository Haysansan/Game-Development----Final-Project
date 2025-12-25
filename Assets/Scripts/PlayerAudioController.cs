using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Movement Sound")]
    public AudioClip footstepSound;
    [Range(0f, 1f)] public float footstepVolume = 0.3f; // Lower this for quieter steps

    [Header("Action Clips")]
    public AudioClip swordDrawClip;
    public AudioClip swordSheathClip;
    [Range(0f, 1f)] public float actionVolume = 0.8f;

    [Header("Jump Clips")]
    public AudioClip jumpClip1;
    public AudioClip jumpClip2;

    [Header("Settings")]
    public float walkInterval = 0.5f;
    public float sprintInterval = 0.3f;

    // We will use two sources so footsteps can be quieter than jumps
    private AudioSource movementSource;
    private AudioSource actionSource;
    
    private float stepTimer;
    private bool swordDrawn = false;
    private bool useFirstJump = true;

    void Start()
    {
        // This part creates the Audio Sources for you if they don't exist
        movementSource = gameObject.AddComponent<AudioSource>();
        actionSource = gameObject.AddComponent<AudioSource>();

        // Optional: Set spatial blend to 1.0 if you want 3D sound (quieter far away)
        movementSource.spatialBlend = 1.0f;
        actionSource.spatialBlend = 1.0f;
    }

    void Update()
    {
        HandleMovementAudio();
        HandleSwordAudio();
        HandleJumpAudio();
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
                    // Randomize pitch slightly so it sounds more natural
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
}