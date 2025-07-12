
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableBehavoiur))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SlotHolderBehaviour))]
public abstract class BaseSpellLauncher : MonoBehaviour, ISpellLauncher, ISlotHolder
{
   [SerializeField] internal float _bonusDamage = 1f;

    [SerializeField] internal float _bonusAoe = 1f;

    [SerializeField] internal int _spellCost = 30;

    [SerializeField] internal GameObject _castEffect;

    [SerializeField] internal float _damage = 20f;

    [SerializeField] internal float _castingTime = 0.5f;
    private int _position = -1;

void Start()
    {
        _position = 0;
        GetComponent<PickableBehavoiur>()._associatedDrop = this;
    }
    
 public float GetBonusDamage()
    {
        return _bonusDamage;
    }
    public  float GetBonusAoe()
    {
        return _bonusAoe;
    }
    
        public void SetBonusDamage(float bonusDamage)
    {
        _bonusDamage = bonusDamage;
    }

     public void SetBonusAoe(float bonusAoe)
    {
        _bonusAoe = bonusAoe;
    }

    public  void ApplyBonusBaseDamage(float increase)
    {
        _damage += increase;
    }

    public  void RequestCastingEffect(Transform parent)
    {
        Instantiate(_castEffect, parent);
    }


    public  void ApplyBonusDamage(float bonusDamage)
    {
        _bonusDamage *= bonusDamage;
    }

    public  void ApplyAoeBonus(float aoeBonus)
    {
        _bonusAoe *= aoeBonus;
    }

    public  int GetCost()
    {
        return _spellCost;
    }

    public  float GetCastingTime()
    {
        return _castingTime;
    }

    public  float GetTotalDamage()
    {
        return _damage;
    }
    public  void ApplyBonusDamageFlat(float bonusDamage)
    {
        _bonusDamage += bonusDamage;
    }

    public  void SetCost(int cost)
    {
        _spellCost = cost;
    }


    public IPickable GetDrop()
    {
        return this;
    }


    public PickableBehavoiur GetDropComponent()
    {
        return GetComponent<PickableBehavoiur>();
    }


    public Sprite GetGraphic()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }


    abstract public GameObject GetImplementingGameObject();


    abstract public string GetLauncherName();

    abstract public GameObject GetObject();


    abstract public PanelInfo GetPanelInfo();


    public int GetPosition()
    {
        return _position;
    }

    public void OnPickup(Player player)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        player.SetSpellLauncher(this);
        GetComponent<PickableBehavoiur>().OnPickup(player, this);
    }

    abstract public void RequestLaunch(Vector3 direction, GameObject originator);

    public void SetPosition(int i)
    {
        _position = i;
    }

    public void TossOnFloor(Player player)
    {
        player.DetachSpellLauncher(GetLauncherName());
        GetComponent<PickableBehavoiur>().TossOnFloorWithoutRemoving(player.GetRandomWalkablePositon(), this, false, 0.5f);
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }

    public void AssignOnHitEffect(GameObject onHitEffect)
    {
        GetComponent<SlotHolderBehaviour>().AddOnHitEffect(onHitEffect.GetComponent<BaseOnHitEffect>());
    }

    public void AssignOnKillEffect(GameObject onKillEffect)
    {
        GetComponent<SlotHolderBehaviour>().AddOnKillEffect(onKillEffect.GetComponent<BaseOnKillEffect>());
    }

    public void AssignPassive(GameObject passiveEffect)
    {
        GetComponent<SlotHolderBehaviour>().AddPassiveEffect(passiveEffect.GetComponent<BasePassiveEffect>());
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public SlotHolderBehaviour GetComponent()
    {
        return GetComponent<SlotHolderBehaviour>();
    }

    public abstract void ApplyRarity(IPickable.Rarity rarity);

    public IPickable.Rarity GetRarity()
    {
        return GetComponent<PickableBehavoiur>().GetRarity();
    }

    public IInventoryItem GetItem()
    {
        return this;
    }
}
