using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{

    public enum StatusBarType
    {
        Tanky,
        Brusier,
        Fast,
        Smart
    }

     [SerializeField] private  GameObject _tankyStatusBarPrefab;
    [SerializeField] private GameObject _brusierStatusBarPrefab;
    [SerializeField] private GameObject _fastStatusBarPrefab;
    [SerializeField] private GameObject _smartStatusBarPrefab;

    private Dictionary<StatusBarType, GameObject> _statusBarPrefabs;
    public Dictionary<StatusBarType, GameObject> _statusBarInstances = new Dictionary<StatusBarType, GameObject>();
    private Dictionary<StatusBarType, int> _statusBarCounters = new Dictionary<StatusBarType, int>
    {
        { StatusBarType.Tanky, 0 },
        { StatusBarType.Brusier, 0 },
        { StatusBarType.Fast, 0 },
        { StatusBarType.Smart, 0 }
    };

    public void Start()
    {
        _statusBarPrefabs = new Dictionary<StatusBarType, GameObject>
    {
        { StatusBarType.Tanky, _tankyStatusBarPrefab },
        { StatusBarType.Brusier, _brusierStatusBarPrefab },
        { StatusBarType.Fast, _fastStatusBarPrefab },
        { StatusBarType.Smart, _smartStatusBarPrefab }
    };
    }
    public void AddInstance(Player.UpgradeType playerType)   
    {
        StatusBarType type;
        switch (playerType) {
            case Player.UpgradeType.Tanky:
                type = StatusBarType.Tanky;
                break;
            case Player.UpgradeType.Bruiser:
                type = StatusBarType.Brusier;
                break;
            case Player.UpgradeType.Fast:
                type = StatusBarType.Fast;
                break;
            case Player.UpgradeType.Smart:
                type = StatusBarType.Smart;
                break;
            default:
                Debug.LogError("Invalid player type");
                return;
        }

        GameObject theInstance;
        if (_statusBarInstances.ContainsKey(type))
        {
            theInstance = _statusBarInstances[type];
        }
        else
        {
            theInstance = GenerateInstance(type);
        }
        _statusBarCounters[type]++;
        TextMeshProUGUI text = theInstance.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "X" + _statusBarCounters[type];
        return;
    }

    private GameObject GenerateInstance(StatusBarType type)
    {
        GameObject newInstance = Instantiate(_statusBarPrefabs[type], transform);
        _statusBarInstances[type] = newInstance;
        return newInstance;
    }
}
