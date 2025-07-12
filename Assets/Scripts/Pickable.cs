using UnityEngine;

//Used for objects that lay on the floor and can be picked up by the player by walking over them.
//This implementing game object should have the PickableBehavoiur component
public interface IPickable
{

    public void OnPickup(Player player);

    public PickableBehavoiur GetDropComponent();

    public Sprite GetGraphic();

    public Color GetColor();

    public void TossOnFloor(Player player);

    public void ApplyRarity(Rarity rarity);

    public Rarity GetRarity();

    public IInventoryItem GetItem();

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        LEGENDARY
    }

}
