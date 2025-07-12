using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathClips : MonoBehaviour
{
    [SerializeField] private AudioClip[] _deathClips; // Array to hold death sound clips

    [SerializeField] private AudioClip[] _tauntClips; // Array to hold death sound clips

    public AudioClip GetRandomDeathClip()
    {
        if (_deathClips.Length == 0)
        {
            Debug.LogWarning("No death clips assigned!");
            return null;
        }
        int randomIndex = Random.Range(0, _deathClips.Length);
        return _deathClips[randomIndex];
    }

    public AudioClip GetRandomTauntClip()
    {
        if (_tauntClips.Length == 0)
        {
            Debug.LogWarning("No taunt clips assigned!");
            return null;
        }
        int randomIndex = Random.Range(0, _tauntClips.Length);
        return _tauntClips[randomIndex];
    }
}
