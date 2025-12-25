using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image realHealthBar;
    [SerializeField] private Image emptyHealthBar;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private Image realStaminaBar;
    [SerializeField] private Image emptyStaminaBar;
    [SerializeField] private float staminaRegenDelay = 0.75f;

    public float CurrentHealth => _currentHealth;
    public float CurrentStamina => _currentStamina;

    private float _currentHealth;
    private float _currentStamina;
    private float _regenTimer;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        maxStamina = Mathf.Max(1f, maxStamina);
        _currentHealth = maxHealth;
        _currentStamina = maxStamina;

        UpdateHealthUI();
        UpdateStaminaUI();
    }

    private void Update()
    {
        if (_regenTimer > 0f)
        {
            _regenTimer -= Time.deltaTime;
        }

        RecoverStamina(20f * Time.deltaTime);

        // Only log every 30 frames so the console doesn't explode
        if (Time.frameCount % 30 == 0)
        {
            if (_currentStamina >= maxStamina)
                Debug.Log("Stamina: FULL");
            else if (_regenTimer > 0f)
                Debug.Log($"Stamina: Waiting for Timer ({_regenTimer:F2}s remaining)");
            else
                Debug.Log("Stamina: Attempting to Recover...");
        }
    }

    // Added for compatibility with boss/other systems
    public void SetMaxHealth(float value, bool clampCurrent = true)
    {
        maxHealth = Mathf.Max(1f, value);
        if (clampCurrent)
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        }
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        SetHealth(_currentHealth - amount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        SetHealth(_currentHealth + amount);
    }

    public void SetHealth(float value)
    {
        _currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        UpdateHealthUI();

        if (_currentHealth <= 0f)
        {
            // Hook for death handling (respawn, game over, etc.)
        }
    }

    public bool TryConsumeStamina(float amount)
    {
        if (_currentStamina >= amount)
        {
            _currentStamina -= amount;

            // Only reset the regen delay if it's a "big" hit (like a jump or roll)
            // If it's a small amount (running), we don't reset the delay
            if (amount > 1f)
            {
                _regenTimer = staminaRegenDelay;
            }

            UpdateStaminaUI();
            return true;
        }
        return false;
    }

    public void DeductStamina(float amount)
    {
        if (amount <= 0f) return;

        _currentStamina = Mathf.Max(0f, _currentStamina - amount);
        _regenTimer = staminaRegenDelay;
        UpdateStaminaUI();
    }

    public void RecoverStamina(float amount)
    {
        if (amount <= 0f) return;
        if (_regenTimer > 0f) return; // This waits for the 0.75s delay

        _currentStamina = Mathf.Min(maxStamina, _currentStamina + amount);
        UpdateStaminaUI();
    }

    public bool HasStamina(float amount = 0.01f)
    {
        return _currentStamina >= amount;
    }

    private void UpdateHealthUI()
    {
        if (realHealthBar != null)
        {
            realHealthBar.fillAmount = Mathf.InverseLerp(0f, maxHealth, _currentHealth);
        }

        if (emptyHealthBar != null)
        {
            emptyHealthBar.fillAmount = 1f;
        }
    }

    private void UpdateStaminaUI()
    {
        if (realStaminaBar != null)
        {
            realStaminaBar.fillAmount = Mathf.InverseLerp(0f, maxStamina, _currentStamina);
        }

        if (emptyStaminaBar != null)
        {
            emptyStaminaBar.fillAmount = 1f;
        }
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        maxStamina = Mathf.Max(1f, maxStamina);

        if (Application.isPlaying)
        {
            UpdateHealthUI();
            UpdateStaminaUI();
        }
    }
}