using UnityEngine;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] internal CharacterInventory _parent;
    [SerializeField] internal bool _isCloseable;

    internal ItemInventoryElement _callingObject;
    private bool _pointerInside;

    void Start()
    {
        _pointerInside = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_pointerInside)
            {
                Close();
            }
        }
    }

    public void Close()
    {
        transform.Find("CombineButton")?.gameObject?.SetActive(false);
        transform.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return gameObject.activeSelf;
    }

    public void PoinerEnter()
    {
        _pointerInside = true;
    }

    public void PointerExit()
    {
        _pointerInside = false;
        if (!_isCloseable) Close();
    }

    public void DropCallingObject()
    {
        _callingObject.OnDrop();
        if (_isCloseable) Close();
    }

    public void CombineCallingObject()
    {
        _callingObject.OnCombine();
    }     
}
