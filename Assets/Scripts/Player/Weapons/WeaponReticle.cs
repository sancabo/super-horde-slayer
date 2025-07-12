using UnityEngine;
using UnityEngine.Assertions;

public class WeaponReticle : MonoBehaviour
{
    public Player player;
    void Start()
    {
        Assert.IsNotNull(player, "Player not found in the scene.");
    }

    void Update()
    {

        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
