using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RangedEnemyAttack : MonoBehaviour, IEnemyAttack
{
    
    public GameObject _proyectilePrefab;
    public AudioSource _attackSound;

    public float _attackDamage = 10f;
    public float _attackDamageMultiplier =1f;

    private Player _target;

    void Start()
    {
        _target = FindAnyObjectByType<Player>();
        Assert.IsNotNull(_target, "Player not found on scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestAttack(Vector3 target, Transform targetTransform = null)
    {
        if(target != null) SpawnBullet(target);
        else  SpawnBullet(_target.transform.position);
    }

    private void SpawnBullet(Vector3 position)
    {
        GameObject newObject = Instantiate(_proyectilePrefab, transform.position, Quaternion.identity);
        if (newObject.TryGetComponent(out Bullet bullet))
        {
            Debug.Log($"Enemy: Spawning bullet at {position}.");
            bullet.direction = (position - transform.position).normalized;
            bullet.damage = (int)(_attackDamage * _attackDamageMultiplier);
            newObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Bullet component not found on the bullet prefab.");
        }
    }
}
