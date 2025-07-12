using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiasmaSpellLauncher : BaseSpellLauncher
{
    [SerializeField] private GameObject _spellPrefab;//
    [SerializeField] internal float _bounsAoe = 1f;
    [SerializeField] internal float _travelSpeed = 5f;
    [SerializeField] Transform _spellSpot;


    override public void RequestLaunch(Vector3 direction, GameObject originator)
    {
        var miasmaCloud = Instantiate(_spellPrefab, transform.position, Quaternion.identity).GetComponent<MiasmaCloud>();
        miasmaCloud.SetOnHitEffects(GetComponent<SlotHolderBehaviour>()._onHitEffects);
        miasmaCloud.SetOnKillEffects(GetComponent<SlotHolderBehaviour>()._onKillEffects);
        miasmaCloud._maxScale *= _bounsAoe;
        miasmaCloud.ApplyDamageMultiplier(_bonusDamage);
    }

    override public GameObject GetObject()
    {
        return gameObject;
    }

    override public string GetLauncherName()
    {
        return "MiasmaSpellLauncher";
    }

    public override GameObject GetImplementingGameObject()
    {
        return gameObject;
    }

    public override PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
       "Miasma",
        "Grants the power to release a dark cloud around you. Enemies Caught will be cursed and take double damage. Cloud radius increases with aoe",
        "Spell",
        new Dictionary<string, string> {
             { "Casting Time", Math.Round(GetCastingTime(),2).ToString() },
            { " Spell Cost",Math.Round((float)_spellCost,2).ToString()},
            {" Base Cloud Radius", "4m" } },
             new EffectSlots());
    }

     public override void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                _bounsAoe *= 1.15f + UnityEngine.Random.Range(-0.05f, 0.08f);
                _castingTime *= 0.9f;
                _spellCost =(int)(_spellCost* 0.85f);
                break;
            case IPickable.Rarity.RARE:
                _bounsAoe *= 1.25f + UnityEngine.Random.Range(-0.05f, 0.1f);
                _castingTime *= 0.75f;
                _spellCost =(int)(_spellCost* 0.7f);
                break;
            case IPickable.Rarity.LEGENDARY:
                _bounsAoe *= 1.5f + UnityEngine.Random.Range(-0.05f, 0.25f);
                _castingTime *= 0.75f;
                _spellCost =(int)(_spellCost* 0.5f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }
}
