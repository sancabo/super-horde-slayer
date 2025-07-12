

using UnityEngine;

public interface IPassiveEffect
{
    public void Install(SlotHolderBehaviour holder);

    public GameObject GetGameObject();
}