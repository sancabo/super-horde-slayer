using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStatsPanel : MonoBehaviour
{
    private GameStats _gameStats;

    [SerializeField] private TextMeshProUGUI _timeValue;
    [SerializeField] private TextMeshProUGUI _enemiesKilledValue;
    [SerializeField] private TextMeshProUGUI _mostDamageValue;

    void Start()
    {
        _gameStats = FindAnyObjectByType<GameStats>();
        _timeValue.text = $"{_gameStats.PlayTimeHour} : {_gameStats.PlayTimeMinute} : {_gameStats.PlayTimeSecond}";
        _enemiesKilledValue.text = $"{_gameStats.enemiesKilled}";
        _mostDamageValue.text = $"{_gameStats.maxDamageDone}";
    }
}
