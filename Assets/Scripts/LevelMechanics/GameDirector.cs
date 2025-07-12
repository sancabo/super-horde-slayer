using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Controls all the enemy spawners, their locations, and increases their number as the player levels up.
public class GameDirector : MonoBehaviour
{   private const int MELEE = 0;
    private const int RANGED = 1;
    private const int HEAVY = 2;
    private const int VAMPIRE = 3;
    private const int ELITE = 4;

    [SerializeField] private GameObject _meleeEnemyPrefab;
    [SerializeField] private GameObject _rangedEnemyPrefab;
    [SerializeField] private GameObject _heavyEnemyPrefab;

    [SerializeField] private GameObject _vampireEnemyPrefabFire;
    [SerializeField] private GameObject _vampireEnemyPrefabLight;


    [SerializeField] private GameObject _spawnerPrefab;

    [SerializeField] private Player _player;

    [SerializeField] private float _maxSpawnRadius = 20f;
    [SerializeField] private float _minSpawnRadius = 1.5f;

    [SerializeField] private SpawnerSettings[] _spawnerSettings;

    private SpawnerSettings _bossSettings;
    private SpawnerSettings _bossSettings2;

    private int _random;

    [SerializeField] private bool _enabled = false;

    private int lvl = 1;
    void Start()
    {
        _random = Random.Range(0, 101);
        if (_spawnerSettings == null)
        {
            _spawnerSettings = new SpawnerSettings[8];
            Assert.IsNotNull(_meleeEnemyPrefab, "Enemy Prefab not set");
            Assert.IsNotNull(_rangedEnemyPrefab, "Enemy Prefab not set");
            Assert.IsNotNull(_heavyEnemyPrefab, "Enemy Prefab not set");
            _spawnerSettings[0] = new SpawnerSettings(2, 0, _meleeEnemyPrefab, 100);
            _spawnerSettings[1] = new SpawnerSettings(1, 0, _rangedEnemyPrefab, 100);
            _spawnerSettings[2] = new SpawnerSettings(0, 0, _heavyEnemyPrefab, 100);
            _spawnerSettings[3] = new SpawnerSettings(0, 0, _vampireEnemyPrefabFire, 100, 30);
            _spawnerSettings[4] = new SpawnerSettings(0, 0, _meleeEnemyPrefab, 100, 20, true);
            _spawnerSettings[5] = new SpawnerSettings(0, 0, _rangedEnemyPrefab, 100, 20, true);
            _spawnerSettings[6] = new SpawnerSettings(0, 0, _heavyEnemyPrefab, 100, 20, true);
            _spawnerSettings[7] = new SpawnerSettings(0, 0, _vampireEnemyPrefabLight, 100, 45, true);
            
            _bossSettings = new SpawnerSettings(0, 0, _vampireEnemyPrefabFire, 1, 1, false);
            _bossSettings2 = new SpawnerSettings(0, 0, _vampireEnemyPrefabLight, 1, 1, false);
        }
    }

    void FixedUpdate()
    {
        if (_enabled)
        {
            //Check each spawner Type to se if we need to update them.
            foreach (SpawnerSettings settings in _spawnerSettings)
            {
                GenerateSpawner(settings);
            }

        }


    }

    //Discard all Exhausted spwaners of this type. If we don't meet the desired number of active spawners, create more until we do.
    private void GenerateSpawner(SpawnerSettings settings)
    {
        settings._activeSpawners = settings._activeSpawners.Where(spawner => spawner != null).ToList();
        if (settings._activeSpawners.Count() < settings._startingSpawners + settings._additionalSpawners)
        {
            //Keep track of the new spawner
            settings._activeSpawners.Add(InstantiateSpawner(settings));
        }
        else
        {
            Debug.Log($"Spawners at Max");
        }
    }

    public EnemySpawner InstantiateSpawner(SpawnerSettings settings)
    {
        GameObject newSpawnPoint = Instantiate(_spawnerPrefab, transform.position, transform.rotation, transform);
        Debug.Log($"Creating new Spawner at for enemy type $ {settings._enemyPrefab.name}. Now there are {settings._activeSpawners.Count + 1}");

        //Load all the settings defaults to the new spawner
        EnemySpawner enemySpawner = newSpawnPoint.GetComponent<EnemySpawner>();
        enemySpawner._amountToSpawn = settings._amount;
        enemySpawner._enemyToSpawn = settings._enemyPrefab;
        enemySpawner._spawnCooldown = settings._cooldown;
        enemySpawner._includesElite = settings._includesElite;
        enemySpawner._gameDirector = this;
        newSpawnPoint.SetActive(true);
        return enemySpawner;
    }
    // Get a random location from the spawn points that are close to the player.
    //A random point 10f away from the player. If its not walkable, get closer.
    // It has a minimum radius, so enemies don't spawnn on top of the player
    public Vector3 GetSpawnLocation()
    {
        float searchRadius = _maxSpawnRadius;
        Vector3 playerPos = _player.transform.position;
        Vector3 randomDir = PickPoint(searchRadius);
        NavMeshHit hit;
        //Once we get the random point, we test it against the navigation mesh to see if it's walkable.
        // Check if the random spawn point is inside a collider of an object called "Nube"

        while (!NavMesh.SamplePosition(playerPos + randomDir, out hit, 2f, NavMesh.AllAreas) || isOnCloud(playerPos + randomDir))
        {
            searchRadius -= 0.5f;
            randomDir = PickPoint(searchRadius);
        }

        return hit.position;
    }

    private bool isOnCloud(Vector3 position)
    {
        // Check if the position is inside a collider of an object called "Nube"
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Nube"))
            {
                return true;
            }
        }
        return false;
    }
    private Vector3 PickPoint(float searchRadius)
    {
        Vector3 randomDir = Random.insideUnitSphere * searchRadius;
        if (randomDir.magnitude < _minSpawnRadius)
        {
            randomDir = randomDir.normalized * _minSpawnRadius;
        }

        return randomDir;
    }

    // At each Lvl Up, Increase de desired number of spawners for each enemy type.
    public void LvlUp()
    {
        lvl++;
        if (lvl == 5)
        {
            IncreaseSpawners(RANGED);
            IncreaseSpawners(MELEE);
        }
        if (lvl == 10) IncreaseSpawners(HEAVY);
        if (lvl == 15)
        {
            InstantiateSpawner(_random > 50 ? _bossSettings : _bossSettings2);
            IncreaseSpawners(MELEE);
            IncreaseSpawners(MELEE + ELITE);
        }
        if (lvl == 20)
        {
            IncreaseSpawners(HEAVY);
            IncreaseSpawners(RANGED);
        }
        ;
        if (lvl == 25)
        {
            IncreaseSpawners(HEAVY);
            IncreaseSpawners(RANGED + ELITE);
            if (_random > 50)
            {
                //_bossSettings2._includesElite = true;
                InstantiateSpawner(_bossSettings2);
                _bossSettings2._includesElite = false;
            }
            else
            {
                //_bossSettings._includesElite = true;
                InstantiateSpawner(_bossSettings);
                _bossSettings._includesElite = false;
            }
        }
        if (lvl == 26)
        {
            IncreaseSpawners(VAMPIRE);
        }
        if (lvl == 28)
        {
            IncreaseSpawners(VAMPIRE);
        }
        if (lvl == 29)
        {
            IncreaseSpawners(HEAVY + ELITE);
        }
        if (lvl == 30)
        {
            DecreaseSpawners(HEAVY);
            IncreaseSpawners(HEAVY + ELITE);
        }
        if (lvl == 31)
        {
            DecreaseSpawners(RANGED);
            IncreaseSpawners(RANGED + ELITE);
        }
        if (lvl == 32)
        {
            DecreaseSpawners(RANGED);
            IncreaseSpawners(RANGED + ELITE);
            IncreaseSpawners(VAMPIRE);
        }
        if (lvl == 33)
        {
            DecreaseSpawners(HEAVY);
            IncreaseSpawners(HEAVY + ELITE);
        }
        if (lvl == 34)
        {
            IncreaseSpawners(VAMPIRE);
            DecreaseSpawners(RANGED);
            IncreaseSpawners(RANGED  + ELITE);
        }
        if (lvl == 35)
        {
            IncreaseSpawners(VAMPIRE + ELITE);
        }
        if (lvl == 36)
        { 
            DecreaseSpawners(MELEE);
            IncreaseSpawners(MELEE + ELITE);
        }
        if (lvl == 37)
        {
            DecreaseSpawners(RANGED);
            IncreaseSpawners(RANGED + ELITE);
        }
        if (lvl == 38)
        {
            DecreaseSpawners(HEAVY);
            IncreaseSpawners(HEAVY + ELITE);
        }
        if (lvl == 39)
        {
            DecreaseSpawners(MELEE);
            IncreaseSpawners(MELEE + ELITE);
        }
        if (lvl == 40) IncreaseSpawners(VAMPIRE);
        if (lvl == 41) 
        {
            DecreaseSpawners(HEAVY);
            IncreaseSpawners(HEAVY + ELITE);
        }
        if (lvl == 42) 
        {
            DecreaseSpawners(MELEE);
            IncreaseSpawners(MELEE + ELITE);
        }
        if (lvl == 43) 
        {
            DecreaseSpawners(RANGED);
            IncreaseSpawners(RANGED + ELITE);
        }
        if (lvl == 45) IncreaseSpawners(VAMPIRE);
        if (lvl == 46)
        {
            IncreaseSpawners(VAMPIRE);
            IncreaseSpawners(VAMPIRE + ELITE);
        }
      
        if (lvl == 49)
        {
            IncreaseSpawners(VAMPIRE);
            IncreaseSpawners(VAMPIRE + ELITE);
        }
    }

    private void IncreaseSpawners(int index)
    {
        _spawnerSettings[index]._additionalSpawners = _spawnerSettings[index]._additionalSpawners + 1;
    }

    private void DecreaseSpawners(int index)
    {
        if (_spawnerSettings[index]._additionalSpawners - 1 < 0)
        {
            if (_spawnerSettings[index]._startingSpawners - 1 < 0)
            {
                _spawnerSettings[index]._startingSpawners = 0;
            }
            else
            {
                 _spawnerSettings[index]._startingSpawners =   _spawnerSettings[index]._startingSpawners - 1;
            }
        }
        else
        {
            _spawnerSettings[index]._additionalSpawners = _spawnerSettings[index]._additionalSpawners - 1;
        }
        
    }

    //Contains settings for the creation of a particular type of spawner
    public class SpawnerSettings
    {
        internal int _startingSpawners;
        internal int _additionalSpawners;

        internal List<EnemySpawner> _activeSpawners;

        internal int _amount;
        internal int _cooldown;

        internal bool _includesElite;
        internal GameObject _enemyPrefab;

        public SpawnerSettings([NotNull] int starting, [NotNull] int additional, [NotNull] GameObject meleeEnemyPrefab, int amount = 10, int cooldown = 10, bool includesElite = false)
        {
            _startingSpawners = starting;
            _additionalSpawners = additional;
            _enemyPrefab = meleeEnemyPrefab;
            _amount = amount;
            _cooldown = cooldown;
            _includesElite = includesElite;
            _activeSpawners = new List<EnemySpawner>();
        }
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public bool IsEnabled()
    {
        return _enabled;
    }

    public void InstantiateOnDemand(int selection)
    {
        if (selection == 0) InstantiateSpawner(new SpawnerSettings(0, 0, _meleeEnemyPrefab, 1, 1, false));
        if (selection == 1) InstantiateSpawner(new SpawnerSettings(0, 0, _rangedEnemyPrefab, 1, 1, false));
        if (selection == 2) InstantiateSpawner(new SpawnerSettings(0, 0, _heavyEnemyPrefab, 1, 1, false));
    }
        
}
