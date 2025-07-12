using UnityEngine;

public interface IOnHitEffect
{
    public void BeforeHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null);
    public void AfterHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null);

    public GameObject GetGameObject();
}