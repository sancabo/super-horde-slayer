using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

//Used for objects that lay on the floor and can be picked up by the player by walking over them.
//Additionally, these object can also be holdeable, that is: they will appear in the inventory
//This component's game object should implement the "Pickable" interface

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequiredInterface(typeof(IPickable))]
public class PickableBehavoiur : MonoBehaviour
{
    [SerializeField] internal bool _isHoldeable;

    [SerializeField] internal bool _hasRarity;

    [SerializeField] internal CharacterInventory _inventory;
    internal IPickable _associatedDrop;
    // Start is called before the first frame update
    [SerializeField] GameObject _rarityPrefab;
    [SerializeField] IPickable.Rarity _rarity;

    private GameObject _rarityIndicator;

    void Start()
    {
        if (_hasRarity) ApplyRarity(_rarity);
    }
    public void AddToInventory(IInventoryItem item)
    {
        if (_hasRarity) TurnOffEffect();
        if (item.GetImplementingGameObject().TryGetComponent(out SlotHolderBehaviour slotHolderBehaviour))
        {
            slotHolderBehaviour.TurnOffEffects();
        }
        if (_isHoldeable) _inventory.AddToInventory(item);
    }

    public void RemoveSelfFromInventory(IInventoryItem item)
    {
        if (_isHoldeable) _inventory.RemoveItem(item.GetPosition());
    }

    public void TossOnFloorWithoutRemoving(Vector3 targetPosition, IInventoryItem item, bool animate = true, float dropDuration = 2f)
    {
        item.GetImplementingGameObject().GetComponent<Collider2D>().enabled = false;
        if (item.GetImplementingGameObject().TryGetComponent(out SlotHolderBehaviour slotHolderBehaviour))
        {
            slotHolderBehaviour.TurnOnEffects();
        }
        if (_isHoldeable)
        {
            if (animate) Assert.IsNotNull(transform.parent.GetComponent<Animation>());
            transform.parent.parent = null;
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 1;
            GetComponent<SpriteRenderer>().color = color;
            if (transform.parent.Find("Shadow") != null)
            {
                var shadowRenderer = transform.parent.Find("Shadow").GetComponent<SpriteRenderer>();
                color = shadowRenderer.color;
                color.a = 0;
                shadowRenderer.color = color;
                transform.parent.Find("Shadow").gameObject.SetActive(true);
            }
            StartCoroutine(DropToPosition(transform.parent.gameObject, gameObject, targetPosition, animate, dropDuration));



        }
    }

    private IEnumerator DropToPosition(GameObject animationHolder, GameObject colliderHolder, Vector3 targetPosition, bool animate, float duration = 2f)
    {
        if (animate) animationHolder.GetComponent<Animation>().Play();
        float elapsedTime = 0f;
        Vector3 startPosition = animationHolder.transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            animationHolder.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }
        if (_hasRarity) TurnOnEffect();
        try
        {
            colliderHolder.GetComponent<Collider2D>().enabled = true;
            animationHolder.GetComponentInChildren<ParticleSystem>()?.Play();
            colliderHolder.GetComponentInChildren<ParticleSystem>()?.Play();
            colliderHolder.GetComponent<ParticleSystem>()?.Play();


        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

         colliderHolder.GetComponent<Collider2D>().enabled = true;
    }

    public void OnPickup(Player player, IInventoryItem item)
    {
        if (item == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.parent.position = player.transform.position;
            transform.parent.parent = player.transform;
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 0;
            GetComponent<SpriteRenderer>().color = color;
            if (transform.parent.Find("Shadow") != null)
            {
                var shadowRenderer = transform.parent.Find("Shadow").GetComponent<SpriteRenderer>();
                color = shadowRenderer.color;
                color.a = 0;
                shadowRenderer.color = color;
            }
            GetComponent<Collider2D>().enabled = false;
            AddToInventory(item);
        }
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }

    public void ApplyRarity(IPickable.Rarity rarity)
    {
        Debug.Log("ITEM: applying rarity " + rarity);
        if(_rarityIndicator != null)Destroy(_rarityIndicator);
        GameObject rarityIndicator = Instantiate(_rarityPrefab, transform);
        if (_rarityIndicator != null) Destroy(_rarityIndicator);
        _rarityIndicator = rarityIndicator;
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                _rarity = IPickable.Rarity.COMMON;
                break;
            case IPickable.Rarity.UNCOMMON:
                _rarity = IPickable.Rarity.UNCOMMON;
                break;
            case IPickable.Rarity.RARE:
                _rarity = IPickable.Rarity.RARE;
                break;
            case IPickable.Rarity.LEGENDARY:
                _rarity = IPickable.Rarity.LEGENDARY;
                break;
        }
        TurnOnEffect();
    }

    public void TurnOnEffect()
    {
        switch (_rarity)
        {
            case IPickable.Rarity.COMMON:
                _rarityIndicator.transform.Find("CommonIndicator").gameObject.SetActive(true);
                break;
            case IPickable.Rarity.UNCOMMON:

                _rarityIndicator.transform.Find("UncommonIndicator").gameObject.SetActive(true);
                break;
            case IPickable.Rarity.RARE:
                _rarityIndicator.transform.Find("RareIndicator").gameObject.SetActive(true);
                break;
            case IPickable.Rarity.LEGENDARY:
            _rarityIndicator.transform.Find("LegendaryIndicator").gameObject.SetActive(true);
                break;
        }
    }

    public void TurnOffEffect()
    {
        switch (_rarity)
        {
            case IPickable.Rarity.COMMON:
                _rarityIndicator.transform.Find("CommonIndicator").gameObject.SetActive(false);
                break;
            case IPickable.Rarity.UNCOMMON:

                _rarityIndicator.transform.Find("UncommonIndicator").gameObject.SetActive(false);
                break;
            case IPickable.Rarity.RARE:
                _rarityIndicator.transform.Find("RareIndicator").gameObject.SetActive(false);
                break;
            case IPickable.Rarity.LEGENDARY:
             _rarityIndicator.transform.Find("LegendaryIndicator").gameObject.SetActive(false);
                break;
        }
    }

    public IPickable.Rarity GetRarity()
    {
        return _rarity;
    }

    public IPickable GetInterface()
    {
        if (TryGetComponent(out Weapon weapon))
        {
            return weapon;
        }
        else if (TryGetComponent(out BaseSpellLauncher launcher))
        {
            return launcher;
        }
        else if (TryGetComponent(out BaseEffect effect))
        {
            return effect;
        }
        else
        {
            throw new Exception("Interface IPickable not found on GameObject");
        }
        
    }
}
