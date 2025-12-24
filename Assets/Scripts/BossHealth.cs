using UnityEngine;

public class TigerBossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 300;
    [SerializeField] private int currentHealth;

    [Header("References")]
    [SerializeField] private TigerBossAI bossAI;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Animator animator;
    private bool isDead = false;

    private void OnEnable()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;

        // Auto-find components if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();

        if (bossAI == null)
            bossAI = GetComponent<TigerBossAI>();

        if (healthBar == null)
            healthBar = GetComponentInChildren<HealthBar>();

        // Setup health bar
        if (healthBar != null)
            healthBar.SetMaxHealth((float)maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Update UI
        if (healthBar != null)
            healthBar.SetHealth((float)currentHealth);

        // Trigger animation
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Only fire if the controller actually defines the trigger
            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger && param.name == "TakeDamage")
                {
                    animator.SetTrigger("TakeDamage");
                    break;
                }
            }
        }

        Debug.Log("Boss Health: " + currentHealth + "/" + maxHealth);

        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Tiger Boss Defeated!");

        if (animator != null)
            animator.SetTrigger("Death");

        if (bossAI != null)
            bossAI.enabled = false;

        // Disable colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        // Schedule destruction
        Destroy(gameObject, 3f);
    }

    public int GetHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}
