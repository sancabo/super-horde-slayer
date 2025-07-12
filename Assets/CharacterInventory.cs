using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{

    [SerializeField] private GameObject _itemPrefab;

    [SerializeField] private ItemPanel _defaultHoverPanel;

    [SerializeField] private ItemPanel _defaultCloseablePabel;
    [SerializeField] internal SpriteRenderer _cursorOverlay;
    private GameObject _carriedItem;

    private IEnumerator _carryCoroutine;

    private List<IInventoryItem> _heldItems = new();
    private List<ItemInventoryElement> _heldItemsComponents = new();


    private Transform _grid;


    public void Close()
    {
        _defaultHoverPanel.Close();
        _defaultCloseablePabel.Close();
        ClearPointerCarry();
        gameObject.SetActive(false);
    }
    private readonly object _inventoryLock = new();


    public void AddToInventory(IInventoryItem element)
    {
        lock (_inventoryLock)
        {
            _heldItems.Add(element);
            AddToInventoryInternal(element, DeterminePosition(element));
        }

    }

    private void AddToInventoryInternal(IInventoryItem element, int position)
    {
        element.SetPosition(position);
        _grid = transform.Find("Grid");
        Transform selectedSlot = FindFirstEmptySlot(position);
        ItemInventoryElement itemElement = Instantiate(_itemPrefab, selectedSlot).GetComponent<ItemInventoryElement>();
        itemElement._characterPanel = this;
        itemElement._closeablePanel = _defaultCloseablePabel;
        itemElement._hoveringPanel = _defaultHoverPanel;
        itemElement._associatedDrop = element.GetDrop();
        itemElement._associatedItem = element;
        element.SetPosition(position);
        Sprite itemSprite = element.GetDrop().GetGraphic();
        Image image = itemElement.GetComponent<Image>();
        image.sprite = itemSprite;
        Color color = element.GetDrop().GetColor();
        color.a = 1;
        image.color = color;
        
        _heldItemsComponents.Add(itemElement);
        Debug.Log("Inventory: added item at " + position);

    }

    private int DeterminePosition(IInventoryItem newItem)
    {
        int result = -1;
        int currentPos = 0;
        _heldItems.ForEach(item =>
        {
            if (ReferenceEquals(item, newItem))
            {
                result = currentPos;
            }
            currentPos++;
        });
        Assert.IsTrue(result >= 0, "Error when determining object position");
        return result;
    }

    private void ReorganizeAll()
    {

        Debug.Log($"Inventory:Reorganizing Items");
        _heldItemsComponents.ForEach(c => Destroy(c.gameObject));
        _heldItemsComponents = new();
        _heldItems.ForEach(i => AddToInventoryInternal(i, DeterminePosition(i)));

    }

    public void RemoveItem(int position)
    {
        lock (_inventoryLock)
        {
            Debug.Log($"Inventory: Removing item at {position}");
            _heldItems.RemoveAt(position);
            ReorganizeAll();
        }
    }
    private Transform FindFirstEmptySlot(int position)
    {
        Debug.Log("Inventory: accessing slot " + position);
        return _grid.Find($"InventoryCell ({position})");
    }

    public void SetPointerCarry(IInventoryItem item)
    {
        Debug.Log("Inventory: Setting pointer Carry");
        _carriedItem = item.GetImplementingGameObject();
        _carryCoroutine = TrackMouseMovement(item);
        StartCoroutine(_carryCoroutine);
    }

    public void ClearPointerCarry()
    {
        if (_carryCoroutine != null) StopCoroutine(_carryCoroutine);
        _carryCoroutine = null;
        _cursorOverlay.gameObject.SetActive(false);
        _carriedItem = null;
    }

    private IEnumerator TrackMouseMovement(IInventoryItem item)
    {
        _cursorOverlay.sprite = item.GetDrop().GetGraphic();
        var color = item.GetDrop().GetColor();
        color.a = 1;
        _cursorOverlay.color = color;
        _cursorOverlay.gameObject.SetActive(true);
        while (true)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = _cursorOverlay.transform.position.z;
            _cursorOverlay.transform.position = worldPos;
            yield return null;
        }

    }

    public bool IsCarryingItem()
    {
        return _carriedItem != null;
    }

    public GameObject GetCarriedItem()
    {
        return _carriedItem;
    }
}
