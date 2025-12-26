using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;

    [SerializeField] private HealthBar healthBar;

    [Header("Weapon")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject swordOnShoulder;

    public bool isEquipping;
    public bool isEquipped;
    public bool isBlocking;
    public bool isAttacking;
    public bool isRolling;

    private float timeSinceAttack;
    public int currentAttack = 0;

    [Header("Roll")]
    public float rollCooldown = 1.2f;
    private float lastRollTime;

    void Start()
    {
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
        }
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime; // Existing line

        Attack();
        Equip();
        Block();
        Roll();

        // Apply root motion for rolling, attacking, blocking, or equipping
        playerAnim.applyRootMotion = isRolling || isAttacking || isBlocking;
    }


    private void Equip()
    {
        if (isAttacking || isRolling) return;

        if (Input.GetKeyDown(KeyCode.R) && playerAnim.GetBool("Grounded"))
        {
            isEquipping = true;
            playerAnim.SetTrigger("Equip");
        }
    }


    public void ActiveWeapon()
    {
        bool equip = !isEquipped;
        sword.SetActive(equip);
        swordOnShoulder.SetActive(!equip);
        isEquipped = equip;
    }

    public void Equipped() // animation event
    {
        isEquipping = false;
    }

    private void Block()
    {
        if (Input.GetKey(KeyCode.Mouse1) && playerAnim.GetBool("Grounded") && !isRolling)
        {
            isBlocking = true;
            playerAnim.SetBool("Block", true);
        }
        else
        {
            isBlocking = false;
            playerAnim.SetBool("Block", false);
        }
    }

    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Q) && playerAnim.GetBool("Grounded") && !isRolling)
        {
            // Check if we have enough stamina before allowing the roll
            if (healthBar != null && !healthBar.HasStamina(20f)) return;

            if (Time.time > lastRollTime + rollCooldown)
            {

                // 1. Get movement input
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                Vector3 inputDir = new Vector3(h, 0, v).normalized;

                // 2. If moving, rotate to face that direction immediately
                if (inputDir.magnitude > 0.1f)
                {
                    transform.rotation = Quaternion.LookRotation(inputDir);
                }
                // If no input, the player just rolls in their current forward direction

                // 3. Start the roll
                isRolling = true;
                lastRollTime = Time.time;
                playerAnim.SetTrigger("Roll");

                if (healthBar != null) healthBar.DeductStamina(20f);

                // 4. Force stop block
                isBlocking = false;
                playerAnim.SetBool("Block", false);
            }
        }
    }

    public void EndRoll() // animation event
    {
        isRolling = false;
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) &&
            playerAnim.GetBool("Grounded") &&
            timeSinceAttack > 0.8f &&
            isEquipped &&
            !isRolling)
        {
            if (healthBar != null && !healthBar.HasStamina(10f)) return;

            currentAttack++;
            isAttacking = true;

            if (currentAttack > 3) currentAttack = 1;
            if (timeSinceAttack > 1f) currentAttack = 1;

            playerAnim.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0f;

            if (healthBar != null) healthBar.DeductStamina(10f);
        }
    }

    public void ResetAttack() // animation event
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        // You can expand this with health system integration
        Debug.Log("Player took " + damage + " damage!");
        
        // Play damage animation
        if (playerAnim != null)
        {
            playerAnim.SetTrigger("TakeDamage");
        }
    }

}
