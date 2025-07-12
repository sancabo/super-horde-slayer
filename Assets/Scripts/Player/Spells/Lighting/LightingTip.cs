using System;
using System.Collections.Generic;
using UnityEngine;

public class LightingTip : MonoBehaviour, ISpellProjectile
{
    [SerializeField] internal float moveSpeed;
    [SerializeField] internal int _baseDamage = 20;

    [SerializeField] internal float _damageMult = 1f;

    [SerializeField] private GameObject _spellPrefab;

    internal GameObject originator;
    internal Vector2 direction = new(0, 1f);

    private List<IOnHitEffect> _onHitEffects = new();

    private List<IOnKillEffect> _onKillEffects = new();

    void Update()
    {
        direction = direction.normalized;
    }

    void FixedUpdate()
    {
        transform.position = transform.position + moveSpeed * Time.fixedDeltaTime * (Vector3)direction;

        //transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public void SetDamageMult(float mult)
    {
        _damageMult = mult;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

        //Que no funcione por colision. Seleccione con auto aim y aplicar el daño.
        Debug.Log($"Lighting collided with {collision.gameObject.name}");
        if (!collision.gameObject.TryGetComponent(out Player player))
        {
            Enemy enemy = collision.gameObject.GetComponentInChildren<Enemy>();
            if (enemy != null)
            {
                ChainSeeker chainSeeker = Instantiate(_spellPrefab, enemy.transform.position, Quaternion.identity).GetComponent<ChainSeeker>();
                chainSeeker.SetDamage((int)(_damageMult * _baseDamage));
                chainSeeker.SetOnHitEffects(_onHitEffects);
                chainSeeker.SetOnKillEffects(_onKillEffects);
            }
            //GetComponentInChildren<TrailRenderer>().emitting = false;
            //GetComponentInChildren<TrailRenderer>().enabled = false;
            Destroy(gameObject);
        }
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        _baseDamage = (int)Math.Floor(_baseDamage * multiplier);
    }

    public void SetOnHitEffects(List<IOnHitEffect> onHitEffects)
    {
        _onHitEffects = onHitEffects;
    }

    public int GetDamage()
    {
        return _baseDamage;
    }

    public void SetDammage(int damage)
    {
        _baseDamage = damage;
    }

    public void SetOnKillEffects(List<IOnKillEffect> effect)
    {
        _onKillEffects = effect;
    }

    public float GetDamageMultiplierr()
    {
        return _damageMult;
    }
}
