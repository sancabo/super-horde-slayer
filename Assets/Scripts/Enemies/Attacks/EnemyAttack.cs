using UnityEngine;

public interface IEnemyAttack
{
    public void RequestAttack(Vector3 target, Transform targetTransform = null);
}
