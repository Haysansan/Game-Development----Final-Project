using UnityEngine;

public class WeaponAnimationHandler : MonoBehaviour
{
    // Drag your Sword object (the one with DealDamage) into this slot in the Inspector
    [SerializeField] private DealDamage swordScript;

    // The Animation Event will call THESE functions
    public void EnableWeapon()
    {
        if (swordScript != null) swordScript.EnableWeapon();
    }

    public void DisableWeapon()
    {
        if (swordScript != null) swordScript.DisableWeapon();
    }
}