using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Firepit : MonoBehaviour
{
    private Collider2D firepitCollider;

    public float damagePerTick = 1f;
    public float durationSeconds = 10f;
    public float tickMs = 200f;
    public Transform target;
    private void Awake()
    {
        firepitCollider = GetComponent<Collider2D>();
        if (firepitCollider == null)
        {
            Debug.LogError("Firepit requires a Collider2D component.");
        }
        StartCoroutine(ApplyDamage());

    }

    void FixedUpdate()
    {
        if(target != null) transform.position = target.position;
    }

    private void Start()
    {
        Destroy(gameObject, durationSeconds);
    }

    private IEnumerator ApplyDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickMs / 1000f);
            List<Collider2D> results = new();
            firepitCollider.OverlapCollider(new ContactFilter2D(), results);
            foreach (var collider in results)
            {
                if (collider.CompareTag("Enemy"))
                {
                    Enemy enemy = collider.GetComponentInChildren<Enemy>();
                    if (enemy != null)
                    {
                        enemy.Hurt((int)Math.Max(1f, damagePerTick));
                    }
                }
            }
        }
    }
}
