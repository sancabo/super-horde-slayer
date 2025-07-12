using System;

using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryElement : MonoBehaviour
{
    [SerializeField] internal CharacterInventory _characterPanel;
    [SerializeField] internal IPickable _associatedDrop;
    [SerializeField] internal ItemPanel _hoveringPanel;
    [SerializeField] internal ItemPanel _closeablePanel;

    internal IInventoryItem _associatedItem;



    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            _characterPanel.ClearPointerCarry();
        }
    }
    public void RemoveFromInventory()
    {
        _characterPanel.RemoveItem(_associatedItem.GetPosition());
    }

    public void OnDrop()
    {
        _closeablePanel.Close();
        _associatedDrop.TossOnFloor(FindFirstObjectByType<Player>());
        _characterPanel.RemoveItem(_associatedItem.GetPosition());
    }

    public void OnCombine()
    {
        _characterPanel.SetPointerCarry(_associatedItem);
        _closeablePanel.Close();
    }

    public void OnAssignSpellLauncher()
    {
        //Get
        //Implement
    }

    public void ConsumeSlottable(ISlotHolder holder, BaseEffect effect)
    {
        switch (effect.GetEffectType())
        {
            case BaseEffect.Type.ON_HIT:
                if (!holder.GetComponent()._onHit || holder.GetComponent()._onHitEffects.Count() > 0) return;
                holder.AssignOnHitEffect(effect.gameObject);
                break;
            case BaseEffect.Type.ON_KILL:
                if (!holder.GetComponent()._onKill || holder.GetComponent()._onKillEffects.Count() > 0) return;
                holder.AssignOnKillEffect(effect.gameObject);
                break;
            case BaseEffect.Type.PASSIVE:
                if (!holder.GetComponent()._passive || holder.GetComponent()._passiveEffects.Count() > 0) return;
                holder.AssignPassive(effect.gameObject);
                break;
            default:
                throw new Exception("No known effect type");
        }

        _characterPanel.ClearPointerCarry();
        _characterPanel.RemoveItem(effect.GetPosition());
    }


    public void OnHoverEnter()
    {
        if (!_closeablePanel.IsOpen())
        {
            _hoveringPanel.gameObject.SetActive(true);
            FillHoveringPanel(_associatedItem);
        }
    }

    public void OnHoverExit()
    {
        RemoveAllEntries();
        _hoveringPanel.Close();
    }


    public void OnClick()
    {
        Debug.Log("Inventory clicked element");
        if (_characterPanel.IsCarryingItem())
        {
            Debug.Log("Inventory searching Slot holder behaviour");
            if (_associatedItem.GetImplementingGameObject().TryGetComponent(out SlotHolderBehaviour slotHolder))
            {
                Debug.Log("Inventory: Combining Items");
                ConsumeSlottable(slotHolder.GetInterface(), _characterPanel.GetCarriedItem().GetComponent<BaseEffect>());
            }

        }
        else
        {

            if (_associatedItem.GetImplementingGameObject().TryGetComponent(out BaseEffect effect))
            {
                _closeablePanel.transform.Find("CombineButton").gameObject.SetActive(true);
            }

            if (_associatedItem.GetImplementingGameObject().TryGetComponent(out BaseSpellLauncher spellLauncher))
            {
                //SWOW "ASSING TO R-CLICK" BUTTON
            }

            if (_associatedItem.GetImplementingGameObject().TryGetComponent(out Weapon weapon))
            {
                //SHOW NOTHING ADDITTIONAL
            }
            _closeablePanel.gameObject.SetActive(true);
            _closeablePanel._callingObject = this;
            FindPanelPosition();
        }

    }

    //TODO Esto esta mal acá. Tiene que ir en ItemPanel


    private void FindPanelPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransform rectTransform = _closeablePanel.GetComponent<RectTransform>();
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        Camera uiCamera = null;

        // Use the correct camera for ScreenPointToLocalPointInRectangle
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        RectTransform parentRect = rectTransform.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            mousePosition,
            uiCamera,
            out Vector2 anchoredPos
        );
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = anchoredPos;
    }

    private void FillHoveringPanel(IInventoryItem item)
    {
        _hoveringPanel._callingObject = this;
        TextMeshProUGUI title = _hoveringPanel.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI category = _hoveringPanel.gameObject.transform.Find("CategoryHeader").GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI description = _hoveringPanel.gameObject.transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>();
        title.text = item.GetPanelInfo().name;
        category.text = item.GetPanelInfo().category;
        description.text = item.GetPanelInfo().description;
        int i = 0;
        item.GetPanelInfo().stats.ToList().ForEach(pair =>
        {
            FillStatEntry(i, pair.Key, pair.Value);
            i++;
        });

        var slots = _hoveringPanel.gameObject.transform.Find("Slots").gameObject;
        if (_associatedItem.GetImplementingGameObject().TryGetComponent(out SlotHolderBehaviour slotHolderBehaviour))
        {
            slots.SetActive(true);
            if (slotHolderBehaviour._onHit) slots.transform.Find("OnHit").gameObject.SetActive(true);
            if (slotHolderBehaviour._onKill) slots.transform.Find("OnKill").gameObject.SetActive(true);
            if (slotHolderBehaviour._passive) slots.transform.Find("Passive").gameObject.SetActive(true);
            if (slotHolderBehaviour._onHitEffects.Count() > 0)
            {
                Transform onHit = slots.transform.Find("OnHit");
                ShowSlot(slotHolderBehaviour, onHit, slotHolderBehaviour._onHitEffects[0].GetGameObject());
            }

            if (slotHolderBehaviour._onKillEffects.Count() > 0)
            {
                Transform onKill = slots.transform.Find("OnKill");
                ShowSlot(slotHolderBehaviour, onKill, slotHolderBehaviour._onKillEffects[0].GetGameObject());
            }
            if (slotHolderBehaviour._passiveEffects.Count() > 0)
            {
                Transform passive = slots.transform.Find("Passive");
                ShowSlot(slotHolderBehaviour, passive, slotHolderBehaviour._passiveEffects[0].GetGameObject());
            }
        }
        slots.SetActive(item.GetPanelInfo().slots != null);
        SetRarity(_associatedItem.GetImplementingGameObject().GetComponent<PickableBehavoiur>().GetRarity());

    }

    private void SetRarity(IPickable.Rarity rarity)

    {
        TextMeshProUGUI title = _hoveringPanel.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Image image = title.transform.Find("TitleGlow").GetComponent<Image>();
        TextMeshProUGUI rarityText = _hoveringPanel.gameObject.transform.Find("CategoryHeader").Find("Rarity").GetComponent<TextMeshProUGUI>();
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                rarityText.text = "Common";
                title.color = Color.white;
                image.color = new Color(1, 1, 1, 0.08f);
                break;
            case IPickable.Rarity.UNCOMMON:
                rarityText.text = "Uncommon";
                title.color = new Color(0, 97, 255, 1);
                image.color = new Color(0, 97, 255, 0.08f);

                break;
            case IPickable.Rarity.RARE:
                rarityText.text = "Rare";
                title.color = new Color(255, 108, 0, 1);
                image.color = new Color(255, 108, 0, 0.08f);

                break;
            case IPickable.Rarity.LEGENDARY:
                rarityText.text = "Legendary";
                title.color = new Color(255, 0, 0, 1);
                image.color = new Color(255, 0, 0, 0.08f);
                break;
        }
    }

    private void ShowSlot(SlotHolderBehaviour slotHolderBehaviour, Transform slot, GameObject effectObject)
{
    Sprite slotGraphic = effectObject.GetComponent<PickableBehavoiur>().GetSprite();
    Color slotColor = effectObject.GetComponent<PickableBehavoiur>().GetColor();
    GameObject effectImage = slot.transform.Find("SlotImage").gameObject;
    effectImage.SetActive(true);
    effectImage.GetComponent<Image>().sprite = slotGraphic;
    slotColor.a = 1;
    effectImage.GetComponent<Image>().color = slotColor;
}

    private void FillStatEntry(int position, string keyString, string valueString)
    {
        GameObject statEntry = _hoveringPanel.gameObject.transform.Find("StatsFrame").GetChild(0).GetChild(position).gameObject;
        TextMeshProUGUI key = statEntry.transform.Find("Key").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI value = statEntry.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        key.text = keyString;
        value.text = valueString;
        statEntry.SetActive(true);
        _hoveringPanel.gameObject.transform.Find("Slots").gameObject.SetActive(true);
    }

    private void RemoveAllEntries()
    {
        var statsFrame = _hoveringPanel.gameObject.transform.Find("StatsFrame").GetChild(0);
        int childCount = statsFrame.childCount;
        for (int i = 0; i < childCount; i++)
        {
            statsFrame.GetChild(i).gameObject.SetActive(false);
        }
        var slots = _hoveringPanel.gameObject.transform.Find("Slots");
        slots.Find("OnHit").Find("SlotImage").gameObject.SetActive(false);
        slots.Find("OnKill").Find("SlotImage").gameObject.SetActive(false);
        slots.Find("Passive").Find("SlotImage").gameObject.SetActive(false);
        slots.transform.Find("OnHit").gameObject.SetActive(false);
        slots.transform.Find("OnKill").gameObject.SetActive(false);
        slots.transform.Find("Passive").gameObject.SetActive(false);
        slots.gameObject.SetActive(false);
    }
}
