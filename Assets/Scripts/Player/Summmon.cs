using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Summon : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private Weapon _heldWeapon, _spearClone, _axeClone;
    [SerializeField] private Player _player;

    private NavMeshAgent _navMeshAgent;
    private Vector3 _lastKnownDirection;
    private Animator _animator;
    private Transform _targetedEnemy;
    private float _distanceToPlayer, _walkingDirectionAngle, _lastKnownAngle;
    private String _prevState = "READY";


    // Get Components that are assumed to always be in the sence. Intialize Graphics.
    void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Assert.IsNotNull(_animator, "Animator for player Not found!");
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }

    private Transform FindCloseEnemyToPLayer(float searchRadius = 5f)
    {
        Debug.Log($"SUMMON: Searching for enemies");
        Transform _target = null;
        float closestDistance = float.MaxValue;

        for (float radius = 0; radius <= searchRadius; radius += 0.5f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_player.transform.position, searchRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                Debug.Log($"SUMMON:Examining Enemy");
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    Debug.Log($"SUMMON: Found enemy");
                    closestDistance = dist;
                    _target = hit.transform;
                }
            }
            if (_target != null) return _target;
        }
        return null;
    }

    void FixedUpdate()
    {
        _navMeshAgent.speed = _moveSpeed;
        //Debug.Log($"SUMMON: Targeted Enemy: {_targetedEnemy?.name} - Distance to Player: {_distanceToPlayer} - Previous State: {_prevState}");
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("READY"))
        {
            if (_prevState != "READY")
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
                _prevState = "READY";
            }

        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("HUNTING"))
        {
            try
            {
                //_targetedEnemy.transform.localScale = new Vector3(3, 3, 3); // Reset enemy scale to normal
                _lastKnownDirection = transform.position - _targetedEnemy.position;

                if (Vector3.Distance(_targetedEnemy.position, transform.position) < 1f + _targetedEnemy.GetComponentInChildren<Enemy>().GetRadius())
                {
                    _navMeshAgent.ResetPath();
                    _navMeshAgent.isStopped = true;
                    _heldWeapon.AttackButtonPressed();
                }

                else
                {
                    _navMeshAgent.isStopped = false;
                    _navMeshAgent.SetDestination(_targetedEnemy.transform.position);
                    _prevState = "HUNTING";
                }
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("Enemy died while hunting, resetting state.");
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Enemy was destroyed while hunting, resetting state.");
            }
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("RETURNING"))
        {
            _targetedEnemy = null;
            if (_distanceToPlayer > 14f)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
                transform.position = _player.GetRandomWalkablePositon(1f);
                GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_player.transform.position);
                _prevState = "RETURNING";
            }
        }
        else
        {
            throw new Exception($"SUMMON: Unrecognized state");
        }
    }

    void Update()
    {

        _distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position); //EDGE INSTEAD OF CENTER
        if (_targetedEnemy == null) _targetedEnemy = FindCloseEnemyToPLayer();
        _animator.SetFloat("distanceToPLayer", _distanceToPlayer);
        _animator.SetBool("hasTarget", _targetedEnemy != null);
        _walkingDirectionAngle = CalculateLookDirection(_navMeshAgent.velocity);
        if (_navMeshAgent.velocity.magnitude > 0)
        {
            _lastKnownAngle = _walkingDirectionAngle;
        }
        float xComponent = GetComponentEightDirectionsX(_lastKnownAngle).x;
        float yComponent = GetComponentEightDirectionsX(_lastKnownAngle).y;
        _animator.SetFloat("dirX", xComponent);
        _animator.SetFloat("dirY", yComponent);
    }



    private Vector2 GetComponentEightDirectionsX(float angle)
    {
        float directionalizedAngle = (float)Math.Floor(angle / 45f);

        switch (directionalizedAngle)
        {
            case 0:
                return new Vector2(1, 0);
            case 1:
                return new Vector2(0, 1);
            case 2:
                return new Vector2(1, 1);
            case 3:
                return new Vector2(-1, 0);
            case 4:
                return new Vector2(-1, 0);
            case 5:
                return new Vector2(0, -1);
            case 6:
                return new Vector2(0, -1);
            case 7:
                return new Vector2(1, 0);
            default:
                throw new Exception("Unrecognized angle");
        }
    }

    private float CalculateLookDirection(Vector3 direction)
    {
        float angle = Vector3.Angle(direction, Vector3.right);
        if (direction.y < 0) angle = 360 - angle;
        return angle;
    }


    public void GiveWeaponClone(Weapon weapon)
    {
        if (weapon.gameObject.name == "AxeDrop")
        {
            _heldWeapon = _axeClone.CopyUpgradesFrom(weapon);

        }
        else if (weapon.gameObject.name == "SpearDrop")
        {
            _heldWeapon = _spearClone.CopyUpgradesFrom(weapon);
        }
        Weapon.TryGiveDamageBonus(_heldWeapon, 0.33f, true);
    }

    public Vector3 GetWeaponDirection()
    {
        return _lastKnownDirection;
    }
}