using UnityEngine;

//Used for objects that lay on the floor and can be picked up by the player by walking over them.
//This implementing game object should have the PickableBehavoiur component
public interface ISlotHolder
{

    public void AssignOnHitEffect(GameObject onHitEffect);

    public void AssignOnKillEffect(GameObject onKillEffect);

    public void AssignPassive(GameObject passiveEffect);

    public GameObject GetGameObject();

    public SlotHolderBehaviour GetComponent();

}
