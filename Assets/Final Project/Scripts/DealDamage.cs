using UnityEngine;



public class DealDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Collider swordCollider;

    void Start() => swordCollider = GetComponent<Collider>();

    // Call these from Animation Events
    private System.Collections.Generic.List<Collider> alreadyHit = new System.Collections.Generic.List<Collider>();
    public void EnableWeapon()
    {
        alreadyHit.Clear(); // Reset the list for a new swing
        swordCollider.enabled = true;
    }
    public void DisableWeapon() => swordCollider.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !alreadyHit.Contains(other))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.root.position);
                alreadyHit.Add(other); // Mark this enemy as hit for this swing
            }
        }
    }
}