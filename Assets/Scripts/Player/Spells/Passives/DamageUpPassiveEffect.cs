using System.Collections.Generic;
using UnityEngine;

public class DamageUpPassiveEffect : BasePassiveEffect
{
    [SerializeField] internal float flatDamageIncrease = 5;

    public override void AddPassiveEffect(Player player, BaseSpellLauncher spellLauncher)
    {
        spellLauncher.ApplyBonusBaseDamage(flatDamageIncrease);
    }

    public override void AddPassiveEffect(Player player, Weapon weapon)
    {
        weapon._activePreset._damage += (int)flatDamageIncrease;
    }

    public override PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
       "Damage Up",
       "The item this is slotted into gains imporved base damage.",
       "Passive Effect",
       new Dictionary<string, string> {
             { "Base Damage Increase", $"+{flatDamageIncrease}" }});
    }
    
     public override void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                flatDamageIncrease *= 1.15f + Random.Range(-0.05f, 0.08f);
                break;
            case IPickable.Rarity.RARE:
                flatDamageIncrease *= 1.25f + Random.Range(-0.05f, 0.1f);
                break;
            case IPickable.Rarity.LEGENDARY:
                flatDamageIncrease *= 1.5f + Random.Range(-0.05f, 0.25f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }
}
