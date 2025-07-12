using UnityEngine;

public class GameOverButtonSupport : MonoBehaviour
{
    [SerializeField] private Animator _trailAnimator;
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTrail()
    {
        _trailAnimator.SetTrigger("start");
    }

    public void DoNothing()
    {
      
    }


}
