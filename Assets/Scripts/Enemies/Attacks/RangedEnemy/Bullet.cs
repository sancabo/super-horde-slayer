using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] internal float moveSpeed =  5f;
    public int damage = 10;
    internal Vector2 direction = new Vector2(0, 1f);
    
    void Update()
    {
         direction = direction.normalized;
    }

      void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D component not found on the Bullet object.");
        }
        else if (!Vector2.zero.Equals(direction))
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.Hurt(damage);
            Debug.Log("Player hit by BULLET damage.");
             Destroy(gameObject);
            
        } 
        else if (!collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            Debug.Log("Bullet hit an obstacle");
            Destroy(gameObject);
        }
        
            
    }

    internal void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
