using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PickableBehavoiur))]
public abstract class BasePassiveEffect : BaseEffect, IPassiveEffect
{
    public override Type GetEffectType()
    {
        return Type.PASSIVE;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Install(SlotHolderBehaviour holder) {
        if (holder.TryGetComponent(out Weapon weapon)){
            AddPassiveEffect(FindFirstObjectByType<Player>(), weapon);
        } else if (holder.TryGetComponent(out BaseSpellLauncher baseSpellLauncher)){
            AddPassiveEffect(FindFirstObjectByType<Player>(), baseSpellLauncher);
        } else {
            throw new System.Exception("Cannot install Passive effect in unknown Object");
        }
    }

    public abstract void AddPassiveEffect(Player player, BaseSpellLauncher spellLauncher);

    public abstract void AddPassiveEffect(Player player, Weapon weapon);

}
