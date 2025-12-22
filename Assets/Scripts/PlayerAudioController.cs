using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Footstep Clips")]
    public AudioClip[] grassSteps;
    public AudioClip[] waterSteps;
    public AudioClip[] roadSteps;

    [Header("Action Clips")]
    public AudioClip swordDrawClip;
    public AudioClip swordSheathClip;

    [Header("Jump Clips")]
    public AudioClip jumpClip1;
    public AudioClip jumpClip2;

    [Header("Settings")]
    public float walkInterval = 0.5f;
    public float sprintInterval = 0.3f;
    public Terrain terrain;

    private AudioSource audioSource;
    private float stepTimer;
    private bool swordDrawn = false;
    private bool useFirstJump = true; // toggle between jump sounds

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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
                PlayFootstep();
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
            if (!swordDrawn)
            {
                if (swordDrawClip != null)
                    audioSource.PlayOneShot(swordDrawClip);
                swordDrawn = true;
            }
            else
            {
                if (swordSheathClip != null)
                    audioSource.PlayOneShot(swordSheathClip);
                swordDrawn = false;
            }
        }
    }

    void HandleJumpAudio()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // jump key
        {
            if (useFirstJump && jumpClip1 != null)
            {
                audioSource.PlayOneShot(jumpClip1);
            }
            else if (!useFirstJump && jumpClip2 != null)
            {
                audioSource.PlayOneShot(jumpClip2);
            }

            // flip toggle for next jump
            useFirstJump = !useFirstJump;
        }
    }

    void PlayFootstep()
    {
        int textureIndex = GetMainTextureIndex(transform.position);
        AudioClip[] chosenArray = null;

        switch (textureIndex)
        {
            case 0: chosenArray = grassSteps; break;
            case 1: chosenArray = waterSteps; break;
            case 2: chosenArray = roadSteps; break;
            default: chosenArray = grassSteps; break;
        }

        if (chosenArray != null && chosenArray.Length > 0)
        {
            int index = Random.Range(0, chosenArray.Length);
            audioSource.PlayOneShot(chosenArray[index]);
        }
    }

    int GetMainTextureIndex(Vector3 worldPos)
    {
        Vector3 terrainPos = worldPos - terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;

        int mapX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int maxIndex = 0;
        float maxMix = 0;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxMix = splatmapData[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}