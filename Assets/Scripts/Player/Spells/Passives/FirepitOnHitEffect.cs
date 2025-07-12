using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirepitOnHit : BaseOnHitEffect
{
    public float damagePerTick = 6f;

    public float damageModifier = 1f;
    public float durationSeconds = 4f;
    public float tickMs = 200f;

    [SerializeField] private GameObject firePitPrefab;

    public override void AfterHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null)
    {
        if (Random.Range(0, 101) > 0)
        {
            if (target.transform.GetComponentInChildren<Firepit>() == null)
            {
                Firepit firepit = Instantiate(firePitPrefab, target.transform.position, Quaternion.identity).GetComponent<Firepit>();
                firepit.transform.parent = target.transform;
                firepit.damagePerTick = damagePerTick * damageModifier;
                firepit.durationSeconds = durationSeconds;
                firepit.tickMs = tickMs;
            }

        }
    }

    public override void BeforeHit(Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null)
    {
        damageModifier = spellLauncher == null ? 1f : spellLauncher.GetDamageMultiplierr();
    }


    public override PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
      "Firepit",
       "The item this is slotted into sets any enemy it hits on fire. Enemies close to the target on fire also take damage. Spell damage improves the fire damage.",
       "On Hit Effect",
       new Dictionary<string, string> {
             { "Fire damage per tick", Math.Round(damagePerTick, 2).ToString()},
             { "Tick rate", Math.Round(1000f/tickMs,2).ToString() + " times per second"},
             { "Fire duration", Math.Round(durationSeconds, 2).ToString() + " seconds" }});
    }
    
      public override void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                damagePerTick *= 1.15f + Random.Range(-0.05f, 0.08f);
                durationSeconds *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.RARE:
                damagePerTick *= 1.25f + Random.Range(-0.05f, 0.1f);
                durationSeconds *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.LEGENDARY:
                damagePerTick *= 1.5f + Random.Range(-0.05f, 0.25f);
                durationSeconds *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }
}


