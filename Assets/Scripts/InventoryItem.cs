
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    public IPickable GetDrop();

    public GameObject GetImplementingGameObject();

    public PanelInfo GetPanelInfo();

    public void SetPosition(int i);

    public int GetPosition();

}

public class PanelInfo
{
    public string name;
    public string description;
    public string category;
    public Dictionary<string, string> stats;
    public EffectSlots slots;

    public PanelInfo(string name, string description, string category, Dictionary<string, string> stats, EffectSlots slots = null)
    {
        this.description = description;
        this.stats = stats;
        this.slots = slots;
        this.category = category;
        this.name = name;
    }

}
