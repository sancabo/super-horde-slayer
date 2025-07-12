using System.Collections;
using UnityEngine;

public class HealthDrop : MonoBehaviour, IPickable
{
    [SerializeField] private float _amount = 20;

    void Start()
    {
        GetDropComponent()._associatedDrop = this;
    }
    public void SetAmount(int amount)
    {
        _amount = amount;
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

    public void OnPickup(Player player)
    {
        player.HealFor(_amount);
        transform.GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(WaitAndDestroy());
        GetComponent<PickableBehavoiur>().OnPickup(player, null);
    }

    public PickableBehavoiur GetDropComponent()
    {
        return GetComponent<PickableBehavoiur>();
    }

    public Sprite GetGraphic()
    {
        throw new System.NotImplementedException();
    }

    public Color GetColor()
    {
        throw new System.NotImplementedException();
    }

    public void TossOnFloor(Player player)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyRarity(IPickable.Rarity rarity)
    {
        
    }

    public IPickable.Rarity GetRarity()
    {
        return IPickable.Rarity.COMMON;
    }

    public IInventoryItem GetItem()
    {
        throw new System.NotImplementedException();
    }
}
