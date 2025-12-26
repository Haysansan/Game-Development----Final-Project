using UnityEngine;
using UnityEngine.AI;

public class TigerBossAI : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private BossWeapon bossWeapon;
    private HealthSystem healthSystem; // Added reference

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
    [SerializeField] private float approachBuffer = 0.75f;

    private Transform player;
    private float lastHeavyAttackTime = 0f;
    private float lastLightAttackTime = 0f;
    private float lastRoarTime = 0f;
    private BossState currentState = BossState.Idle;
    private bool hasPlayerInSight = false;

    public enum BossState { Idle, Patrol, Chase, Attack, Roar, Stun, Dead }

    private void OnEnable() => SetupComponents();

    private void SetupComponents()
    {
        healthSystem = GetComponent<HealthSystem>(); // Initialize reference
        if (animator == null) animator = GetComponent<Animator>();
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent != null)
            navMeshAgent.stoppingDistance = Mathf.Max(0f, Mathf.Min(stoppingDistance, attackRange - Mathf.Max(0.1f, approachBuffer)));

        if (bossWeapon == null) bossWeapon = GetComponentInChildren<BossWeapon>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void Start() => EnsureOnNavMesh();

    private void Update()
    {
        // MASTER SWITCH: If dead, stop all AI logic immediately
        if (healthSystem != null && healthSystem.IsDead())
        {
            currentState = BossState.Dead;
            return;
        }

        if (player == null || navMeshAgent == null || animator == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        hasPlayerInSight = distanceToPlayer < detectionRange;

        animator.SetBool("PlayerSighted", hasPlayerInSight);
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        switch (currentState)
        {
            case BossState.Idle: HandleIdleState(); break;
            case BossState.Patrol: HandlePatrolState(); break;
            case BossState.Chase: HandleChaseState(distanceToPlayer); break;
            case BossState.Attack: HandleAttackState(); break;
            case BossState.Roar: HandleRoarState(); break;
        }

        if (hasPlayerInSight && currentState != BossState.Idle && currentState != BossState.Dead)
        {
            FacePlayer();
        }
    }

    // ... Keep all your existing HandleState, PerformAttack, and FacePlayer methods exactly as they were ...
    // Note: The FacePlayer() and HandleAttackState() are now protected by the IsDead() check in Update.

    private void HandleIdleState()
    {
        navMeshAgent.velocity = Vector3.zero;
        if (hasPlayerInSight) currentState = BossState.Chase;
    }

    private void HandlePatrolState()
    {
        navMeshAgent.speed = patrolSpeed;
        if (hasPlayerInSight) currentState = BossState.Chase;
    }

    private void HandleChaseState(float distanceToPlayer)
    {
        navMeshAgent.speed = chaseSpeed;
        if (distanceToPlayer < attackRange)
        {
            navMeshAgent.velocity = Vector3.zero;
            currentState = BossState.Attack;
        }
        else if (EnsureOnNavMesh())
        {
            navMeshAgent.SetDestination(player.position);
        }

        if (!hasPlayerInSight) currentState = BossState.Patrol;
    }

    private void HandleAttackState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange + 2f)
        {
            currentState = BossState.Chase;
            if (navMeshAgent.isActiveAndEnabled) navMeshAgent.ResetPath();
            return;
        }

        float desiredStop = Mathf.Max(0.1f, attackRange - approachBuffer * 0.5f);
        if (distanceToPlayer > desiredStop)
        {
            navMeshAgent.speed = chaseSpeed;
            if (EnsureOnNavMesh()) navMeshAgent.SetDestination(player.position);
            return;
        }

        navMeshAgent.velocity = Vector3.zero;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingAttack = stateInfo.IsName("LightAttack") || stateInfo.IsName("HeavyAttack") || stateInfo.IsName("Roar");

        if (!isPlayingAttack)
        {
            float timeSinceRoar = Time.time - lastRoarTime;
            if (timeSinceRoar > roarCooldown && Random.value > 0.6f) PerformRoar();
            else
            {
                if (Time.time - lastHeavyAttackTime > heavyAttackCooldown && Random.value > 0.5f) PerformHeavyAttack();
                else if (Time.time - lastLightAttackTime > lightAttackCooldown) PerformLightAttack();
                else animator.SetFloat("Speed", 0);
            }
        }
    }

    private void HandleRoarState()
    {
        navMeshAgent.velocity = Vector3.zero;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Roar") || stateInfo.normalizedTime >= 1.0f) currentState = BossState.Attack;
    }

    private void PerformLightAttack()
    {
        animator.SetTrigger("LightAttack");
        lastLightAttackTime = Time.time;
        if (bossWeapon != null) bossWeapon.ActivateWeapon(0.5f, 15);
    }

    private void PerformHeavyAttack()
    {
        animator.SetTrigger("HeavyAttack");
        lastHeavyAttackTime = Time.time;
        if (bossWeapon != null) bossWeapon.ActivateWeapon(0.8f, 30);
    }

    private void PerformRoar()
    {
        animator.SetTrigger("Roar");
        lastRoarTime = Time.time;
        currentState = BossState.Roar;
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Keep rotation upright
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private bool EnsureOnNavMesh(float searchRadius = 10f)
    {
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled) return false;
        if (navMeshAgent.isOnNavMesh) return true;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, searchRadius, NavMesh.AllAreas))
        {
            navMeshAgent.enabled = false;
            transform.position = hit.position;
            navMeshAgent.enabled = true;
            return navMeshAgent.isOnNavMesh;
        }
        return false;
    }
}