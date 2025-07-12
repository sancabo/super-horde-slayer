using System.Collections.Generic;
using UnityEngine;

//Used for objects that lay on the floor and can be picked up by the player by walking over them.
//This implementing game object should have the PickableBehavoiur component
public class SlotHolderBehaviour : MonoBehaviour
{

    internal List<IOnHitEffect> _onHitEffects = new();

    internal List<IOnKillEffect> _onKillEffects = new();

    internal List<IPassiveEffect> _passiveEffects = new();

    [SerializeField] internal GameObject _slotIndicatorPrefab;

    [SerializeField] internal bool _onHit;
    [SerializeField] internal bool _onKill;
    [SerializeField] internal bool _passive;

    private List<GameObject> _currentEffects = new();


    public ISlotHolder GetInterface()
    {
        if (TryGetComponent(out BaseSpellLauncher spellLauncher))
        {
            return spellLauncher;
        }

        else if (TryGetComponent(out Weapon weapon))
        {
            return weapon;
        }
        else
        {
            throw new System.Exception("Cannot obtain a 'SlotHolder' implementation from this SlotHolderBehaviour's game object");
        }
    }

    public void AddOnHitEffect(IOnHitEffect effect)
    {
        _onHitEffects.Add(effect);
        AddEffect(effect.GetGameObject());
    }

    public void AddOnKillEffect(IOnKillEffect effect)
    {
        _onKillEffects.Add(effect);
        AddEffect(effect.GetGameObject());
    }

    public void AddPassiveEffect(IPassiveEffect effect)
    {
        _passiveEffects.Add(effect);
        effect.Install(this);
        AddEffect(effect.GetGameObject());
    }


    private void AddEffect(GameObject effect)
    {
        GameObject indicator = Instantiate(_slotIndicatorPrefab, gameObject.transform);
        //indicator.transform.position = indicator.transform.position + new Vector3(0, Random.Range(-0.5f, 0.5f), 0);
        //indicator.transform.localScale = indicator.transform.localScale + new Vector3(Random.Range(-0.1f, 0.3f), 0, 0);
        Color color = effect.GetComponent<PickableBehavoiur>().GetColor();
        color.a = 1;
        indicator.GetComponentInChildren<SpriteRenderer>().color = color;
        indicator.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TrailRenderer>().startColor = color;
        indicator.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TrailRenderer>().endColor = color;
        _currentEffects.Add(indicator);
        TurnOffEffects();
    }

    public void TurnOnEffects()
    {
        _currentEffects.ForEach(e => e.SetActive(true));
        _currentEffects.ForEach(e => e.GetComponent<Animator>().SetFloat("offset", Random.Range(0f, 1f)));
    }

    public void TurnOffEffects()
    {
        _currentEffects.ForEach(e => e.SetActive(false));
    }
}
