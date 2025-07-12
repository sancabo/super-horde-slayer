using UnityEngine;

public interface ISpellLauncher : IPickable, IInventoryItem
{
    public void RequestLaunch(Vector3 direction, GameObject originator);

    public void ApplyBonusDamage(float bonusDamage);

    public void ApplyBonusDamageFlat(float bonusDamage);

    public void ApplyAoeBonus(float aoeBonus);

    public int GetCost();

    public void SetCost(int cost);

    public float GetCastingTime();

    public void RequestCastingEffect(Transform parent);

    public float GetTotalDamage();

    public GameObject GetObject();
    public string GetLauncherName();

    public float GetBonusDamage();

    public float GetBonusAoe();

    public void SetBonusDamage(float bonusDamage);

    public void SetBonusAoe(float bonusAoe);
   

}

