using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Still needed

public class gameflow : MonoBehaviour
{
    // ====== HEALTH ======
    public static float currentHP = 500f;
    public static float maxHP = 500f;

    // ====== STAMINA ======
    public static float currentStam = 200f;
    public static float maxStam = 200f;

    // ====== UI - Changed to Image type to use fillAmount ======
    public Image healthImage;
    public Image staminaImage;

    // ====== INPUT ======
    public KeyCode runForward = KeyCode.LeftShift;

    // ====== STAMINA SETTINGS ======
    public float staminaDrain = 5f;
    public float staminaRegen = 20f;
    public float drainInterval = 0.5f;
    private float timeCheck = 0f;

    void Start()
    {
        // Check if images are assigned in the Inspector
        if (healthImage != null)
            healthImage.color = Color.red;

        if (staminaImage != null)
            staminaImage.color = Color.green;

        // No need to cache components if public Image variables are assigned in the Inspector
    }

    void Update()
    {
        HandleStamina();
        UpdateBars();
    }

    // ================= STAMINA LOGIC =================
    void HandleStamina()
    {
        if (Input.GetKey(runForward) && currentStam > 0)
        {
            timeCheck += Time.deltaTime;

            if (timeCheck >= drainInterval)
            {
                timeCheck = 0f;
                currentStam -= staminaDrain;
            }
        }
        else
        {
            // Reset timeCheck here is fine if you want drain to restart immediately
            timeCheck = 0f;

            if (currentStam < maxStam)
                currentStam += staminaRegen * Time.deltaTime;
        }

        currentStam = Mathf.Clamp(currentStam, 0, maxStam);
    }

    // ================= BAR FILL (More efficient) =================
    void UpdateBars()
    {
        // Use fillAmount for standard UI bars
        healthImage.fillAmount = currentHP / maxHP;
        staminaImage.fillAmount = currentStam / maxStam;
    }
}