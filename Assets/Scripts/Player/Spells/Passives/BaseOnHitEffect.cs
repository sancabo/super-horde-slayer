using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PickableBehavoiur))]
public abstract class BaseOnHitEffect : BaseEffect, IOnHitEffect
{

    abstract public void BeforeHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null);

    abstract public void AfterHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null);

    public override Type GetEffectType()
    {
        return Type.ON_HIT;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

}
