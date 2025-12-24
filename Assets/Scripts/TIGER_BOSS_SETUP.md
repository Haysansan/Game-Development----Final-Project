# Tiger Boss System Setup Guide

## Overview
This is a complete boss system for your tiger boss with weapon mechanics, AI behavior, and health phases.

## Created Scripts

### 1. **TigerBossAI.cs** - Boss Behavior & AI
Controls the tiger boss's behavior including:
- **Detection System**: Detects player within 50 units
- **State Machine**: Idle → Patrol → Chase → Attack → Roar
- **Attack Patterns**:
  - Light Attacks (1.5s cooldown, 15 damage)
  - Heavy Attacks (3s cooldown, 30 damage)
  - Roar ability (8s cooldown, stuns player area)
- **Movement**: NavMeshAgent-based pathfinding
- **Combat**: Faces player while attacking

### 2. **BossWeapon.cs** - Weapon Attack System
Manages the weapon collider and damage:
- Activates weapon for specific duration during attacks
- Applies damage on collision with player
- Applies knockback to player
- Prevents multiple hits per attack 

### 3. **TigerBossHealth.cs** - Health & Phase System
Handles boss health and battle phases:
- **Phase 1** (100-300 HP): Normal behavior
- **Phase 2** (≤200 HP): Increased aggression (customizable)
- **Phase 3** (≤100 HP): Enraged mode (customizable)
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
- `LightAttack`
- `HeavyAttack`
- `Roar`
- `TakeDamage`
- `Phase2`
- `Phase3`
- `Death`

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
[SerializeField] private int phase2Threshold = 200;
[SerializeField] private int phase3Threshold = 100;
```

### Adjust Attack Patterns
Modify in **TigerBossAI.cs**:
```csharp
[SerializeField] private float detectionRange = 50f;      // How far boss sees
[SerializeField] private float attackRange = 5f;          // Range to attack
[SerializeField] private float heavyAttackCooldown = 3f;  // Cooldown timer
[SerializeField] private float lightAttackCooldown = 1.5f;
[SerializeField] private float chaseSpeed = 6f;           // Movement speed
```

### Adjust Weapon Damage
Modify in **BossWeapon.cs**:
```csharp
[SerializeField] private int baseDamage = 20;
[SerializeField] private float knockbackForce = 500f;
```

Or override per attack in **TigerBossAI.cs**:
```csharp
bossWeapon?.ActivateWeapon(0.5f, 15);   // Light attack: 15 damage
bossWeapon?.ActivateWeapon(0.8f, 30);   // Heavy attack: 30 damage
```

## Attack Flow

1. **Light Attack**: 15 damage, 0.5s weapon active, 1.5s cooldown
2. **Heavy Attack**: 30 damage, 0.8s weapon active, 3s cooldown
3. **Roar**: Stun ability, triggers animation, 8s cooldown

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

**Animations not playing?**
- Create animation transitions in Animator for all triggers
- Verify parameter names match exactly
- Check animation states exist in Animator

## Next Steps

1. Create animation transitions in Animator for all attack states
2. Add visual effects (particle systems, screen shake)
3. Add audio effects (attack sounds, roar, hit sounds)
4. Fine-tune phase transition timings
5. Add loot/reward system for defeating boss
6. Consider adding more complex attack patterns in Phase 3
