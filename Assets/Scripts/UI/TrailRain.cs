using UnityEngine;

public class TrailRain : MonoBehaviour
{
    
    public float angleOfMotion = 0f;
    public float speedOfMotion = 1f;
    void Start()
    {
        Destroy(gameObject, 30f); // Destroy the rain after 5 seconds
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(Mathf.Cos(angleOfMotion * Mathf.Deg2Rad), Mathf.Sin(angleOfMotion * Mathf.Deg2Rad));
        transform.position += (Vector3)(direction * speedOfMotion * Time.deltaTime);
    }
}
