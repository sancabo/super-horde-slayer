using UnityEngine;

public interface IOnKillEffect
{
    public void Trigger(Player player, Enemy target, Weapon weapon = null, ISpellProjectile spellLauncher = null);

    public GameObject GetGameObject();
}