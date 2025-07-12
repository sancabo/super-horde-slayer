using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributePanelValues : MonoBehaviour
{

    public int strength = 0;
    public int stamina = 0;
    public int intelligence = 0;
    public int dexterity = 0;
    public int soul = 0;

   

    // Update is called once per frame
    void Update()
    {
        WriteValues();
    }
    
    public void WriteValues()
    {
        transform.Find("StrengthValue").GetComponent<TextMeshProUGUI>().text = strength.ToString();
        transform.Find("StaminaValue").GetComponent<TextMeshProUGUI>().text = stamina.ToString();
        transform.Find("DexterityValue").GetComponent<TextMeshProUGUI>().text = dexterity.ToString();
        transform.Find("SoulValue").GetComponent<TextMeshProUGUI>().text = soul.ToString();
        transform.Find("IntValue").GetComponent<TextMeshProUGUI>().text = intelligence.ToString();
    }
}
