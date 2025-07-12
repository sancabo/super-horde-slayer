using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PickableBehavoiur))]
public abstract class BaseOnKillEffect : BaseEffect, IOnKillEffect
{

    public override Type GetEffectType()
    {
        return Type.ON_KILL;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public abstract void Trigger(Player player, Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null);
}
