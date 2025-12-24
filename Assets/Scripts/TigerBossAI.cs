using UnityEngine;
using UnityEngine.AI;

public class TigerBossAI : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private BossWeapon bossWeapon;

    [Header("Optional Components")]
    [SerializeField] private Transform weaponHand;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 50f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float stoppingDistance = 4f;

    [Header("Attack Cooldowns")]
    [SerializeField] private float heavyAttackCooldown = 3f;
    [SerializeField] private float lightAttackCooldown = 1.5f;
    [SerializeField] private float roarCooldown = 8f;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float approachBuffer = 0.75f; // how much closer than attackRange to get before stopping

    private Transform player;
    private float lastHeavyAttackTime = 0f;
    private float lastLightAttackTime = 0f;
    private float lastRoarTime = 0f;
    private BossState currentState = BossState.Idle;
    private bool hasPlayerInSight = false;

    public enum BossState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Roar,
        Stun
    }

    private void OnEnable()
    {
        SetupComponents();
    }

    private void SetupComponents()
    {
        // Auto-find Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("TigerBossAI: No Animator found! Assign one in Inspector.", gameObject);
        }

        // Auto-find NavMeshAgent
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
                Debug.LogError("TigerBossAI: No NavMeshAgent found! Add one to the boss.", gameObject);
        }

        // Apply configured stopping distance
        if (navMeshAgent != null)
        {
            // Stop a bit inside attack range so melee can connect
            navMeshAgent.stoppingDistance = Mathf.Max(0f, Mathf.Min(stoppingDistance, attackRange - Mathf.Max(0.1f, approachBuffer)));
        }

        // Auto-find BossWeapon in children
        if (bossWeapon == null)
        {
            bossWeapon = GetComponentInChildren<BossWeapon>();
            if (bossWeapon == null)
                Debug.LogError("TigerBossAI: No BossWeapon found in children! Assign the weapon.", gameObject);
        }

        // Try to find Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("TigerBossAI: No player found with 'Player' tag!");
    }

    private void Start()
    {
        // Try to place the agent on the nearest NavMesh at start
        EnsureOnNavMesh();
    }

    // Ensures the NavMeshAgent is active and placed on a NavMesh.
    // Attempts to snap the boss to the nearest valid NavMesh position.
    private bool EnsureOnNavMesh(float searchRadius = 10f)
    {
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled)
            return false;

        if (navMeshAgent.isOnNavMesh)
            return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, searchRadius, NavMesh.AllAreas))
        {
            // Temporarily disable agent to move Transform, then re-enable
            navMeshAgent.enabled = false;
            transform.position = hit.position;
            navMeshAgent.enabled = true;
            return navMeshAgent.isOnNavMesh;
        }

        // Fallback: try placing near the player if available
        if (player != null && NavMesh.SamplePosition(player.position, out hit, searchRadius * 2f, NavMesh.AllAreas))
        {
            navMeshAgent.enabled = false;
            transform.position = hit.position;
            navMeshAgent.enabled = true;
            return navMeshAgent.isOnNavMesh;
        }

        Debug.LogWarning("TigerBossAI: Could not find NavMesh near boss or player to place agent.", gameObject);
        return false;
    }

    private void Update()
    {
        if (player == null || navMeshAgent == null || animator == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        hasPlayerInSight = distanceToPlayer < detectionRange;

        // Update animator
        animator.SetBool("PlayerSighted", hasPlayerInSight);
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Patrol:
                HandlePatrolState();
                break;
            case BossState.Chase:
                HandleChaseState(distanceToPlayer);
                break;
            case BossState.Attack:
                HandleAttackState();
                break;
            case BossState.Roar:
                HandleRoarState();
                break;
        }

        // Face player when in combat
        if (hasPlayerInSight && currentState != BossState.Idle)
        {
            FacePlayer();
        }
    }

    private void HandleIdleState()
    {
        navMeshAgent.velocity = Vector3.zero;

        if (hasPlayerInSight)
        {
            currentState = BossState.Chase;
        }
    }

    private void HandlePatrolState()
    {
        navMeshAgent.speed = patrolSpeed;

        if (hasPlayerInSight)
        {
            currentState = BossState.Chase;
        }
    }

    private void HandleChaseState(float distanceToPlayer)
    {
        navMeshAgent.speed = chaseSpeed;

        if (distanceToPlayer < attackRange)
        {
            navMeshAgent.velocity = Vector3.zero;
            currentState = BossState.Attack;
        }
        else
        {
            if (EnsureOnNavMesh())
            {
                navMeshAgent.SetDestination(player.position);
            }
            // If not on NavMesh, EnsureOnNavMesh() already attempted a fix and logged a warning.
        }

        if (!hasPlayerInSight)
        {
            currentState = BossState.Patrol;
        }
    }

    private void HandleAttackState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // If drifted out of range, resume chasing
        if (distanceToPlayer > attackRange + 2f)
        {
            currentState = BossState.Chase;
            navMeshAgent.ResetPath();
            return;
        }

        // Micro-approach to ensure we are close enough to actually hit
        float desiredStop = Mathf.Max(0.1f, attackRange - approachBuffer * 0.5f);
        if (distanceToPlayer > desiredStop)
        {
            navMeshAgent.speed = chaseSpeed;
            if (EnsureOnNavMesh())
            {
                navMeshAgent.SetDestination(player.position);
            }
            return; // keep closing in this frame
        }

        // Close enough: stop and perform attacks
        navMeshAgent.velocity = Vector3.zero;

        // Check if an attack animation is currently playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingAttack = stateInfo.IsName("LightAttack") || stateInfo.IsName("HeavyAttack") || stateInfo.IsName("Roar");

        // Only start new attacks if no attack is currently playing
        if (!isPlayingAttack)
        {
            // Attack decision logic
            float timeSinceRoar = Time.time - lastRoarTime;
            if (timeSinceRoar > roarCooldown && Random.value > 0.6f)
            {
                PerformRoar();
            }
            else
            {
                float timeSinceHeavy = Time.time - lastHeavyAttackTime;
                float timeSinceLight = Time.time - lastLightAttackTime;

                if (timeSinceHeavy > heavyAttackCooldown && Random.value > 0.5f)
                {
                    PerformHeavyAttack();
                }
                else if (timeSinceLight > lightAttackCooldown)
                {
                    PerformLightAttack();
                }
                else
                {
                    // No attack ready - play idle animation
                    animator.SetFloat("Speed", 0);
                }
            }
        }
    }

    private void HandleRoarState()
    {
        navMeshAgent.velocity = Vector3.zero;
        
        // Check if roar animation has finished
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Roar") || stateInfo.normalizedTime >= 1.0f)
        {
            // Roar finished, return to attack state
            currentState = BossState.Attack;
        }
    }

    private void PerformLightAttack()
    {
        if (animator != null)
            animator.SetTrigger("LightAttack");
        
        lastLightAttackTime = Time.time;
        
        if (bossWeapon != null)
            bossWeapon.ActivateWeapon(0.5f, 15);
    }

    private void PerformHeavyAttack()
    {
        if (animator != null)
            animator.SetTrigger("HeavyAttack");
        
        lastHeavyAttackTime = Time.time;
        
        if (bossWeapon != null)
            bossWeapon.ActivateWeapon(0.8f, 30);
    }

    private void PerformRoar()
    {
        if (animator != null)
            animator.SetTrigger("Roar");
        
        lastRoarTime = Time.time;
        currentState = BossState.Roar;
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void OnRoarEnd()
    {
        currentState = BossState.Attack;
    }

    public void TakeDamage(int damage)
    {
        if (animator != null)
            animator.SetTrigger("TakeDamage");
    }

    public bool IsInAttackRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    public BossState GetCurrentState() => currentState;
}
