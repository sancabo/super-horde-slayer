using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemButton : MonoBehaviour
{
    [SerializeField] private Player _player;

    internal bool activated = true;
    private Bonfire _bonfire;
    public void GiveUpgradeByInt(int upgradeTypeInt)
    {
        if (Enum.IsDefined(typeof(Player.UpgradeType), upgradeTypeInt))
        {
            _player.GiveUpgrade((Player.UpgradeType)upgradeTypeInt);
            _bonfire.OnClickSideEffect((Player.UpgradeType)upgradeTypeInt);
        }
        else
        {
            Debug.LogWarning("Invalid upgrade type int passed to GiveUpgradeByInt.");
        }
    }

    public void SetBonfire(Bonfire bonfire)
    {
        _bonfire = bonfire;
    }
    private void FixedUpdate()
    {
        GetComponentInChildren<Button>().interactable = activated;
    }
}
