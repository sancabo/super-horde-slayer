using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PickableBehavoiur))]
public abstract class BaseEffect : MonoBehaviour, IInventoryItem, IPickable
{
    private int _position = -1;

    void Start()
    {
        _position = 0;
        GetComponent<PickableBehavoiur>()._associatedDrop = this;
    }


    public void OnPickup(Player player)
    {
        transform.parent.GetComponentInChildren<ParticleSystem>().Stop();
        transform.parent.Find("Shadow").gameObject.SetActive(false);
        GetComponent<PickableBehavoiur>().OnPickup(player, this);
    }

    public PickableBehavoiur GetDropComponent()
    {
        return GetComponent<PickableBehavoiur>();
    }

    public Sprite GetGraphic()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public void TossOnFloor(Player player)
    {
        GetComponent<PickableBehavoiur>().TossOnFloorWithoutRemoving(player.GetRandomWalkablePositon(), this, false, 0.5f);
    }

    public IPickable GetDrop()
    {
        return this;
    }

    public GameObject GetImplementingGameObject()
    {
        return gameObject;
    }

    abstract public PanelInfo GetPanelInfo();

     abstract public Type GetEffectType();

    public void SetPosition(int i)
    {
        _position = i;
    }

    public int GetPosition()
    {
        return _position;
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
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

    public enum Type
    {
        ON_HIT,
        ON_KILL,
        PASSIVE,
        NONE
    }
}
