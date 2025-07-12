using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillableOrb : MonoBehaviour
{
    private Image _orbImage;

    public float _maxAmount = 100;
    void Awake()
    {
        Debug.Log($"AWAKE: {gameObject.name}");
        _orbImage = GetComponent<Image>();
    }
    void Start()
    {
        Debug.Log($"START: {gameObject.name}");
        _orbImage = GetComponent<Image>();
    }

    internal void Substract(int amount)
    {
        _orbImage.fillAmount = Mathf.Clamp(_orbImage.fillAmount - (amount / _maxAmount), 0f, 1f);
    }

    internal void Award(int amount)
    {
        _orbImage.fillAmount = Mathf.Clamp(_orbImage.fillAmount + (amount / _maxAmount), 0f, 1f);
    }

    internal void SetAmountAndFill(int amount)
    {
        _maxAmount = amount;
        _orbImage.fillAmount = 1;
    }

    internal void SetAmountAndDeplete(int amount)
    {
        Debug.Log($"SetAmountAndDeplete: {amount}");
        _maxAmount = amount;
        _orbImage.fillAmount = 0;
    }
}
