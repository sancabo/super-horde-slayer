using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiasmaCloud : MonoBehaviour, ISpellProjectile
{

    [SerializeField] internal float _maxScale = 6f;//

    [SerializeField] internal float _growDurationSeconds = 0.5f;

    [SerializeField] internal GameObject _miasmaIndicator;

    private float _damageMul = 1f;

    private List<IOnHitEffect> _OnHitEffects = new();

    private List<IOnKillEffect> _onKillEffects = new();

    public void ApplyDamageMultiplier(float multiplier)
    {
        _damageMul = multiplier;
    }



    public int GetDamage()
    {
        return 0;
    }

    public float GetDamageMultiplierr()
    {
        return _damageMul;
    }

    public void SetDammage(int damage)
    {

    }

    public void SetOnHitEffects(List<IOnHitEffect> onHitEffects)
    {
        _OnHitEffects = onHitEffects;
    }

    public void SetOnKillEffects(List<IOnKillEffect> effect)
    {
        _onKillEffects = effect;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private float _growTimer = 0f;

    void FixedUpdate()
    {
        if (transform.localScale.x < _maxScale)
        {
            _growTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(_growTimer / _growDurationSeconds);
            float scale = Mathf.Lerp(1f, _maxScale, t);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color startingColor = renderer.color;
        Color targetColor = new Color(startingColor.r, startingColor.g, startingColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            renderer.color = Color.Lerp(startingColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInChildren<Enemy>();
        if (enemy != null)
        {
            _OnHitEffects.ForEach(oh => oh.BeforeHit(enemy, null, this));
            enemy.damageHook = damage => (int)(damage * 1.5f);
            var miasma = Instantiate(_miasmaIndicator);
            miasma.transform.position = enemy.transform.position + new Vector3(0, 1, 0);
            miasma.transform.parent = enemy.transform;
            _OnHitEffects.ForEach(oh => oh.AfterHit(enemy, null, this));
        }
    }
    }
