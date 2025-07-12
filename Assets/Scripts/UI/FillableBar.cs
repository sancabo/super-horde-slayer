using System;
using UnityEngine;

public class FillableBar : MonoBehaviour
{

    internal float _maxAmount = 100;
    internal float _currentAmount = 100;

    public float _originalScale = 0.75f;

    public GameObject _lifeBarBackgroundPrefab;

    void Start()
    {
        Debug.Log($"Adjusting life bat scale: {transform.localScale.x} - {transform.localScale.y} - {transform.localScale.z}");
    }

    internal void Substract(int amount){
        _currentAmount = Math.Clamp(_currentAmount - amount, 0, _maxAmount);
        transform.localScale = new Vector3(_originalScale * (_currentAmount / _maxAmount), transform.localScale.y , transform.localScale.z);     
    }

    internal void Award(int amount){
        _currentAmount = Math.Clamp(_currentAmount + amount, 0, _maxAmount);
        transform.localScale = new Vector3(_originalScale * (_currentAmount / _maxAmount), transform.localScale.y , transform.localScale.z);     
    }

    internal void SetAmountAndFill(int amount){
        _maxAmount = amount;
        _currentAmount = _maxAmount;
        Debug.Log($"Set Fillable Bar values: {_maxAmount}");
        if(_originalScale > 0) transform.localScale = new Vector3(_originalScale ,  transform.localScale.y, transform.localScale.z); 
    }

    internal void SetAmountAndDeplete(int amount){
        _maxAmount = amount;
        _currentAmount = 0;
        Debug.Log($"Set Fillable Bar values: {_maxAmount}");
        if(_originalScale > 0) transform.localScale = new Vector3(0f,  transform.localScale.y, transform.localScale.z); 
    }
}
