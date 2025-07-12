using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;


public class HeavyEnemyAttack : MonoBehaviour, IEnemyAttack
{

    public GameObject _aoePrefab;
    public AudioSource _attackSound;

    public float _attackDamage = 25f;
    public float _attackDamageMultiplier = 1f;

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
        if (target != null) SpawnAoe(target);
        else SpawnAoe(_target.transform.position);
    }

    private void SpawnAoe(Vector3 targetPosition)
    {
        Vector3 directionToPlayer = targetPosition - transform.position;
        //Change magnitude to 2.
        directionToPlayer = directionToPlayer.normalized;
        //Move the aoe 1 unit in the direction of the player
        Vector3 aoePosition = transform.position + directionToPlayer;

        GameObject newObject = Instantiate(_aoePrefab, aoePosition, Quaternion.identity);
        newObject.SetActive(true);
        if (newObject.TryGetComponent(out Collider2D aoeCollider))
        {
            Collider2D[] hitEntities = new Collider2D[10];
            int amount = aoeCollider.OverlapCollider(new ContactFilter2D().NoFilter(), hitEntities);
            for (int i = 0; i < amount; i++)
            {
                if (hitEntities[i].TryGetComponent(out Player player))
                {
                    player.Hurt((int)math.floor(_attackDamage * _attackDamageMultiplier));
                }
            }
        }

        StartCoroutine(FadeAoe(newObject, 1f));
    }

    private IEnumerator FadeAoe(GameObject newObject, float duration)
    {
        float elapsedTime = 0;
        SpriteRenderer renderer = newObject.GetComponent<SpriteRenderer>();
        Color startingColor = Color.white;
        Color targetColor = Color.white;
        if (renderer != null)
        {
            startingColor = renderer.color;
            targetColor = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f);

        }
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                if (renderer != null)
                {
                    renderer.color = Color.Lerp(startingColor, targetColor, elapsedTime / duration);
                }
                yield return null;
            }
            Destroy(newObject);
        }
    }
