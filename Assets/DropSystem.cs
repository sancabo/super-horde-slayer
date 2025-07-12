using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RarityBucket
{
    [SerializeField] internal IPickable.Rarity _rarity; // e.g., "VeryRare", "Rare", "Common"
    [SerializeField] internal int _itemCount; // Global count of remaining items
}

[System.Serializable]
public class Bucket
{
    [SerializeField] internal IPickable.Rarity _rarity; // References a RarityBucket
    [SerializeField] internal float _selectionProbability; // Probability within drop table
}

[System.Serializable]
public class DropTable
{
    [SerializeField] internal string _tableName; // e.g., "CommonEnemyTable"
    [SerializeField] internal List<Bucket> _buckets; // Buckets for this enemy type
}

public class DropSystem : MonoBehaviour
{
    [SerializeField] internal List<RarityBucket> _rarityBuckets; // Global rarity buckets
    [SerializeField] internal List<DropTable> _dropTables; // Enemy-specific drop tables

    [SerializeField] internal List<GameObject> _prefabs; // Prefabs, assigned in Inspectora

    [SerializeField] internal CharacterInventory _inventory; // Prefabs, assigned in Inspectora


    // Singleton
    public static DropSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

      public GameObject GetDrop(string dropTableName, Vector3 position)
    {
        DropTable table = _dropTables.Find(t => t._tableName == dropTableName);
        if (table == null)
        {
            Debug.LogError($"Drop table {dropTableName} not found!");
            return null;
        }

        float roll = Random.Range(0f, 1f);
        float cumulative = 0f;

        // Select a rarity from the drop table
        foreach (var bucket in table._buckets)
        {
            cumulative += bucket._selectionProbability;
            RarityBucket rarityBucket = _rarityBuckets.Find(rb => rb._rarity == bucket._rarity);
            if (roll <= cumulative && rarityBucket != null && rarityBucket._itemCount > 0)

            {
                rarityBucket._itemCount--; // Deplete the global rarity count
                if (_prefabs.Count > 0)
                {
                    // Select a random prefab with equal probability
                    int prefabIndex = Random.Range(0, _prefabs.Count);
                    GameObject prefab = _prefabs[prefabIndex];
                    if (prefab != null)
                    {
                        // Instantiate at the specified position
                        PickableBehavoiur droppped = Instantiate(prefab, position, Quaternion.identity).GetComponentInChildren<PickableBehavoiur>();
                        droppped.TossOnFloorWithoutRemoving(FindFirstObjectByType<Player>().GetRandomWalkablePositon(2f), droppped.GetInterface().GetItem(), false, 0.4f);
                        droppped._inventory = _inventory;
                        droppped.GetInterface().ApplyRarity(bucket._rarity);
                        droppped._isHoldeable = true;
                        droppped._hasRarity = true;
                        droppped._associatedDrop = droppped.GetInterface();
                        return droppped.gameObject;


                    }
                }
                else
                {
                     return null; // No prefabs available (e.g., "None" rarity)
                }
               
            }
        }

        return null; // All buckets empty
    }
}