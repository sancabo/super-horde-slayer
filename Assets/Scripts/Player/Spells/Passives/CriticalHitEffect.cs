using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CriticalHitEffect : BaseOnHitEffect
{
    private int _originalDamage;

    private float _chance = 25;

    private float _damage = 2;

    public override void BeforeHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null)
    {

        _originalDamage = 0;
        if (Random.Range(0, 101) > 100 - _chance)
        {
            if (weapon != null)
            {
                _originalDamage = weapon._activePreset._damage;
                weapon._activePreset._damage *= (int) _damage;
            }
            if (spellLauncher != null)
            {
                _originalDamage = spellLauncher.GetDamage();
                spellLauncher.ApplyDamageMultiplier(2f);
            }
            target.NextHitIsCritical();
        }
    }

    public override void AfterHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null)
    {
        if (weapon != null && _originalDamage != 0) weapon._activePreset._damage = _originalDamage;
        if (spellLauncher != null && _originalDamage != 0) spellLauncher.SetDammage(_originalDamage);
    }

    public override PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
      "Critical Hit Effect",
       "The item this is slotted into has a 25% chance of dealing double damage when it hits an enemy.",
       "On Hit Effect",
       new Dictionary<string, string> {
             { "Critical Hit Chance", $"{ Math.Round(_chance,2)}%" },
             { "Critical Hit Damage", $"x{ Math.Round(_damage,2)}" }});
    }
    
     public override void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                _damage *= 1.15f + Random.Range(-0.05f, 0.08f);
                _chance *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.RARE:
                _damage *= 1.25f + Random.Range(-0.05f, 0.1f);
                _chance *= 1.25f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.LEGENDARY:
                _damage *= 1.5f + Random.Range(-0.05f, 0.25f);
                _chance *= 1.5f + Random.Range(-0.05f, 0.08f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }
}
