using UnityEngine;

public class Autodestruct : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.3f); 
    }

}
