using System.Collections.Generic;
using UnityEngine;

public class NullSpellLauncher : MonoBehaviour, ISpellLauncher
{

    [SerializeField] internal float _bonusDamage = 1f;

    [SerializeField] internal float _bounsAoe = 1f;

    internal List<IOnHitEffect> _OnHitEffects = new();

    internal List<IOnKillEffect> _onKillEffects = new();

    public void RequestLaunch(Vector3 direction, GameObject originator)
    {

    }

    public void ApplyBonusDamage(float bonusDamage)
    {
        _bonusDamage *= bonusDamage;
    }

    public void ApplyAoeBonus(float aoeBonus)
    {
        _bounsAoe *= aoeBonus;
    }

    public void RequestCastingEffect(Transform parent)
    {

    }

    public float GetCastingTime()
    {
        return 0.5f;
    }

    public int GetCost()
    {
        return 0;
    }

    public void AddOnHitEffect(IOnHitEffect effect)
    {
        _OnHitEffects.Add(effect);
    }

    public void AddOnKillEffect(IOnKillEffect effect)
    {
        _onKillEffects.Add(effect);
    }

    public float GetTotalDamage()
    {
        return 0f;
    }

    public void ApplyBonusDamageFlat(float bonusDamage)
    {
        _bonusDamage += bonusDamage;
    }

    public void SetCost(int cost)
    {

    }

    public void MoveToSpot()
    {

    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public string GetLauncherName()
    {
        return "NullSpellLauncher";
    }

    public void OnPickup(Player player)
    {
        throw new System.NotImplementedException();
    }

    public PickableBehavoiur GetDropComponent()
    {
        throw new System.NotImplementedException();
    }

    public Sprite GetGraphic()
    {
        throw new System.NotImplementedException();
    }

    public void TossOnFloor(Player player)
    {
        throw new System.NotImplementedException();
    }

    public IPickable GetDrop()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetImplementingGameObject()
    {
        return gameObject;
    }

    public PanelInfo GetPanelInfo()
    {
        throw new System.NotImplementedException();
    }

    public void SetPosition(int i)
    {
        throw new System.NotImplementedException();
    }

    public int GetPosition()
    {
        throw new System.NotImplementedException();
    }

    public float GetBonusDamage()
    {
        return _bonusDamage;
    }
    
    public float GetBonusAoe()
    {
        return _bounsAoe;
    }

    public void SetBonusDamage(float bonusDamage)
    {
        _bonusDamage = bonusDamage;
    }

    public void SetBonusAoe(float bonusAoe)
    {
        _bounsAoe = bonusAoe;
    }

    public Color GetColor()
    {
        throw new System.NotImplementedException();
    }

    public void ApplyRarity(IPickable.Rarity rarity)
    {
        
    }

    public IPickable.Rarity GetRarity()
    {
        throw new System.NotImplementedException();
    }

    public IInventoryItem GetItem()
    {
        throw new System.NotImplementedException();
    }
}
