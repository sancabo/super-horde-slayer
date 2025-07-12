using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeEnemyAttack : MonoBehaviour, IEnemyAttack
{   
    public float _dash_duration = 0.1f;
    public float _attackSpeedBonus = 1f;
    public float _attackDamage = 15f;
    public float _attackDamageMultiplier =1f;
    public Transform _enemyTransform;
    private Player _target;
    // Start is called before the first frame update
    void Start()
    {
        _target = FindAnyObjectByType<Player>();
        Assert.IsNotNull(_target, "Player not found on scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestAttack(Vector3 lockedInTarget, Transform targetTransform = null)
    {
        if(lockedInTarget!= null) StartCoroutine(MakeAttack(lockedInTarget, targetTransform));
        else StartCoroutine(MakeAttack(_target.transform.position, _target.transform));
    }

    public IEnumerator MakeAttack(Vector3 lockedInTarget,  Transform targetTransform){
         float elapsedTime = 0f;
        bool playerHurt = false;
        while (elapsedTime < _dash_duration / _attackSpeedBonus)
        {
            elapsedTime += Time.deltaTime;
            Vector3 direction = (lockedInTarget - transform.position).normalized;
            _enemyTransform.parent.Translate(direction * 10 * Time.deltaTime);
            if(!playerHurt && transform.parent.GetComponent<Collider2D>().IsTouching(targetTransform.GetComponent<Collider2D>())){
                targetTransform.GetComponent<Player>().Hurt((int)(_attackDamage * _attackDamageMultiplier));
                playerHurt = true;
            }
            yield return null;
        }
    }
}
