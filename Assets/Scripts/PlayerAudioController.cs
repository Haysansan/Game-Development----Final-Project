using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Footstep Clips")]
    public AudioClip[] grassSteps;
    public AudioClip[] waterSteps;
    public AudioClip[] roadSteps;

    [Header("Settings")]
    public float walkInterval = 0.5f;   // footsteps when walking
    public float sprintInterval = 0.3f; // footsteps when sprinting
    public Terrain terrain;             // assign your Terrain in Inspector

    private AudioSource audioSource;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f; // 2D sound (set to 1f for 3D positional)
    }

    void Update()
    {
        float move = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));

        if (move > 0.1f) // Player is moving
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

    void PlayFootstep()
    {
        int textureIndex = GetMainTextureIndex(transform.position);
        AudioClip[] chosenArray = null;

        switch (textureIndex)
        {
            case 0: chosenArray = grassSteps; break; // Grass
            case 1: chosenArray = roadSteps; break; // Road
            case 2: chosenArray = waterSteps; break; // Water
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