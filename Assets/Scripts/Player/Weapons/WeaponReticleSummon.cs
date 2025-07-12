using UnityEngine;
using UnityEngine.Assertions;

public class WeaponReticleSummon : MonoBehaviour
{
    public Summon summon;
    void Start()
    {
        Assert.IsNotNull(summon, "Summon not found in the scene.");
    }

    void FixedUpdate()
    {
        Vector3 direction = summon.GetWeaponDirection(); // Assuming Summon has a method to get the last known direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);        
    }
}
