using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torus : MonoBehaviour
{

    [SerializeField] private float _anglesPerSecond = 1f;
    
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, _anglesPerSecond * Time.fixedDeltaTime);
    }
}
