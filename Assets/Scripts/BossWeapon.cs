using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private int baseDamage = 20;
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float knockbackForce = 500f;

    private bool isActive = false;
    private float activationDuration;
    private float activationTimer;
    private int currentDamage;

    private void Start()
    {
        if (weaponCollider == null)
            weaponCollider = GetComponent<Collider>();

        // Disable collider initially
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    private void Update()
    {
        if (isActive)
        {
            activationTimer -= Time.deltaTime;
            if (activationTimer <= 0)
            {
                DeactivateWeapon();
            }
        }
    }

    public void ActivateWeapon(float duration, int damageOverride = 0)
    {
        isActive = true;
        activationDuration = duration;
        activationTimer = duration;
        currentDamage = damageOverride > 0 ? damageOverride : baseDamage;

        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    public void DeactivateWeapon()
    {
        isActive = false;
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!isActive) return;

        // Check if hit the player
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(currentDamage);

                // Apply knockback
                Rigidbody playerRb = collision.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRb.velocity = Vector3.zero;
                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                }
            }

            // Deactivate after hitting to prevent multiple hits
            DeactivateWeapon();
        }
    }

    public bool IsActive() => isActive;
}
