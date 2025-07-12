using UnityEngine;
using UnityEngine.Assertions;

public class WeaponPreset : MonoBehaviour
{
    public GameObject _areaOfEffect;

    public float _windUpTime = 0.2f;
    public float _attackTime = 0.15f;
    public float _chainWindowTime = 0.3f;
    public float _chainBreakableAfter = 0.15f;
    //Speeds up both wind up and attack animations.
    public float _attackSpeedBonus = 1.0f;
    public float _damageBonus = 1.0f;
    public int _damage = 10;
    public float _aoeMultiplier = 1f;

    public bool _enableCombo = true;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(_chainBreakableAfter <= _chainWindowTime, "Chain Break must be lower than Chain Time");
    }

}
