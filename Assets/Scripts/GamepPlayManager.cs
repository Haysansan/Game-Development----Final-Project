using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider healthSlider;
    public Slider staminaSlider;

    [Header("Settings")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float lerpSpeed = 6f;

    private float _health;
    private float _stamina;

    private void Start()
    {
        _health = maxHealth;
        _stamina = maxStamina;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _health;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = _stamina;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10f);
        }

        if (healthSlider != null && healthSlider.value != _health)
        {
            healthSlider.value = _health;
        }

        if (staminaSlider != null && staminaSlider.value != _stamina)
        {
            staminaSlider.value = _stamina;
        }
    }

    public void TakeDamage(float damage)
    {
        _health = Mathf.Clamp(_health - damage, 0f, maxHealth);
    }

    public void Heal(float amount)
    {
        _health = Mathf.Clamp(_health + amount, 0f, maxHealth);
    }

    public void DeductStamina(float amount)
    {
        _stamina = Mathf.Clamp(_stamina - amount, 0f, maxStamina);
    }

    public void RecoverStamina(float amount)
    {
        _stamina = Mathf.Clamp(_stamina + amount, 0f, maxStamina);
    }

    public float GetCurrentStamina()
    {
        return _stamina;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }}