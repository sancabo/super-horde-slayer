using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSeeker : MonoBehaviour, ISpellProjectile
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _aoe;

    [SerializeField] private GameObject _chainSeekerPrefab;

    [SerializeField] private GameObject _chainTrailPrefab;
    private int _damage;

    private float _damageMult = 1f;

    private List<IOnHitEffect> _onHitEffects = new();

    private List<IOnKillEffect> _onKillEffects = new();

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensure the collided object is checked directly for the Enemy component
        Enemy enemy = collision.gameObject.GetComponentInChildren<Enemy>();
        if (enemy != null && !enemy.HasStatus("Electrocuted"))
        {
            _onHitEffects.ForEach(oh => oh.BeforeHit(enemy, null, this));
            enemy.Hurt(_damage, _onKillEffects); // Example damage value, adjust as needed
            enemy.ApplyStatus("Electrocuted");
            _onHitEffects.ForEach(oh => oh.AfterHit(enemy, null, this));
            StartCoroutine(CreateArc(enemy.transform.position));
            Instantiate(_aoe, enemy.transform.position, Quaternion.identity);
            ChainSeeker newChainSeeker = Instantiate(_chainSeekerPrefab, enemy.transform.position, Quaternion.identity).GetComponent<ChainSeeker>();
            newChainSeeker.SetDamage(_damage);
            newChainSeeker.SetOnHitEffects(_onHitEffects);
            GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent further collisions
        }
    }

    private IEnumerator CreateArc(Vector3 endPosition)
    {
        GameObject trail = Instantiate(_chainTrailPrefab, transform.position, Quaternion.identity);
        float duration = 0.14f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            trail.transform.position = Vector3.Lerp(transform.position, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        _damageMult = multiplier;
        _damage = (int)Math.Floor(_damage * multiplier);
    }

    public void SetOnHitEffects(List<IOnHitEffect> onHitEffects)
    {
        _onHitEffects = onHitEffects;
    }

    public int GetDamage()
    {
        return _damage;
    }

    public void SetDammage(int damage)
    {
        _damage = damage;
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
