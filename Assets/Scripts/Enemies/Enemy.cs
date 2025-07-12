/// <summary>
/// Represents an enemy entity in the game that can chase the player, take damage, attack and be destroyed.
/// It implements a very crude state machine to handle its behavior.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Enemy : MonoBehaviour
{
    private const float ATTACK_DURATION = 0.1f;
    private const float DAMAGE_DISPLAY_DURATION = 0.2f;

    // The attack used by the enemy once is out of cooldown and on range
    private IEnemyAttack _enemyAttack;

    // The cooldown time (in seconds) between enemy attacks.
    public float _attackCooldown = 3f;

    // The color of the enemy when it is in its normal state.
    public Color normalColor = new Color(1f, 0.38f, 0.61f, 1f);

    // The health points of the Enemy.
    public int _maxHealth = 100;

    // The recoil time in seconds after the enemy is hurt
    public float _recoilTime = 0.5f;

    // The attack range of the enemy. This is the distance at which the enemy can attack the player.
    public float _attackRange = 0.5f;

    // The period of time in which the attack animation is interruptable
    public float _windUpDuration = 1.2f;

    // A multiplier for the enemy attack anmiation speed and cooldown
    public float _attackSpeedBonus = 1f;

    // A multiplier for the enemy attack anmiation speed and cooldown
    public int _experienceGranted = 100;

    // The amount of essence the enemy drops.
    public int _essenceGranted = 10;

    // Indicates whether the enemy will be interrupted by the player attacks.
    public bool _attacksHaveArmor = false;

    // Indicates whether the enemy will flee if the player get close to it.
    public bool _shouldFlee = false;

    // How close the Player needs to be to start fleeing
    public float _fleeDistance = 2f;

    // The sound the enemy plays when it dies.
    public AudioSource _deathAudioSource;

    // Visual effect when the enemy is marked as elite
    public GameObject _eliteMarker;

    // Object the enemy drops at death
    [SerializeField] private GameObject _dropPrefab;

    // Object the enemy drops at death
    [SerializeField] private GameObject _healthPrefab;

    // Object the enemy drops at death
    [SerializeField] private GameObject _bloodPrefab;

    [SerializeField] private GameObject _damageTextPrefab;

    [SerializeField] private Level _level;


    private State state = State.Chasing;
    private bool _vulnerable = true;
    private bool _onCooldown = false;
    private int _health = 100;
    private Transform _target;
    private Player _player;
    private NavMeshAgent _navMeshAgent;
    private Vector3 _lockedPostion;
    private FillableBar _lifeBar;
    private Animator _animator;
    private IEnumerator _runningWindupCoroutine = null;
    private IEnumerator _runningDisplayDamageCoroutine = null;
    private Boolean _isElite = false;

    internal Func<int, int> damageHook = (damage) => damage; // Hook to modify the damage dealt to the enemy

    private string _status = "none";

    private Collider2D _collider;
    private bool _nextHitIsCritical;

    private GameStats _gameStats;

    private enum State
    {
        Chasing, //Will walk towards the player using a Path Find Algorithm
        WindUp, //Will stop, lock a position, and prepare an attack on it
        Recoiling, //It will cancel any attack, and disable the enemy for a short time
        Attacking, //The short window in which the enemy's attack effect is generated.
        Dying, //Will play any effects that happen when the enemy's hp reaches 0
        OnCooldown, //Is on cooldown and cannot attack
        Fleeing // Will ignore tha player and only walk towards a predetermined position.
    }

    //  -- LIFECYCLE METHODS -- 

    //Get Components assumed to always be in the scene. Initialize variables and graphics.
    void Start()
    {
        _level = GameObject.Find("Level")?.GetComponent<Level>();
        _target = GameObject.Find("Player")?.transform;
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        _lifeBar = transform.parent.Find("LifeBar")?.gameObject?.GetComponent<FillableBar>();
        _navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
        _enemyAttack = GetComponent<MeleeEnemyAttack>();
        _animator = GetComponent<Animator>();
        _collider = transform.parent.GetComponent<Collider2D>();
        if (_enemyAttack == null) _enemyAttack = GetComponent<RangedEnemyAttack>();
        if (_enemyAttack == null) _enemyAttack = GetComponent<HeavyEnemyAttack>();
        _gameStats = FindAnyObjectByType<GameStats>();

        Assert.IsNotNull(_level, "No Level object found in the scene.");
        Assert.IsNotNull(_lifeBar, "No life Bar object found in the scene.");
        Assert.IsNotNull(_navMeshAgent, "Enemy has no nav agent");
        Assert.IsNotNull(_deathAudioSource, "Enemy has no audio source");
        Assert.IsNotNull(_enemyAttack, "Enemy has no attack");
        Assert.IsNotNull(_animator, "Enemy has no animator");
        Assert.IsTrue(_target != null && _player != null, "No Player components found in the scene.");
        Assert.IsNotNull(_gameStats, "Enemy has no access to Game Stats");

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _health = _maxHealth;
        _lifeBar.SetAmountAndFill(_maxHealth);


        gameObject.layer = LayerMask.NameToLayer("Enemy");

        StartCoroutine(TauntClips());
    }

    //Check if the player is close or not.
    void Update()
    {
        float playerRadius = _target.GetComponent<Collider2D>().bounds.extents.magnitude;
        float distanceToPlayerEdge = Vector3.Distance(transform.position, _target.position) - (_navMeshAgent.radius + playerRadius);
        _animator.SetFloat("moveSpeed", _navMeshAgent.velocity.magnitude);
        _animator.SetFloat("attackSpeedMultiplier", 1.5f / (ATTACK_DURATION / _attackSpeedBonus));
        if (_navMeshAgent.velocity.magnitude > 0)
        {
            float angle = Vector3.Angle(_navMeshAgent.velocity, Vector3.right);
            if (_navMeshAgent.velocity.y < 0) angle = 360 - angle;
            _animator.SetFloat("moveAngle", angle);

        }


        if (_shouldFlee)
        {
            if (distanceToPlayerEdge < _fleeDistance)
            {
                PlayerInMelee();
            }
            else
            {
                PlayerOutOfMelee();

            }
        }

        if (distanceToPlayerEdge < _attackRange)
        {
            PlayerInRange();
        }
        else
        {
            PlayerOutsideRange();

        }
    }

    // -- STATE MACHINE TRANSITIONS () -- 

    private void PlayerInRange()
    {
        switch (state)
        {
            case State.Chasing:
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
                if (!_onCooldown)
                {
                    _runningWindupCoroutine = StartAttackWindUp();
                    StartCoroutine(_runningWindupCoroutine);
                }
                break;
            default:
                break;
        }
    }

    private void PlayerOutsideRange()
    {
        switch (state)
        {
            case State.Chasing:
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_target.position);
                break;
            case State.WindUp:
                InterruptWindUp();
                break;
            default:
                break;
        }
    }

    private void PlayerInMelee()
    {
        switch (state)
        {
            case State.Chasing:
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
                state = State.Fleeing;
                break;
            case State.Fleeing:
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(GameObject.Find("CaveExit").transform.position);
                break;
            default:
                break;
        }
    }

    private void PlayerOutOfMelee()
    {
        switch (state)
        {
            case State.Fleeing:
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
                state = State.Chasing;
                break;
            default:
                break;
        }
    }


    private void WindupFinished()
    {
        switch (state)
        {
            case State.WindUp:
                StartCoroutine(Attack());
                break;
            default:
                break;
        }
    }

    private void RecoilFinished()
    {
        switch (state)
        {
            case State.Fleeing:
            case State.WindUp:
            case State.Chasing:
            case State.Recoiling:
            case State.Dying:
                state = State.Chasing;
                break;
            default:
                break;
        }
    }

    private void EnemyTookDamage()
    {
        switch (state)
        {
            case State.WindUp:
                if (!_attacksHaveArmor)
                {
                    InterruptWindUp();
                    StartCoroutine(Recoil(_recoilTime));
                }
                break;
            case State.Fleeing:
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
                StartCoroutine(Recoil(_recoilTime));
                break;
            case State.Chasing:
                StartCoroutine(Recoil(_recoilTime));
                break;
            default:
                break;
        }
    }

    // COROUTINES

    //Cancel any attack, prevent more attacks for a duration, and show a visual effect.
    private IEnumerator Recoil(float duration)
    {
        state = State.Recoiling;
        float elapsedTime = 0f;
        float wobbleSpeed = 20f;
        float wobbleAngle = 20f;

        while (elapsedTime < duration)
        {
            //SIN = a value between -1 and 1. So it gives us an angle between -10 and 10 degrees.
            float angle = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAngle;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.identity; // Reset rotation
        RecoilFinished();
    }

    private IEnumerator AttackCooldown(float seconds)
    {
        _onCooldown = true;
        yield return new WaitForSeconds(seconds);
        _onCooldown = false;
    }

    //Perform the attack effect dictated by the _enemyAttack object. Show a visual effect
    private IEnumerator Attack()
    {
        _animator.SetTrigger("attack");
        _enemyAttack.RequestAttack(_lockedPostion, _target);
        float elapsedTime = 0f;
        state = State.Attacking;
        _vulnerable = false;
        while (elapsedTime < ATTACK_DURATION / _attackSpeedBonus)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = normalColor;
        _vulnerable = true;
        StartCoroutine(AttackCooldown(_attackCooldown / _attackSpeedBonus));
        state = State.Chasing;
    }

    //Prepare an attack, gradually showing a visual effect.
    private IEnumerator StartAttackWindUp()
    {
        Debug.Log("ENEMY: Charging attack...");
        float elapsedTime = 0f;
        state = State.WindUp;
        _lockedPostion = _target.position;
        while (elapsedTime < (_windUpDuration / _attackSpeedBonus))
        {
            elapsedTime += Time.deltaTime;
            float transitionRatio = elapsedTime / (_windUpDuration / _attackSpeedBonus);
            GetComponent<SpriteRenderer>().color = Color.Lerp(normalColor, Color.red, transitionRatio);
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = normalColor;
        WindupFinished();
        _runningWindupCoroutine = null;
    }

    private IEnumerator DisplayDamage()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(DAMAGE_DISPLAY_DURATION);
        GetComponent<SpriteRenderer>().color = normalColor;
    }


    // EVENT HANDLERS

    //Deal damage to the enemy. Show visual effects and check if it's dead.
    public void NextHitIsCritical()
    {
        _nextHitIsCritical = true;
    }
    public void Hurt(int damage, List<IOnKillEffect> onKillEffects = null)
    {

        HurtInternal( damageHook.Invoke(damage), onKillEffects);
    }

    private void HurtInternal(int damage, List<IOnKillEffect> onKillEffects = null)
    {
        _gameStats.DamageDone(damage);
         Instantiate(_bloodPrefab, transform.position, Quaternion.identity).GetComponent<Animator>().SetFloat("selector", UnityEngine.Random.Range(0f, 5f));
        GameObject damageText = Instantiate(_damageTextPrefab, transform.position, Quaternion.identity);
        damageText.GetComponent<DamageNumber>().SetValue(damage);
        if (_nextHitIsCritical)
        {
            damageText.GetComponent<DamageNumber>().SetCriticalHit();
            _nextHitIsCritical = false;
        }
        if (_vulnerable)
        {
            if (_lifeBar != null) _lifeBar.Substract(damage);
            Debug.Log($"ENEMY: hurt by {damage} damage.");
            _health -= damage;
            if (_health <= 0)
            {
                StopAllCoroutines();
                if (onKillEffects != null)
                {
                    foreach (var effect in onKillEffects)
                    {
                        effect.Trigger(_player, this);
                    }
                }
                Kill();
            }
            else
            {
                if (_runningDisplayDamageCoroutine != null) StopCoroutine(_runningDisplayDamageCoroutine);
                _runningDisplayDamageCoroutine = DisplayDamage();
                StartCoroutine(_runningDisplayDamageCoroutine);
            }
            EnemyTookDamage();
        }
    }

    //Handle the events that happen upon enemy death: visual effects, drops, experience, etc.
    private void Kill()
    {
        _gameStats.EnemyDied();
        if (state != State.Dying)
        {
            //_level.EnemyKilled();
            state = State.Dying;
            if (_animator != null) _animator.SetTrigger("death");
            _deathAudioSource.PlayOneShot(GetComponent<DeathClips>().GetRandomDeathClip());
            _collider.enabled = false;
            Debug.Log($"ENEMY: dead.");
            StopAllCoroutines();
            if (_player.TryGetComponent(out LevelingSystem levelingSystem)) levelingSystem.AwardExperience(_experienceGranted);

            GameObject drop = Instantiate(_dropPrefab, transform.position, Quaternion.identity);
            DropSystem.Instance.GetDrop("basicEnemies", transform.position);
            if (drop.TryGetComponent(out EssenceDrop dropScript))
            {
                if (_isElite)
                {
                    SetDropColor(drop, Color.yellow);
                    Instantiate(_healthPrefab, transform.position, Quaternion.identity).GetComponent<HealthDrop>().SetAmount(20);
                }
                dropScript.SetAmount(_essenceGranted + UnityEngine.Random.Range(-5, 5));
            }
        }
    }

    public void Die()
    {
        transform.parent.GetComponentInChildren<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        transform.parent.Find("Shadow").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        Destroy(gameObject.transform.parent.gameObject);
    }

    //Change the color of the enemy drop.
    private void SetDropColor(GameObject gameObject, Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
        ParticleSystem.MinMaxGradient gradient = gameObject.GetComponentInChildren<ParticleSystem>().colorOverLifetime.color;
        gradient.colorMin = color;
        gradient.colorMax = new Color(color.r, color.g, color.b, 0f);
    }

    //Cancels the preparation of the attack.
    private void InterruptWindUp()
    {
        if (_runningWindupCoroutine != null)
        {
            StopCoroutine(_runningWindupCoroutine);
            _runningWindupCoroutine = null;
        }
        GetComponent<SpriteRenderer>().color = normalColor;
        state = State.Chasing;
        return;
    }//


    //Greatly increase the enemy stats and apply visual effects
    public void MakeElite()
    {
        _isElite = true;
        _attacksHaveArmor = true;
        _attackSpeedBonus += 0.4f;
        _maxHealth *= 10;
        _health = _maxHealth;
        _experienceGranted *= 15;
        _essenceGranted *= 3;
        if (TryGetComponent(out NavMeshAgent navMeshAgent)) navMeshAgent.speed *= 1.4f;
        transform.localScale = transform.localScale * 1.4f;
        Instantiate(_eliteMarker, transform.position, transform.rotation, transform);
    }

    public void ApplyStatus(string status)
    {

        if (status == "Electrocuted")
        {
            if (_status != "Electrocuted")
            {
                _status = status;
                StartCoroutine(Electrocuted(1f));
            }
        }
    }



    public bool HasStatus(string status)
    {
        return _status == status;
    }

    private IEnumerator Electrocuted(float duration)
    {
        //transform.localScale = transform.localScale * 2f;
        yield return new WaitForSeconds(duration);
        //transform.localScale = transform.localScale / 2f;
        _status = "none";
    }


    private IEnumerator TauntClips()
    {
        while (state != State.Dying)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 20f));
            _deathAudioSource.PlayOneShot(GetComponent<DeathClips>().GetRandomTauntClip());
        }
    }

    public void InstantiateDelegate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Instantiate(prefab, position, rotation, transform);
    }

    public float GetRadius()
    {
        if (_navMeshAgent != null)
        {
            return _navMeshAgent.radius * transform.parent.localScale.x;
        }
        else
        {
            return 0.5f; // Default radius if NavMeshAgent is not available
        }
    }
}
