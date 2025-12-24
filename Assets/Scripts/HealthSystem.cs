using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float knockbackForce = 5f; // How far the enemy slides back
    [SerializeField] private float stunDuration = 0.5f; // How long they stop moving

    private Animator enemyAnim;
    private NavMeshAgent agent;
    private bool isDead = false;

    void Start()
    {
        enemyAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public bool IsDead() => isDead;

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"<color=red>HIT!</color> {gameObject.name} lost {damage} HP. Remaining Health: {health}");

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (enemyAnim != null) enemyAnim.SetTrigger("Hit");

            // Stop current movement and apply knockback
            StopAllCoroutines();
            StartCoroutine(ApplyHitEffects(attackerPosition));
        }
    }

    private IEnumerator ApplyHitEffects(Vector3 attackerPosition)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;

            // KNOCKBACK LOGIC
            // Calculate direction away from the attacker
            Vector3 knockbackDir = (transform.position - attackerPosition).normalized;

            // Move the agent manually for a brief moment
            float timer = 0;
            while (timer < 0.2f) // Apply force over 0.2 seconds
            {
                agent.Move(knockbackDir * knockbackForce * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            // STUN LOGIC
            yield return new WaitForSeconds(stunDuration);

            if (!isDead) agent.isStopped = false;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
        }

        if (enemyAnim != null) enemyAnim.SetTrigger("Die");

        if (TryGetComponent(out Collider col)) col.enabled = false;

        Invoke("DisableAgent", 0.1f);
        Destroy(gameObject, 5f);
    }

    private void DisableAgent()
    {
        if (agent != null) agent.enabled = false;
    }
}