# Tiger Boss System Setup Guide

## Overview
This is a complete boss system for your tiger boss with weapon mechanics and a simplified animation set (idle, walk, two attack combos, death).

## Created Scripts

### 1. **TigerBossAI.cs** - Boss Behavior & AI
Controls the tiger boss's behavior including:
- **Detection System**: Detects player within 50 units
- **State Machine**: Idle → Chase → Attack
- **Attack Patterns**:
  - Normal Attack trigger (`NormalAttack`) (1.0s cooldown, 15 damage)
  - Combo 1 trigger (`Attack1`) (2.0s cooldown, 20 damage)
  - Combo 2 trigger (`Attack2`) (3.0s cooldown, 30 damage)
- **Movement**: NavMeshAgent-based pathfinding (reduced speed: 3.5)
- **Combat**: Faces player while attacking, **stops movement during attacks to prevent floating**
- Applies knockback to player
- Prevents multiple hits per attack

### 3. **TigerBossHealth.cs** - Health System
Handles boss health and death:
- Health bar integration
- Death sequence with animation

## Setup Instructions

### Step 1: Configure the Tiger Boss Prefab
1. Select the **tiger boss** prefab in your Prefabs folder
2. Inspect it in the Inspector:
   - Add a **NavMeshAgent** component if not present
   - Ensure it has an **Animator** component
   - Add a **Rigidbody** (if needed for physics)
   - Tag the boss as "Boss" or "Enemy"

### Step 2: Setup the Boss Weapon
1. Find the weapon object under the tiger boss (it should be a child of the hand bone)
2. Add the **BossWeapon.cs** component to the weapon
3. Configure in Inspector:
   - **Base Damage**: 20 (default)
   - **Knockback Force**: 500 (adjust for desired knockback)
   - **Weapon Collider**: Assign or ensure it has a Collider (use Trigger)

### Step 3: Add AI Component
1. Select the tiger boss prefab
2. Add the **TigerBossAI.cs** component
3. Assign in Inspector:
   - **Animator**: Drag the Animator component
   - **NavMeshAgent**: Drag the NavMeshAgent
   - **Weapon Hand**: Drag the hand bone transform
   - **Boss Weapon**: Drag the weapon object with BossWeapon script
   - Configure ranges and speeds as needed

### Step 4: Add Health Component
1. Select the tiger boss prefab
2. Add the **TigerBossHealth.cs** component
3. Assign in Inspector:
   - **Max Health**: 300 (adjust difficulty)
   - **Boss AI**: Drag the TigerBossAI component
   - **Health Bar**: Drag a HealthBar UI element (optional)
   - **Animator**: Drag the Animator

### Step 5: Setup Animator Parameters
Make sure your Animator has these **Trigger** parameters:
- `NormalAttack`
- `Attack1`
- `Attack2`
- `Death`
- (Optional) `TakeDamage` if you add a hurt flinch

And these **Bool** parameters:
- `PlayerSighted`

And this **Float** parameter:
- `Speed`

### Step 6: Setup NavMesh
1. Open **Window → AI → Navigation**
2. Mark your terrain/floor as "Walkable"
3. Mark walls/obstacles as "Not Walkable"
4. Click **Bake** to generate NavMesh
5. The boss should now be able to navigate properly

### Step 7: Player Setup
Make sure your player has:
- **Tag**: "Player" (used for detection)
- **TakeDamage()** method (already added to PlayerController)
- **Rigidbody** component (for knockback effects)

## Customization

### Adjust Difficulty
Modify in **TigerBossHealth.cs**:
```csharp
[SerializeField] private int maxHealth = 300;  // Higher = more HP
```

### Adjust Attack Patterns
Modify in **TigerBossAI.cs**:
```csharp
[SerializeField] private float detectionRange = 50f; // How far boss sees
[SerializeField] private float attackRange = 5f;     // Range to attack
[SerializeField] private float normalAttackCooldown = 1.0f;
[SerializeField] private float attack1Cooldown = 2.0f;
[SerializeField] private float attack2Cooldown = 3.0f;
[SerializeField] private float chaseSpeed = 3.5f;    // Movement speed (reduced)
```

### Adjust Weapon Damage
Modify in **BossWeapon.cs**:
```csharp
[SerializeField] private int baseDamage = 20;
[SerializeField] private float knockbackForce = 500f;
```

Or override per attack in **TigerBossAI.cs**:
```csharp
bossWeapon?.ActivateWeapon(attack1WeaponDuration, attack1Damage);   // Combo 1
bossWeapon?.ActivateWeapon(attack2WeaponDuration, attack2Damage);   // Combo 2
```

## Attack Flow

1. **Normal Attack (`NormalAttack`)**: 15 damage, 0.5s weapon active, 1.0s cooldown - fastest attack
2. **Combo 1 (`Attack1`)**: 20 damage, 0.6s weapon active, 2.0s cooldown
3. **Combo 2 (`Attack2`)**: 30 damage, 0.9s weapon active, 3.0s cooldown

**Note:** Boss will **completely stop moving during attack animations** to prevent floating/sliding with legs planted on the ground.

## Testing Tips

1. **Disable NavMesh temporarily** to test melee combat in small areas
2. **Increase detection range** to 100 for easier testing
3. **Decrease cooldowns** to see attack patterns faster
4. **Lower max health** to quickly test death sequences
5. **Use Debug.Log()** to track state changes

## Troubleshooting

**Boss not moving?**
- Check NavMesh is baked properly
- Ensure NavMeshAgent is assigned in AI script
- Verify floor is marked "Walkable" in Navigation settings

**Boss not attacking?**
- Verify Animator parameters match exactly (case-sensitive)
- Check weapon collider is set to "Is Trigger"
- Ensure BossWeapon script is on weapon object

**Player not taking damage?**
- Verify player has "Player" tag
- Check weapon collider is enabled
- Ensure BossWeapon script is assigned

**Boss floating/sliding during attack animations?**
- The script uses `navMeshAgent.isStopped = true` during attacks to prevent this
- Make sure your Animator is NOT set to "Apply Root Motion" (should be unchecked)
- Verify the NavMeshAgent component has "Auto Braking" enabled
- Check that attack animation states have transitions back to idle/walk

**Animations not playing?**
- Create animation transitions in Animator for all triggers
- Verify parameter names match exactly
- Check animation states exist in Animator

## Next Steps

1. Create animation transitions in Animator for all attack states
2. Add visual effects (particle systems, screen shake)
3. Add audio effects (attack and hit sounds)
4. Fine-tune attack timings and cooldowns
5. Add loot/reward system for defeating boss
6. Consider adding more combos or ranged attacks
