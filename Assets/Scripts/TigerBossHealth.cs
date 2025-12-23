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

    [Header("Phase Thresholds")]
    [SerializeField] private int phase2Threshold = 200;
    [SerializeField] private int phase3Threshold = 100;

    private int currentPhase = 1;
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
        if (animator != null)
            animator.SetTrigger("TakeDamage");

        // Alert AI
        if (bossAI != null)
            bossAI.TakeDamage(damage);

        Debug.Log("Boss Health: " + currentHealth + "/" + maxHealth);

        // Check phase changes
        CheckPhaseTransition();

        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckPhaseTransition()
    {
        if (currentHealth <= phase3Threshold && currentPhase != 3)
        {
            currentPhase = 3;
            EnterPhase3();
        }
        else if (currentHealth <= phase2Threshold && currentPhase != 2)
        {
            currentPhase = 2;
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        Debug.Log("Tiger Boss entering Phase 2!");
        if (animator != null)
            animator.SetTrigger("Phase2");
    }

    private void EnterPhase3()
    {
        Debug.Log("Tiger Boss entering Phase 3 - ENRAGED!");
        if (animator != null)
            animator.SetTrigger("Phase3");
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
    public int GetPhase() => currentPhase;
    public bool IsDead() => isDead;
    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}
