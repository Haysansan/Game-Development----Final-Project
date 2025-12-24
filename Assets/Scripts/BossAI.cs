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

    [Header("Attack Animations")]
    [SerializeField] private string normalAttackTrigger = "NormalAttack";
    [SerializeField] private string attack1Trigger = "Attack1";
    [SerializeField] private string attack2Trigger = "Attack2";
    [SerializeField] private string normalAttackStateName = "NormalAttack";
    [SerializeField] private string attack1StateName = "Attack1";
    [SerializeField] private string attack2StateName = "Attack2";

    [Header("Attack Cooldowns")]
    [SerializeField] private float normalAttackCooldown = 1.0f;
    [SerializeField] private float attack1Cooldown = 2.0f;
    [SerializeField] private float attack2Cooldown = 3.0f;

    [Header("Weapon Windows")]
    [SerializeField] private float normalAttackWeaponDuration = 0.5f;
    [SerializeField] private int normalAttackDamage = 15;
    [SerializeField] private float attack1WeaponDuration = 0.6f;
    [SerializeField] private int attack1Damage = 20;
    [SerializeField] private float attack2WeaponDuration = 0.9f;
    [SerializeField] private int attack2Damage = 30;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float approachBuffer = 0.75f; // how much closer than attackRange to get before stopping

    [Header("Attack Lock")]
    [Tooltip("Time to lock NavMeshAgent movement while an attack plays to prevent sliding.")]
    [SerializeField] private float attackMoveLockDuration = 1.0f;

    private Transform player;
    private float lastNormalAttackTime = 0f;
    private float lastAttack1Time = 0f;
    private float lastAttack2Time = 0f;
    private float attackLockTimer = 0f;
    private BossState currentState = BossState.Idle;
    private bool hasPlayerInSight = false;
    private bool isAttacking = false;

    public enum BossState
    {
        Idle,
        Chase,
        Attack
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
            case BossState.Chase:
                HandleChaseState(distanceToPlayer);
                break;
            case BossState.Attack:
                HandleAttackState();
                break;
        }

        // Maintain attack lock timer
        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;
            if (attackLockTimer <= 0f)
            {
                isAttacking = false;
                ResumeAgentMovement();
            }
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
        navMeshAgent.ResetPath();

        if (hasPlayerInSight)
        {
            currentState = BossState.Chase;
        }
    }

    private void HandleChaseState(float distanceToPlayer)
    {
        ResumeAgentMovement();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = chaseSpeed;

        if (distanceToPlayer < attackRange)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.ResetPath();
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
            currentState = BossState.Idle;
        }
    }

    private void HandleAttackState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // If drifted out of range, resume chasing
        if (distanceToPlayer > attackRange + 2f)
        {
            currentState = BossState.Chase;
            attackLockTimer = 0f;
            isAttacking = false;
            ResumeAgentMovement();
            navMeshAgent.ResetPath();
            return;
        }

        // Micro-approach to ensure we are close enough to actually hit
        float desiredStop = Mathf.Max(0.1f, attackRange - approachBuffer * 0.5f);
        if (distanceToPlayer > desiredStop && !isAttacking)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = chaseSpeed;
            if (EnsureOnNavMesh())
            {
                navMeshAgent.SetDestination(player.position);
            }
            return; // keep closing in this frame
        }

        // Close enough: FULLY STOP the NavMeshAgent to prevent floating
            StopAgentForAttack();

        // Check if an attack animation is currently playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingAttack = IsInAttackState(stateInfo);

        // Only start new attacks if no attack is currently playing
        if (!isPlayingAttack)
        {
            isAttacking = false;
            
            bool normalReady = CanUseNormalAttack();
            bool attack1Ready = CanUseAttack1();
            bool attack2Ready = CanUseAttack2();

            // Prioritize normal attack for faster pacing
            if (normalReady && Random.value > 0.4f)
            {
                PerformNormalAttack();
            }
            else if (attack1Ready && attack2Ready)
            {
                // Alternate combo choices to keep variety
                if (Random.value > 0.5f)
                    PerformAttack2();
                else
                    PerformAttack1();
            }
            else if (attack2Ready)
            {
                PerformAttack2();
            }
            else if (attack1Ready)
            {
                PerformAttack1();
            }
            else if (normalReady)
            {
                PerformNormalAttack();
            }
        }
        else
        {
            isAttacking = true;
        }
    }

    private bool CanUseNormalAttack() => Time.time - lastNormalAttackTime >= normalAttackCooldown;
    private bool CanUseAttack1() => Time.time - lastAttack1Time >= attack1Cooldown;
    private bool CanUseAttack2() => Time.time - lastAttack2Time >= attack2Cooldown;

    private bool IsInAttackState(AnimatorStateInfo stateInfo)
    {
        // Also consider animator tags named "Attack" if your states use tags
        return stateInfo.IsName(normalAttackStateName)
            || stateInfo.IsName(attack1StateName)
            || stateInfo.IsName(attack2StateName)
            || stateInfo.IsTag("Attack");
    }

    private void StartAttackLock(float duration)
    {
        attackLockTimer = Mathf.Max(attackLockTimer, duration);
        isAttacking = true;
        StopAgentForAttack();
    }

    private void PerformNormalAttack()
    {
        if (animator != null)
            animator.SetTrigger(normalAttackTrigger);

        lastNormalAttackTime = Time.time;
        StartAttackLock(Mathf.Max(attackMoveLockDuration, normalAttackWeaponDuration + 0.2f));

        if (bossWeapon != null)
            bossWeapon.ActivateWeapon(normalAttackWeaponDuration, normalAttackDamage);
    }

    private void PerformAttack1()
    {
        if (animator != null)
            animator.SetTrigger(attack1Trigger);

        lastAttack1Time = Time.time;
        StartAttackLock(Mathf.Max(attackMoveLockDuration, attack1WeaponDuration + 0.2f));

        if (bossWeapon != null)
            bossWeapon.ActivateWeapon(attack1WeaponDuration, attack1Damage);
    }

    private void PerformAttack2()
    {
        if (animator != null)
            animator.SetTrigger(attack2Trigger);

        lastAttack2Time = Time.time;
        StartAttackLock(Mathf.Max(attackMoveLockDuration, attack2WeaponDuration + 0.2f));

        if (bossWeapon != null)
            bossWeapon.ActivateWeapon(attack2WeaponDuration, attack2Damage);
    }

    private void StopAgentForAttack()
    {
        if (navMeshAgent == null) return;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.ResetPath();
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    private void ResumeAgentMovement()
    {
        if (navMeshAgent == null) return;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
        navMeshAgent.isStopped = false;
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public bool IsInAttackRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    public BossState GetCurrentState() => currentState;
}
