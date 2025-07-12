using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//An entity that regularly spawns enemies according to various parameters.
public class EnemySpawner : MonoBehaviour
{
    public float _spawnCooldown = 1f;
    public GameObject _enemyToSpawn;

    private List<GameObject> _enemiesSpawned = new List<GameObject>();

    public int _amountToSpawn = 5;

    internal GameDirector _gameDirector;

    public int _maxSimultaneousEnemies = 3;

    private bool _isOnCooldown = false;

    public bool _includesElite = false;
    void Start()
    {
        _isOnCooldown = true;
        StartCoroutine(SpawnCooldown(_spawnCooldown));
    }



    void FixedUpdate()
    {
        if(_amountToSpawn < 1) {
            Debug.Log($"#####DESTROYING SPAWNER.");
            StopAllCoroutines();
            Destroy(gameObject);
        }
        if(!_isOnCooldown){
            _enemiesSpawned = _enemiesSpawned.Where(enemy => enemy!= null && !enemy.IsDestroyed()).ToList();
            if(_enemiesSpawned.Count() < _maxSimultaneousEnemies) {
                Enemy enemy = SpawnEnemy();
                int attempts = 0;
                while (enemy == null && attempts < 10)
                {
                    enemy = SpawnEnemy();
                    attempts++;
                }
                if (enemy != null)
                {
                    if (_includesElite) enemy.MakeElite();
                    _isOnCooldown = true;
                    StartCoroutine(SpawnCooldown(_spawnCooldown));
                    _enemiesSpawned.Add(enemy.gameObject);
                }
            } else {
                 Debug.Log($"Max enemies of {_enemiesSpawned.Count()} reached. Will not spawn.");
            }  
        } 
    }

    private IEnumerator SpawnCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        Debug.Log($"Cooldown ended, can spawn again.");
        _isOnCooldown = false;
    }

    private Enemy SpawnEnemy()
    {
        try
        {
            _amountToSpawn--;
            /*Vector3 location = _gameDirector.GetSpawnLocation();
            float checkRadius = 0.5f; // Adjust as needed for your enemy size
            Collider2D[] colliders = Physics2D.OverlapCircleAll(location, checkRadius);
            if (colliders.Any(c => c.GetComponentInChildren<Enemy>() != null))
            {
                Debug.Log("ENEMY: Spawn location is occupied. Skipping spawn.");
                return null;
            }*/
            GameObject enemy = Instantiate(_enemyToSpawn, _gameDirector.GetSpawnLocation(), Quaternion.identity);
            Debug.Log($"Instantiating enemy at {enemy.transform.position}.");
            enemy.SetActive(true);
            return enemy.GetComponentInChildren<Enemy>();
        } catch (System.Exception e)
        {
            Debug.Log($"ENEMY: Exception while spawning enemy: {e}");
            return null;
        }
    }
}
