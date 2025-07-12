using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour, ISpellProjectile
{
    [SerializeField] internal float moveSpeed;
    [SerializeField] private GameObject _aoe;
    [SerializeField] private double _accelerationFactor = 4d;

    private List<IOnHitEffect> _onHitEffects = new();

    private List<IOnKillEffect> _onKillEffects = new();

    public int damage = 10;

    public float _multiplier = 1f;
    internal Vector2 direction = new(0, 1f);

    internal float aoeBonus = 1f;

    private bool _hasExploded = false;

    void Start()
    {
        StartCoroutine(Accelerate(_accelerationFactor));
    }
    void Update()
    {
        direction = direction.normalized;
    }


    void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (!Vector2.zero.Equals(direction))
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Fireball collided with {collision.gameObject.name}");
        if (!collision.gameObject.TryGetComponent(out Player player))
        {
            if (!_hasExploded) Explode(direction);
        }

    }
    //
    public void Explode(Vector3 direction)
    {
        GameObject aoe = Instantiate(_aoe, transform.position + (direction.normalized * 1f), Quaternion.identity);
        aoe.transform.localScale *= aoeBonus;
        if (aoe.TryGetComponent(out CircleCollider2D collider))
        {
            Collider2D[] hitEntities = new Collider2D[100];
            int amount = collider.OverlapCollider(new ContactFilter2D().NoFilter(), hitEntities);
//            StartCoroutine(FadeAoe(aoe, 1f));

            for (int i = 0; i < amount; i++)
            {
                if (hitEntities[i].transform.childCount > 0)
                {
                    if (hitEntities[i].transform.GetChild(0).TryGetComponent(out Enemy enemyHit))
                    {
                        _onHitEffects.ForEach(oh => oh.BeforeHit(enemyHit, null, this));
                        enemyHit.Hurt(damage, _onKillEffects);
                        _onHitEffects.ForEach(oh => oh.AfterHit(enemyHit, null, this));
                    }
                }


            }
        }
        _hasExploded = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Stop();
        Destroy(gameObject);
    }
    internal void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private IEnumerator Accelerate(double factor)
    {
        while (true)
        {
            moveSpeed += (float)Math.Pow(1.5f, factor) * Time.deltaTime;
            yield return null;
        }
    }

    public void SetOnHitEffects(List<IOnHitEffect> effects)
    {
        _onHitEffects = effects;
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        _multiplier = multiplier;
        damage = (int)Math.Floor(damage * multiplier);
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDammage(int damage)
    {
        this.damage = damage;
    }
    
    public void SetOnKillEffects(List<IOnKillEffect> effect)
    {
        _onKillEffects = effect;
    }

    public float GetDamageMultiplierr()
    {
        return _multiplier;
    }
}
