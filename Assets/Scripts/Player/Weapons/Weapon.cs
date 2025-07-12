using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This class handles the Player's weapon behaviour.
/// 
/// When activated, after a short wind up, it generates a small AOE that damages enemies.
/// This AOE is generated at the "AttachPoint". Normally, is the player's facing direction.
/// 
/// If a new attack is requested during an ongoing attack, it will be queued and auto-fired after.
/// The queue mechanic allows keeping track of attack Chains.
/// 
/// The attack can be broken, for example: dashes, stuns, etc.
/// </summary>
[RequireComponent(typeof(PickableBehavoiur))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SlotHolderBehaviour))]
public class Weapon : MonoBehaviour, IInventoryItem, IPickable, ISlotHolder
{
    public GameObject _weaponAttachPoint;

    public WeaponPreset _activePreset;
    internal bool _isOnWindUp = false;
    internal bool _isAttacking = false;
    internal bool _isOnChainWindow = false;

    private int _combo = 0;
    private Color[] _comboColors = new Color[] { Color.white, Color.cyan, Color.green };

    private bool _isChainBreakable = false;


    private IEnumerator _waitQueueCoroutine = null;
    private IEnumerator _windUpCoroutine = null;
    private bool _dashQueued = false;
    private bool _attackQueued = false;

    private static Collider2D[] _hitEnemies = new Collider2D[100];

    private GameObject _aoe;
    private int _position = -1;

    private void Start()
    {
        GetComponent<PickableBehavoiur>()._associatedDrop = this;
    }

    public void AttackButtonPressed()
    {
        if (_isOnChainWindow)
        {
            if (!_dashQueued) _attackQueued = true;
        }
        else if (!_isOnWindUp && !_isAttacking)
        {
            StartPerformingAttack();
        }
    }

    //Starts performing the first step of an attack.
    //The attack is divided in three steps: Swing Wind-Up, Attack Effect Excecution, and Chain Waiting Window.
    //They May or May not reflect actual animations, so each has a duration.
    private IEnumerator WindUpCoroutine(float duration)
    {
        Debug.Log($"WEAPON: Attack Windup (combo {_combo})");
        float elapsedTime = 0;
        _isOnWindUp = true;

        InstantiateAttackAoe(_weaponAttachPoint.transform.position - (transform.position - _weaponAttachPoint.transform.position).normalized * 0.8f);

        //Make the Aoe Transparent
        SpriteRenderer renderer = _aoe.GetComponentInChildren<SpriteRenderer>();
        _aoe.GetComponentInChildren<Animator>().SetFloat("attackSpeedBonus", _activePreset._attackSpeedBonus);
        Color originalColor = renderer.color;
        renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.05f);

        //For the duration, make the aoe follow the attach point that has the player's direction
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _aoe.transform.position = _weaponAttachPoint.transform.position - (transform.position - _weaponAttachPoint.transform.position).normalized * 0.4f;
            _aoe.transform.rotation = _weaponAttachPoint.transform.rotation;

            // For the last 1/4 of the duration, it's possible to queue/chain an attack.
            if (!_isOnChainWindow && elapsedTime > 3f * duration / 4f)
            {
                _isOnChainWindow = true;
            }
            yield return null;
        }
        renderer.color = originalColor;
        _isOnWindUp = false;
        //Start Step Two
        StartCoroutine(Attack(_activePreset._attackTime / _activePreset._attackSpeedBonus));
    }

    private void InstantiateAttackAoe(Vector3 position)
    {
        _aoe = Instantiate(_activePreset._areaOfEffect, position, _weaponAttachPoint.transform.rotation);
        float aoeMultipler = _activePreset._aoeMultiplier;
        if (_activePreset._enableCombo) aoeMultipler = _activePreset._aoeMultiplier * (1f + (float)_combo / 15f);
        _aoe.transform.localScale = new Vector3(_aoe.transform.localScale.x, -_aoe.transform.localScale.y, _aoe.transform.localScale.z) * aoeMultipler;
        _aoe.transform.localScale = (1f + (0.1f * _combo)) * _aoe.transform.localScale;
    }


    //Executes the Attack effect, Damaging Enemies
    //The effect may have an animation, so we add duration
    //This Step is not intended to be Interrupted
    private IEnumerator Attack(float duration)
    {
        Debug.Log($"WEAPON: Attack execution (combo {_combo})");
        _isOnChainWindow = true;
        _isChainBreakable = false;
        _isAttacking = true;
        _aoe.GetComponentInChildren<SpriteRenderer>().color = _comboColors[_combo];
        yield return new WaitForSeconds(duration);
        DamageEnemiesInsideAoe();
        Destroy(_aoe);
        _isAttacking = false;
        _waitQueueCoroutine = QueueWindow(_activePreset._chainWindowTime, _activePreset._chainBreakableAfter);
        StartCoroutine(_waitQueueCoroutine);

    }

    //It searches for any Enemy components inside the weapon AOE, and calls their Hurt() method
    private void DamageEnemiesInsideAoe()
    {
        int results = _aoe.GetComponentInChildren<Collider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), _hitEnemies);
        Debug.Log($"WEAPON: Entities hit by weapon aoe: {results}");
        for (int i = 0; i < results; i++)
        {
            if (_hitEnemies[i].gameObject.CompareTag("Enemy"))
            {
                if (_hitEnemies[i].transform.GetChild(0).TryGetComponent(out Enemy enemy))
                {
                    var slotHolder = GetComponent<SlotHolderBehaviour>();
                    int damage;
                    slotHolder._onHitEffects.ForEach(oh => oh.BeforeHit(enemy, this));
                    if (_activePreset._enableCombo) damage = (int)Math.Floor(_activePreset._damage * _activePreset._damageBonus * (1 + (float)_combo / 5f));
                    else damage = (int)Math.Floor(_activePreset._damage * _activePreset._damageBonus);
                    enemy.Hurt(damage, slotHolder._onKillEffects);
                    slotHolder._onHitEffects.ForEach(oh => oh.AfterHit(enemy, this));
                }
            }
        }
    }


    // Lets an attack continue the chain if it's requested within delay + breakableDelay seconds.
    private IEnumerator QueueWindow(float delay, float breakableDelay)
    {
        bool attackChained = false;
        float elapsedTime = 0;
        _isOnChainWindow = true;
        while (elapsedTime < delay + breakableDelay && !attackChained)
        {
            elapsedTime += Time.deltaTime;
            _isChainBreakable = elapsedTime > breakableDelay;
            if (_attackQueued)
            {
                _attackQueued = false;
                _combo = (_combo + 1) % 3;
                StartPerformingAttack();
                attackChained = true;
            }
            yield return null;
        }
        _isOnChainWindow = false;
        if (!attackChained) _combo = 0;
    }

    //Starts the attack windup. We save the couroutine in case we need to stop it.
    private void StartPerformingAttack()
    {
        Debug.Log("WEAPON: Attack initiated!");
        _windUpCoroutine = WindUpCoroutine(_activePreset._windUpTime / _activePreset._attackSpeedBonus);
        GetComponent<AudioSource>().Play();
        StartCoroutine(_windUpCoroutine);
    }


    //Cancels the chain waiting window that happens after an attack.
    //This allows the player to move at full speed immediately
    public void BreakQueue()
    {
        Debug.Log("WEAPON: Queue windowk broken");
        if (_isOnChainWindow && _isChainBreakable)
        {
            if (_waitQueueCoroutine != null) StopCoroutine(_waitQueueCoroutine);
            _attackQueued = false;
            _isChainBreakable = false;
            _isOnChainWindow = false;
            _combo = 0;
        }
    }


    //Inconditional BreakQueue, and it also breaks the Wind-Up Coroutine.
    public void BreakQueueHard()
    {
        Debug.Log("WEAPON: Attack cancelled");
        if (_waitQueueCoroutine != null) StopCoroutine(_waitQueueCoroutine);
        if (_windUpCoroutine != null) StopCoroutine(_windUpCoroutine);
        _isOnWindUp = false;
        _attackQueued = false;
        _isChainBreakable = false;
        _isOnChainWindow = false;
        _combo = 0;

    }

    //Cancels Any Wind-Up action, along with the Chain Window. And cleans up Graphics.
    //This is used to Break the Attack
    internal void BreakWindup()
    {
        if (!_isAttacking)
        {
            BreakQueueHard();
            if (_aoe != null && !_aoe.IsUnityNull() && !_aoe.IsDestroyed()) Destroy(_aoe);
        }
    }

    public void QueueDash()
    {
        if (_isOnChainWindow)
        {
            _dashQueued = true;
            _attackQueued = false;
        }
    }

    public void UnQueueDash()
    {
        _dashQueued = false;
    }

   


    //The following methods perform operations on the given weapon, if it is not null.
    public static void TryQueueDash(Weapon weapon)
    {
        if (weapon != null && !weapon._isAttacking)
        {
            weapon.QueueDash();
        }
    }

    public static void TryGiveAttackSpeed(Weapon weapon, float bonus, bool multiplicative = false)
    {
        if (weapon != null)
        {
            if (multiplicative) weapon._activePreset._attackSpeedBonus *= bonus;
            else weapon._activePreset._attackSpeedBonus += bonus;
        }
    }

    public static void TryGiveAoeBonus(Weapon weapon, float bonus, bool multiplicative = false)
    {
        if (weapon != null)
        {
            if (multiplicative) weapon._activePreset._aoeMultiplier *= bonus;
            else weapon._activePreset._aoeMultiplier += bonus;
        }
    }

    public static void TryGiveDamageBonus(Weapon weapon, float bonus, bool multiplicative = false)
    {
        if (weapon != null)
        {
            if (multiplicative) weapon._activePreset._damageBonus *= bonus;
            else weapon._activePreset._damageBonus += bonus;
        }
    }

    public static float TryGetAttackSpeed(Weapon weapon)
    {
        return weapon == null ? 1f : weapon._activePreset._attackSpeedBonus;
    }

    public static float TryGetDamage(Weapon weapon)
    {
        return weapon == null ? 0f : weapon._activePreset._damage * weapon._activePreset._damageBonus;
    }

    public static float TryGetAnimationDuration(Weapon weapon)
    {
        return weapon == null ? 2f : weapon._activePreset._windUpTime / weapon._activePreset._attackSpeedBonus + weapon._activePreset._attackTime;
    }


    public static bool TestIsAttacking(Weapon weapon)
    {
        return weapon != null && (weapon._isAttacking || weapon._isOnWindUp);
    }

    public static bool TestIsOnChainWindow(Weapon weapon)
    {
        return weapon != null && (weapon._isOnWindUp || weapon._isOnChainWindow);
    }

    public GameObject GetImplementingGameObject()
    {
        return transform.gameObject;
    }

    public PanelInfo GetPanelInfo()
    {
        return new PanelInfo(
        gameObject.name == "AxeDrop" ? "Axe" : "Spear",
         gameObject.name == "AxeDrop" ? "A large double-bladed runic axe. Slow weapon with mediocre damage, but great area of Effect" : "A long sturdy runic spear. Fast high-damage single-target focused weapon",
        "Weapon",
         new Dictionary<string, string> {
             { "Base Damage", Math.Round((float)_activePreset._damage, 2).ToString() },
             { "Base Attack Duration", Math.Round(_activePreset._attackTime + _activePreset._windUpTime, 2).ToString() },
            { "Area Of Effect", gameObject.name == "AxeDrop"? "Big Wide Arc" : "Small Straight Line" } },
            new EffectSlots());
    }


    public void SetPosition(int i)
    {
        _position = i;
    }

    public int GetPosition()
    {
        return _position;
    }

    public void OnPickup(Player player)
    {
        Debug.Log($"PLAYER: Picked up weapon {name}.");
        player.PickedWeaponFlag();
       player._heldWeapon = SwapWeapon(player._heldWeapon, player.GetRandomWalkablePositon(), transform);
        player.AssignWeapon(gameObject.name == "AxeDrop" ? SkillBar.Skill.Axe : SkillBar.Skill.Spear);
        GetComponent<PickableBehavoiur>().OnPickup(player, this);
    }

 internal Weapon SwapWeapon(Weapon heldWeapon, Vector3 discardPosition, Transform player)
    {
        if (heldWeapon != null)
        {
            //CopyUpgradesFrom(heldWeapon);
            //heldWeapon.GetComponent<PickableBehavoiur>().RemoveSelfFromInventory(heldWeapon);
            //heldWeapon.GetComponent<PickableBehavoiur>().TossOnFloorWithoutRemoving(discardPosition, heldWeapon);
            return heldWeapon;
        }
        return this;
    }

        internal Weapon CopyUpgradesFrom(Weapon heldWeapon)
    {
        if (heldWeapon != null)
        {
            _activePreset._aoeMultiplier = heldWeapon._activePreset._aoeMultiplier;
            _activePreset._damageBonus = heldWeapon._activePreset._damageBonus;
            _activePreset._attackSpeedBonus = heldWeapon._activePreset._attackSpeedBonus;
        }
        return this;
    }

    public PickableBehavoiur GetDropComponent()
    {
        return GetComponent<PickableBehavoiur>();
    }

    public Sprite GetGraphic()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public IPickable GetDrop()
    {
        return this;
    }

    public void TossOnFloor(Player player)
    {
        player._heldWeapon = null;
        GetComponent<PickableBehavoiur>().TossOnFloorWithoutRemoving(player.GetRandomWalkablePositon(), this);
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }

    public void AssignOnHitEffect(GameObject onHitEffect)
    {
        GetComponent<SlotHolderBehaviour>().AddOnHitEffect(onHitEffect.GetComponent<BaseOnHitEffect>());
    }

    public void AssignOnKillEffect(GameObject onKillEffect)
    {
        GetComponent<SlotHolderBehaviour>().AddOnKillEffect(onKillEffect.GetComponent<BaseOnKillEffect>());
    }

    public void AssignPassive(GameObject passiveEffect)
    {
        GetComponent<SlotHolderBehaviour>().AddPassiveEffect(passiveEffect.GetComponent<BasePassiveEffect>());
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public SlotHolderBehaviour GetComponent()
    {
        return GetComponent<SlotHolderBehaviour>();
    }

    public void ApplyRarity(IPickable.Rarity rarity)
    {
        switch (rarity)
        {
            case IPickable.Rarity.COMMON:
                break;
            case IPickable.Rarity.UNCOMMON:
                _activePreset._damage = (int)(_activePreset._damage * (1.15f + Random.Range(-0.05f, 0.08f)));
                _activePreset._windUpTime *= 0.9f + Random.Range(-0.05f, 0.05f);
                break;
            case IPickable.Rarity.RARE:
                _activePreset._damage = (int)(_activePreset._damage * (1.25f + Random.Range(-0.05f, 0.1f)));
                _activePreset._windUpTime *= 0.85f + Random.Range(-0.05f, 0.05f);;
                break;
            case IPickable.Rarity.LEGENDARY:
               _activePreset._damage = (int)(_activePreset._damage * (1.4f + Random.Range(-0.05f, 0.2f)));
                _activePreset._windUpTime *= 0.8f + Random.Range(-0.05f, 0.05f);
                break;
        }
        GetComponent<PickableBehavoiur>().ApplyRarity(rarity);
    }

    public IPickable.Rarity GetRarity()
    {
        return GetComponent<PickableBehavoiur>().GetRarity();
    }

    public IInventoryItem GetItem()
    {
        return this;
    }
}