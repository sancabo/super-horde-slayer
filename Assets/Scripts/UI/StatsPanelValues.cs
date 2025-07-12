using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanelValues : MonoBehaviour
{
    [SerializeField] private Player _player;

    void Update()
    {
        WriteValues();
    }

    public void WriteValues()
    {
        var statsMap = _player.ExportStatsMap();
        transform.Find("MaxHp").GetComponent<TextMeshProUGUI>().text = statsMap["MaxHealth"];
        transform.Find("HpRegen").GetComponent<TextMeshProUGUI>().text = statsMap["HealthRegen"];
        transform.Find("AtkDmg").GetComponent<TextMeshProUGUI>().text = statsMap["Damage"];
        transform.Find("AtkSpd").GetComponent<TextMeshProUGUI>().text = statsMap["AttackSpeed"];
        transform.Find("MovSpd").GetComponent<TextMeshProUGUI>().text = statsMap["MoveSpeed"];
        transform.Find("MaxMp").GetComponent<TextMeshProUGUI>().text = statsMap["MaxMana"];
        transform.Find("MpRegen").GetComponent<TextMeshProUGUI>().text = statsMap["ManaRegen"];
        transform.Find("SpellDmg").GetComponent<TextMeshProUGUI>().text = statsMap["SpellDamage"] == "0" ? "N/A" : statsMap["SpellDamage"];
    }
}
