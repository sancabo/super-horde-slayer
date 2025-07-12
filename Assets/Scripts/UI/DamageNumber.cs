using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f;
    private Color _textColor;
    private bool _isCritical = false;
    // Start is called before the first frame update

    private float elapsed;
    void Start()
    {
        if (_isCritical)
        {
            _textColor = Color.red;
            transform.localScale  = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else
        {
             _textColor = GetComponentInChildren<TextMeshProUGUI>().color;
        }
       
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        
        GetComponentInChildren<TextMeshProUGUI>().color = Color.Lerp(_textColor, new Color(1, 1, 1, 0), elapsed / destroyTime);
        transform.position += new Vector3(0, 0.3f, 0) * Time.deltaTime;
    }

    public void SetValue(int value)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
    }

    public void SetCriticalHit()
    {
        _isCritical = true;
    }
}
