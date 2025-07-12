using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightingSpellLauncher : BaseSpellLauncher
{
    [SerializeField] private GameObject _spellPrefab;


    [SerializeField] internal float _travelSpeed = 5f;

    [SerializeField] Transform _spellSpot;

    override public void RequestLaunch(Vector3 direction, GameObject originator)
    {
        Debug.Log("SPELL: launched!");
        GameObject spell = Instantiate(_spellPrefab, transform.position, Quaternion.identity);
        LightingTip bullet = spell.GetComponent<LightingTip>();
        bullet.direction = direction - transform.position;
        bullet.SetDamageMult(_bonusDamage);
        bullet.originator = originator;
        bullet.SetOnHitEffects(GetComponent<SlotHolderBehaviour>()._onHitEffects);
        bullet.SetOnKillEffects(GetComponent<SlotHolderBehaviour>()._onKillEffects);
        _damage = bullet._baseDamage;
    }

    override public GameObject GetObject()
    {
        return gameObject;
    }

    override public string GetLauncherName()
    {
        return "LightingSpellLauncher";
    }

    public override GameObject GetImplementingGameObject()
    {
        return gameObject;
    }

    public override PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
      "Chain Lighting",
       "Grants the power to release bouncing lighting that jumps from enemy to enemy. It will not hit the same enemy twice.",
       "Spell",
       new Dictionary<string, string> {
             { "Base Damage", Math.Round(_damage,2).ToString() },
             { "Casting Time", Math.Round(GetCastingTime(),2).ToString() },
            { " Spell Cost" ,Math.Round((float)_spellCost,2).ToString()},
            {" Jump Distance", "2.5m" } },
             new EffectSlots());
    }

     public override void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                _damage *= 1.15f + Random.Range(-0.05f, 0.08f);
                _castingTime *= 0.9f;
                _spellCost =(int)(_spellCost* 0.9f);
                break;
            case IPickable.Rarity.RARE:
                _damage *= 1.25f + Random.Range(-0.05f, 0.1f);
                _castingTime *= 0.75f;
                _spellCost =(int)(_spellCost* 0.8f);
                break;
            case IPickable.Rarity.LEGENDARY:
                _damage *= 1.5f + Random.Range(-0.05f, 0.25f);
                _castingTime *= 0.6f;
                _spellCost =(int)(_spellCost* 0.8f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }
}

