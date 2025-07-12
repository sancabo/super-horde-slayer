using UnityEngine;
using TMPro;

public class LvlDisplay : MonoBehaviour
{
     public void SetLvl(int lvl)
    {
         TextMeshProUGUI textComp = GetComponent<TextMeshProUGUI>();
        if(textComp != null)
        {
             textComp.text = $"Lvl. {lvl}";
        } else
        {
            Debug.LogError("TextMeshProUGUI component not found on the EssenceText object.");
        }
    }
}
