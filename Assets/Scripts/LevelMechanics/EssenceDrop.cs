using System;
using System.Collections;
using UnityEngine;

public class EssenceDrop : MonoBehaviour
{
    //How much bigger it grows when its amount is increased.
    private static readonly float STACK_GROW_FACTOR = 0.18f;

    //Limit of how much the sprite can grow.
    private static readonly float GROW_FACTOR_LIMIT = 2.5f;

    //Radius to use in the clean up operation.
    private static readonly float CLEAN_UP_RADIUS = 4f;

    [SerializeField] private int _amount;

    //Time in seconds until a clean up operation is performed.
    [SerializeField] private float _duration;

    //lock to the clean up operation.
    static readonly object locker = new();

    //How much this instance has grown since its creation.
    private float _increasedScale = 0f;

    private Vector3 _originalScale = new(1,1,1);
    private Vector3 _originalScaleParticle = new(1,1,1);


    void Start()
    {
        StartCoroutine(CleanUpTimer(_duration + UnityEngine.Random.Range(0f, 1f)));
        _originalScale = transform.localScale;
        _originalScaleParticle = GetComponentInChildren<ParticleSystem>().transform.localScale;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out Player player))
        {
            player.AwardEssence(_amount);
            GetComponent<AudioSource>().Play();
            transform.GetComponentInChildren<ParticleSystem>().Stop();
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            GetComponent<Collider2D>().enabled = false;
            StopAllCoroutines(); //We don't need the clean up anymore.
            StartCoroutine(WaitAndDestroy());
        }
    }

    //We need to prevent the number of objets to grow indefinetly.
    //So every X seconds, we search for drops that are close together and fuse them into one.
    private IEnumerator CleanUpTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        PerformCleanUp();
    }


    //Search for a nearby Essence drop, transfer this isntance's amount and scale (fusing), and destroy this.
    //If none are close, reset the clean up timer.
    //To prevent fusing into another Essence that's also being cleaned up, we lock the method to prevent race conditions.
    //We can afford the hit in performance since this is not a time-sensitive method.
    void PerformCleanUp()
    {
        lock (locker)
        {
            bool fused = false;
            Collider2D[] _closeObjects = Physics2D.OverlapCircleAll(transform.position, CLEAN_UP_RADIUS, LayerMask.GetMask("Essence"));
            Debug.Log("Essence Drop: XX " + _closeObjects.Length);
            foreach (Collider2D col in _closeObjects)
            {
                //Debug.Log("Essence Drop: XX");
                if (!fused && col.gameObject != gameObject)
                {
                    col.GetComponent<EssenceDrop>().IncreaseStack(_amount, _increasedScale + STACK_GROW_FACTOR);
                    fused = true;
                    Destroy(gameObject);

                }
            }
            if (!fused) StartCoroutine(CleanUpTimer(_duration));
        }
    }
    public void SetAmount(int amount)
    {
        _amount = amount;
    }

    //Increase this Essence Drop amount, and grow a little in size, up to a limit.
    public void IncreaseStack(int amount, float additionalScale = 0f)
    {
        _amount += amount;
        _increasedScale = Math.Clamp(_increasedScale + additionalScale,0f, GROW_FACTOR_LIMIT);
        transform.localScale = _originalScale * (1 + _increasedScale);
        GetComponentInChildren<ParticleSystem>().transform.localScale = _originalScaleParticle * (1 + _increasedScale);
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
