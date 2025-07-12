using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

//Player entity. It can move with WASD, attack with left mouse, cast spell with right mouse, and dash with space
//To attack or cast spells, it needs the appropiate item.
//It can be hurt, and has hit points.
//Gains levels if it has a Leveling System assigned.
public class Player : MonoBehaviour
{
    private const float DASH_DISTANCE = 5f;
    private const float DAMAGE_PROTECTION_DURATION = 0.5f;
    private const float DASH_SPEED = 20f;
    [Header("Base Stats")]
    [SerializeField] private float _moveSpeed, _moveSpeedMultiplier;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _baseHealth, _maxHealthMultiplier;
    [SerializeField] private float _healthRegen, _healthRegenMultiplier;
    [SerializeField] private float _baseMana, _baseManaMultiplier;

    [SerializeField] private float _manaRegen, _manaRegenMultiplier;

    [Header("State")]
    [SerializeField] private bool _vulnerable;
    [SerializeField] private Level _levelMechanics;
    [SerializeField] private AttributeButtons _attributeButtons;
    [SerializeField] private AttributePanelValues _attributePanelValues;

    [Header("Items and Drops")]
    [SerializeField] internal Weapon _heldWeapon;
    [SerializeField] internal int _essence;
    [SerializeField] internal GameObject _nullSpellLauncherPrefab;

    [Header("Visual Effects")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Transform _weaponAttachPoint;
    [SerializeField] private GameObject _lvlUpEffect;
    [SerializeField] private SkillBar _skillBar;

    private ISpellLauncher[] _heldSpellLaunchers;
    private ISpellLauncher[] _nullSpellLaunchers;
    private bool _dashing = false;
    private bool _dashOnCooldown = false;
    private int _maxHealth;
    private int _currentHealth;
    private int _maxMana;
    private int _currentMana;
    private float _regenTickCounterSeconds = 0f;
    private bool _controlsEnabled = true;
    private bool _multiFire = false;
    internal Vector2 _direction = Vector2.up;
    internal Vector2 _lastKnownDirection = Vector2.up;
    private IEnumerator _runningDisplayDamageCoroutine;
    private Animator _animator;
    private Animator _shadowAnimator;
    private FillableOrb _lifeBar;
    private FillableOrb _manaBar;
    private bool _hasSignature = false;
    private bool _hasSignatureAdvanced = false;

    private static Dictionary<string, SkillBar.Skill> _skillMap = new()
    {
        { "FireBallSpellLauncher", SkillBar.Skill.Fireball },
        { "LightingSpellLauncher", SkillBar.Skill.Lighting },
        { "MiasmaSpellLauncher", SkillBar.Skill.Miasma },
        { "NullSpellLauncher", SkillBar.Skill.None }
    };

    public enum UpgradeType
    {
        Tanky,
        Fast,
        Bruiser,
        Smart
    }

    // Get Components that are assumed to always be in the sence. Intialize Graphics.
    void Start()
    {
        _lifeBar = GameObject.Find("HpOrb").GetComponent<FillableOrb>();
        _manaBar = GameObject.Find("ManaOrb").GetComponent<FillableOrb>();
        _animator = GetComponent<Animator>();
        _shadowAnimator = transform.Find("AnimatedShadow").GetComponent<Animator>();
        Assert.IsNotNull(_lifeBar, "Life Bar for player Not found!");
        Assert.IsNotNull(_manaBar, "Mana Bar Bar for player Not found!");
        Assert.IsNotNull(_animator, "Animator for player Not found!");
        Assert.IsNotNull(_shadowAnimator, "Shadow Animator for player Not found!");
        _maxHealth = (int)math.floor(_baseHealth * _maxHealthMultiplier);
        _currentHealth = _maxHealth;
        _baseHealth = _maxHealth;
        _maxMana = (int)math.floor(_baseMana * _baseManaMultiplier);
        _currentMana = _maxMana;
        _lifeBar.SetAmountAndFill(_maxHealth);
        _manaBar.SetAmountAndFill(_maxMana);
        AwardEssence(0);
        _heldSpellLaunchers = new ISpellLauncher[4];
        _nullSpellLaunchers = new ISpellLauncher[4];
        _heldSpellLaunchers[0] = Instantiate(_nullSpellLauncherPrefab, _weaponAttachPoint.position, Quaternion.identity, transform).GetComponent<NullSpellLauncher>();
        _heldSpellLaunchers[1] = Instantiate(_nullSpellLauncherPrefab, _weaponAttachPoint.position, Quaternion.identity, transform).GetComponent<NullSpellLauncher>();
        _heldSpellLaunchers[2] = Instantiate(_nullSpellLauncherPrefab, _weaponAttachPoint.position, Quaternion.identity, transform).GetComponent<NullSpellLauncher>();
        _heldSpellLaunchers[3] = Instantiate(_nullSpellLauncherPrefab, _weaponAttachPoint.position, Quaternion.identity, transform).GetComponent<NullSpellLauncher>();
        _nullSpellLaunchers[0] = _heldSpellLaunchers[0];
        _nullSpellLaunchers[1] = _heldSpellLaunchers[1];
        _nullSpellLaunchers[2] = _heldSpellLaunchers[2];
        _nullSpellLaunchers[3] = _heldSpellLaunchers[3];
    }

    //Capture Inputs and adjust animations
    void Update()
    {
        if (Time.timeScale == 0) return; //Pause the game if time is stopped.
        //Handle Movement and feed parameters to the Animation State Machine.
        float xComponent = Input.GetAxisRaw("Horizontal");
        float yComponent = Input.GetAxisRaw("Vertical");
        if (!_controlsEnabled)
        {
            xComponent = 0f;
            yComponent = 0f;
        }

        if (!_dashing)
        {
            _direction = new Vector2(xComponent, yComponent).normalized;
            if (_direction != Vector2.zero) _lastKnownDirection = _direction;
        }

        _animator.SetFloat("xComponent", (float)_lastKnownDirection.x);
        _animator.SetFloat("yComponent", (float)_lastKnownDirection.y);
        _shadowAnimator.SetFloat("xComponent", (float)_lastKnownDirection.x);
        _shadowAnimator.SetFloat("yComponent", (float)_lastKnownDirection.y);

        if (_multiFire)
        {
            if (Input.GetMouseButtonDown(1) && _controlsEnabled)
            {
                if (_currentMana >= _heldSpellLaunchers[0].GetCost() + _heldSpellLaunchers[1].GetCost()
                + _heldSpellLaunchers[2].GetCost() + _heldSpellLaunchers[3].GetCost())
                {
                    Array.ForEach(_heldSpellLaunchers, spellLauncher => StartCoroutine(InitiateSpell(spellLauncher)));
                }
            }
        }
        else
        {
            //Handle Spell Button
            if (Input.GetMouseButtonDown(1) && _controlsEnabled) TryRequestSpell(0);
            if (Input.GetKeyDown(KeyCode.Alpha1) && _controlsEnabled) TryRequestSpell(1);
            if (Input.GetKeyDown(KeyCode.Alpha2) && _controlsEnabled) TryRequestSpell(2);
            if (Input.GetKeyDown(KeyCode.Alpha3) && _controlsEnabled) TryRequestSpell(3);
        }

        //Handle Dash Button
        if (Input.GetKeyDown(KeyCode.Space) && _controlsEnabled)
        {
            if (!_dashOnCooldown)
            {
                Weapon.TryQueueDash(_heldWeapon);
                StartCoroutine(Dash());
            }
        }

        //Handle Attack Button
        if (Input.GetMouseButtonDown(0) && _controlsEnabled && _heldWeapon != null)
        {
            _heldWeapon.AttackButtonPressed();
        }

        //AdjustAnimations
        _animator.SetBool("isAttacking", Weapon.TestIsAttacking(_heldWeapon));
        _animator.SetFloat("lookAngle", CalculateLookDirection());
        _animator.SetFloat("attackSpeed", 2f / Weapon.TryGetAnimationDuration(_heldWeapon));
        _shadowAnimator.SetBool("isAttacking", Weapon.TestIsAttacking(_heldWeapon));
        _shadowAnimator.SetFloat("lookAngle", CalculateLookDirection());
        _shadowAnimator.SetFloat("attackSpeed", 2f / Weapon.TryGetAnimationDuration(_heldWeapon));
        _animator.SetFloat("currentSpeed", _direction.magnitude);
    }

    private void TryRequestSpell(int position)
    {
        if (_currentMana >= _heldSpellLaunchers[position].GetCost())
        {
            StartCoroutine(InitiateSpell(_heldSpellLaunchers[position]));

        }
    }
    private IEnumerator InitiateSpell(ISpellLauncher spellLauncher)
    {
        float elapsed = 0f;
        _controlsEnabled = false;
        spellLauncher.RequestCastingEffect(transform);
        while (elapsed < spellLauncher.GetCastingTime())
        {
            elapsed += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, elapsed / spellLauncher.GetCastingTime());
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = _normalColor;
        spellLauncher.RequestLaunch(_weaponAttachPoint.position, gameObject);
        _currentMana -= spellLauncher.GetCost();
        _manaBar.Substract(spellLauncher.GetCost());
        _controlsEnabled = true;
    }

    //Calculate the angle of the direction pointing to the weapon origin. Return it in increments of 45, so It gives one of 8 directions.
    private float CalculateLookDirection()
    {
        Vector3 weaponPositon = _weaponAttachPoint.position;
        Vector3 weaponDirection = weaponPositon - transform.position;
        float weaponAngle = (float)(Math.Atan2(weaponDirection.x, weaponDirection.y) * Mathf.Rad2Deg);
        if (weaponAngle < 0) weaponAngle += 360f;
        return weaponAngle;
    }

    //Apply Regens. Update player position acording to its speed and modifiers.
    void FixedUpdate()
    {
        if (Time.timeScale == 0) return;
        _regenTickCounterSeconds += Time.fixedDeltaTime;
        if (_regenTickCounterSeconds > 1f)
        {
            HealFor(_healthRegen * _healthRegenMultiplier);
            HealManaFor(_manaRegen * _manaRegenMultiplier);
            _regenTickCounterSeconds -= 1f;
        }

        if (TryGetComponent(out Rigidbody2D rb))
        {
            if (!_dashing)
            {
                if (Weapon.TestIsOnChainWindow(_heldWeapon))

                {
                    rb.velocity = _direction * _moveSpeed * _moveSpeedMultiplier * 0.15f; // Slow down the player while attacking
                }
                else
                {
                    rb.velocity = _direction * _moveSpeed * _moveSpeedMultiplier;
                }
            }

        }

        _animator.SetFloat("moveSpeed", _moveSpeedMultiplier);
        _shadowAnimator.SetFloat("moveSpeed", _moveSpeedMultiplier);
    }

    //Increase current hp by ampout. It cannot go over the max.
    public void HealFor(float amount)
    {
        Debug.Log($"PLAYER: Healed for {amount}");
        _currentHealth = Math.Clamp(_currentHealth + (int)Math.Floor(amount), 0, _maxHealth);
        _lifeBar.Award((int)amount);
    }

    //Increase current mana by ampout. It cannot go over the max.
    public void HealManaFor(float amount)
    {
        Debug.Log($"PLAYER: Mana Healed for {amount}");
        _currentMana = Math.Clamp(_currentMana + (int)Math.Floor(amount), 0, _maxMana);
        _manaBar.Award((int)Math.Floor(amount));
    }

    //Quickly travel in the direction pointed by the mouse cursor, harmlessly passing through enemies. It has a cooldown.
    //The dash ends if it hits an obstacle.
    private IEnumerator Dash()
    {
        Vector3 facingDirection = (_weaponAttachPoint.position - transform.position).normalized;
        Debug.Log($"Performing Dash. Direction {facingDirection}");
        GetComponentInChildren<TrailRenderer>().emitting = true; //Trail visual effect
        _dashing = true;
        _vulnerable = false;
        _dashOnCooldown = true;
        if (_heldWeapon != null) _heldWeapon.BreakWindup(); //Dash breaks attack.
        float totalDistanceTraveled = 0f;
        Vector2 startPosition = transform.position;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        LayerMask originalMask = rb.excludeLayers;
        rb.excludeLayers = LayerMask.GetMask("Enemy"); //While dashing, pass through enemies

        while (totalDistanceTraveled < DASH_DISTANCE)
        {
            //I cannot delegate the movement to rigidbody, because it might go through walls.
            //So we RayCast ahead each frame to see if we're going to hit a wall. We add a radius to the raycast to give it thickness.
            totalDistanceTraveled = Vector2.Distance(startPosition, transform.position);
            Vector2 movementThisFrame = _moveSpeed * DASH_SPEED * Time.deltaTime * facingDirection.normalized;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.2f, movementThisFrame, movementThisFrame.magnitude, LayerMask.GetMask("Walls", "Impassable"));
            if (hit)
            {
                Debug.Log($"Hit something during Dash: {hit.collider.gameObject.name}");
                break;
            }
            else
            {
                transform.Translate(movementThisFrame, Space.World);
            }
            yield return null;

        }
        rb.excludeLayers = originalMask;
        _dashing = false;
        _vulnerable = true;
        if (_heldWeapon != null) _heldWeapon.UnQueueDash(); //Allow the weapon to attack again.
        GetComponentInChildren<TrailRenderer>().emitting = false;
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(_dashCooldown);
        _dashOnCooldown = false;
    }

    //Increase Essence and update the UI
    public void AwardEssence(int increment)
    {
        Debug.Log($"PLAYER: Gained essence: {increment}.");
        _essence += increment;
        GameObject counter = GameObject.Find("EssenceCounter");
        if (counter != null)
        {
            counter.GetComponent<EssenceText>().SetEssence(_essence);
        }
        else
        {
            Debug.LogWarning("PLAYER: EssenceCounter not found in the scene.");
        }
    }

    //Selects a powerup to increase the player's stats.
    public void AwardPowerUp(int powerUpId)
    {
        _levelMechanics.PlayerUpgradedAttribute();
        switch (powerUpId)
        {
            case 0:
                Debug.Log("PLAYER: Damage up!!!");
                Weapon.TryGiveDamageBonus(_heldWeapon, 0.15f);
                _attributePanelValues.strength++;
                break;
            case 1:
                _moveSpeedMultiplier += 0.03f;
                Debug.Log("PLAYER: Move Speed + 10%!!!");
                Debug.Log("PLAYER: Attack Speed up!!!");
                Weapon.TryGiveAttackSpeed(_heldWeapon, 0.02f);
                _attributePanelValues.dexterity++;
                break;
            case 2:
                Debug.Log("PLAYER: Attack Speed up!!!");
                Weapon.TryGiveAttackSpeed(_heldWeapon, 0.02f);
                break;
            case 3:
                Debug.Log("PLAYER: AOE up !!!");
                Weapon.TryGiveAoeBonus(_heldWeapon, 0.05f);
                break;
            case 4:
                Debug.Log("PLAYER: Max HP and Regen up !!!");
                IncreaseHpByX(0.03f);
                _healthRegen += 0.1f;
                _attributePanelValues.stamina++;
                break;
            case 5:
                Debug.Log("PLAYER: HP Regen up !!!");
                _healthRegen += 0.1f;
                break;
            case 6:
                Debug.Log("PLAYER: Mp Regen up !!!");
                _manaRegen += 0.1f;
                break;
            case 7:
                Debug.Log("PLAYER: Max Mp and Regen up !!!");
                _manaRegen += 0.1f;
                IncreaseManaByX(0.08f);
                _attributePanelValues.soul++;
                break;
            case 8:
                Debug.Log("PLAYER: Spell Power up !!!");
                Array.ForEach(_heldSpellLaunchers, spellLauncher =>
                {
                    spellLauncher.ApplyBonusDamageFlat(0.15f);
                    spellLauncher.ApplyAoeBonus(1.01f);
                });
                _attributePanelValues.intelligence++;
                break;
            default:
                break;
        }
        _attributeButtons.points--;
        _attributePanelValues.WriteValues();
    }

    //Modify max healh. Heal for the amount increased, and reset Life Bar.
    private void IncreaseHpByX(float multIncrease)
    {
        _maxHealthMultiplier += multIncrease;
        int oldMaxHealth = _maxHealth;
        _maxHealth = (int)math.floor(_maxHealthMultiplier * (float)_baseHealth);
        _currentHealth = math.clamp(_currentHealth + _maxHealth - oldMaxHealth, 0, _maxHealth);
        _lifeBar.SetAmountAndFill(_maxHealth);
        _lifeBar.Substract(_maxHealth - _currentHealth);
        Debug.Log($"PLAYER: Max HP + {multIncrease * 100} %!!!");
    }

    //Same as IncreaseHpByX, but for mana.
    private void IncreaseManaByX(float multIncrease)
    {
        _baseManaMultiplier += multIncrease;
        int oldMaxMana = _maxMana;
        _maxMana = (int)math.floor(_baseManaMultiplier * _baseMana);
        _currentMana = math.clamp(_currentMana + _maxMana - oldMaxMana, 0, _maxMana);
        _manaBar.SetAmountAndFill(_maxMana);
        _manaBar.Substract(_maxMana - _currentMana);
        Debug.Log($"PLAYER: Max Mana + {multIncrease * 100} %!!!");
    }

    //Deals damage to the player, updates the UI, and shows a visual effect. Makes the player not take damage for a duration.
    public void Hurt(int damage)
    {
        if (_vulnerable)
        {
            Debug.Log($"PLAYER: Hurt by {damage} damage.");
            _currentHealth -= damage;
            _lifeBar.Substract(damage);
            if (_currentHealth <= 0) HandlePlayerDeath();
            else
            {
                //We don't want two coroutines changing the color at the same time.
                if (_runningDisplayDamageCoroutine != null) StopCoroutine(_runningDisplayDamageCoroutine);
                _runningDisplayDamageCoroutine = DisplayDamage();
                StartCoroutine(_runningDisplayDamageCoroutine);
                StartCoroutine(HitProtection(DAMAGE_PROTECTION_DURATION));
            }

        }
    }

    public void HandlePlayerDeath()
    {
        DisablePlayer();
        _levelMechanics.PlayerDied();
        Debug.Log("PLAYER: is dead.");
    }

    public void DisablePlayer()
    {
        StopAllCoroutines();
        _controlsEnabled = false;
        _vulnerable = false;
        _dashing = false;
        _dashOnCooldown = false;
    }
    //Turn the player red for a short while
    private IEnumerator DisplayDamage(float duration = 0.2f)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(duration);
        GetComponent<SpriteRenderer>().color = _normalColor;
    }

    //Make the player invulnerable for a short while.
    private IEnumerator HitProtection(float duration = 0.2f)
    {
        _vulnerable = false;
        yield return new WaitForSeconds(duration);
        _vulnerable = true;
    }

    //Apply a powerful upgrase to the player's satas. These upgrades are multiplicative.
    public void GiveUpgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Tanky:
                _healthRegenMultiplier += 1.5f;
                IncreaseHpByX(0.5f);
                break;
            case UpgradeType.Fast:
                _moveSpeedMultiplier *= 1.1f;
                Weapon.TryGiveAttackSpeed(_heldWeapon, 1.2f, true);
                _dashCooldown *= 0.8f;
                break;
            case UpgradeType.Bruiser:
                Weapon.TryGiveDamageBonus(_heldWeapon, 1.2f, true);
                Weapon.TryGiveAoeBonus(_heldWeapon, 1.15f, true);
                break;

            case UpgradeType.Smart:
                _manaRegenMultiplier += 1f;
                IncreaseManaByX(0.5f);
                Array.ForEach(_heldSpellLaunchers, spellLauncher =>
                {
                    spellLauncher.ApplyBonusDamageFlat(1.2f);
                    spellLauncher.ApplyAoeBonus(1.1f);
                });
                break;
            default:
                Debug.LogWarning("Invalid upgrade type.");
                break;
        }

        Debug.Log($"PLAYER: Upgrade {upgradeType} applied.");
    }

    //Disable or enable controls.
    public void SetControlsEnabled(bool enabled)
    {
        _controlsEnabled = enabled;
    }

    //Interact with pickable items.
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out PickableBehavoiur drop))
        {
            drop._associatedDrop.OnPickup(this);
        }
    }

    //Select a random point inside the Walkable area. The point must be within a certain radius around the player. 
    public Vector3 GetRandomWalkablePositon(float searchRadius = 4f)
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * searchRadius;
        NavMeshHit hit;
        //Once we get the random point, we test it against the navigation mesh to see if it's walkable.
        while (!NavMesh.SamplePosition(transform.position + randomDir, out hit, 2f, NavMesh.AllAreas))
        {
            searchRadius -= 0.5f;
            randomDir = UnityEngine.Random.insideUnitSphere * searchRadius;
        }
        return hit.position;
    }

    public void SetSpellLauncher(ISpellLauncher spellLauncher)
    {

        SkillBar.Skill skill = _skillMap[spellLauncher.GetLauncherName()];
        int i;
        for (i = 0; i < _heldSpellLaunchers.Length; i++)
        {
            if (_heldSpellLaunchers[i].GetLauncherName() == "NullSpellLauncher")
            {
                _nullSpellLaunchers[i] = _heldSpellLaunchers[i];
                AssignSpellLauncher(i, spellLauncher, skill);
                spellLauncher.GetObject().GetComponent<Collider2D>().enabled = false;
                break;
            }
        }
    }

    private void AssignSpellLauncher(int position, ISpellLauncher spellLauncher, SkillBar.Skill skill)
    {
        if (position >= 4) throw new Exception("Cannot hold more than 4 spell launchers");
        ISpellLauncher detachedLauncher = _heldSpellLaunchers[position];
        if (detachedLauncher != null)
        {
            _heldSpellLaunchers[position] = spellLauncher;
            spellLauncher.SetBonusDamage(detachedLauncher.GetBonusDamage());
            spellLauncher.SetBonusAoe(detachedLauncher.GetBonusAoe());
            if (skill != SkillBar.Skill.None) _skillBar.AssignToPosition(position, skill);
        }

    }

    public void DetachSpellLauncher(string name)
    {
        if (name == "NullSpellLauncher") throw new Exception("Cannot detach a Null Spell Launcher");
        for (int i = 0; i < _heldSpellLaunchers.Length; i++)
        {
            if (_heldSpellLaunchers[i].GetLauncherName() == name)
            {
                
                AssignSpellLauncher(i, _nullSpellLaunchers[i], SkillBar.Skill.None);
                _skillBar.RemoveFromSlot(i);
                i = 5;
            }
        }


    }
    public void LvlUp()
    {
        Instantiate(_lvlUpEffect, transform.position, Quaternion.identity, transform);
        _levelMechanics.PlayerLeveled();
        _attributeButtons.points++;
    }

    public Dictionary<string, string> ExportStatsMap()
    {
        Debug.Log("Exporting Player Stats Map");
        return new Dictionary<string, string>
        {
            { "MoveSpeed", Math.Round(_moveSpeed * _moveSpeedMultiplier, 2).ToString() },
            { "MaxHealth", Math.Round(_baseHealth * _maxHealthMultiplier, 2).ToString() },
            { "HealthRegen", Math.Round(_healthRegen * _healthRegenMultiplier, 2).ToString() },
            { "MaxMana", Math.Round(_baseMana * _baseManaMultiplier, 2).ToString() },
            { "ManaRegen", Math.Round(_manaRegen * _manaRegenMultiplier, 2).ToString() },
            { "AttackSpeed", Math.Round(Weapon.TryGetAttackSpeed(_heldWeapon), 2).ToString() },
            { "Damage", Math.Round(Weapon.TryGetDamage(_heldWeapon), 2).ToString() },
            { "SpellDamage", Math.Round(_heldSpellLaunchers[0].GetTotalDamage(), 2).ToString() }
        };
    }

    public int GetMaxMana()
    {
        return _maxMana;
    }

    public void ApplyOnHitToSpells()
    {
        _hasSignatureAdvanced = true;
    }

    public void ApplyMultiFire()
    {
        Array.ForEach(_heldSpellLaunchers, spellLauncher =>
        {
            spellLauncher.SetCost(spellLauncher.GetCost() / 2);
        });
        _multiFire = true;
        _hasSignatureAdvanced = true;
    }

    public void ApplyCriticalHit()
    {
        _hasSignature = true;
    }

    public void ApplyFireOnHit()
    {
        
        _hasSignature = true;
    }

    public void ApplyManaOnKill()
    {

        _hasSignature = true;
    }

    public void ApplySummon()
    {
        var summon = FindObjectOfType<Summon>(true);
        summon.gameObject.SetActive(true);
        //summon.transform.position = GetRandomWalkablePositon(2f);
        summon.GiveWeaponClone(_heldWeapon);
        summon.GetComponentInChildren<ParticleSystem>().Play();
        _hasSignatureAdvanced = true;
    }

    public int GetLvl()
    {
        if (TryGetComponent(out LevelingSystem system))
        {
            return system.GetLevel();
        }
        return 1;
    }

    public bool HasSignature()
    {
        return _hasSignature;
    }

    public bool HasSignatureAdvanccced()
    {
        return _hasSignatureAdvanced;
    }

    public void PickedWeaponFlag()
    {
        _levelMechanics.PickedWeapon();
    }

    public void AssignWeapon(SkillBar.Skill skill)
    {
        _skillBar.AssignToLeftClick(skill);
    }
}