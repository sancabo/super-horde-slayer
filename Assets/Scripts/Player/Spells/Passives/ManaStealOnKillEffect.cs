
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManaStealOnKillEffect : BaseOnKillEffect
{

    private float _restoredPercentage = 2;

    private float _restoredFlat = 2;
    public override PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
       "Mana Siphon",
        "When an enemy dies due to damage dealt by any item this is slotted into, restore some mana.",
        "On Kill Effect",
        new Dictionary<string, string> {
             { $"Mana restored", $"{Math.Round(_restoredPercentage, 2 )}% of max mana + {Math.Round(_restoredFlat, 2)}"}});
    }

    public override void Trigger(Player player, Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null)
    {
        player.HealManaFor((player.GetMaxMana() / (100f/_restoredPercentage)) + _restoredFlat);
    }
    
      public override void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                _restoredPercentage *= 1.15f + Random.Range(-0.05f, 0.08f);
                _restoredFlat *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.RARE:
                _restoredPercentage *= 1.25f + Random.Range(-0.05f, 0.1f);
                _restoredFlat *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.LEGENDARY:
                _restoredPercentage *= 1.5f + Random.Range(-0.05f, 0.25f);
                _restoredFlat *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }
}
