using TMPro;
using UnityEngine;

public class EssenceText : MonoBehaviour
{

    internal void SetEssence(int essence)
    {
        TextMeshProUGUI textComp = GetComponent<TextMeshProUGUI>();
        if(textComp != null)
        {
             textComp.text = $"Essence: {essence}";
        } else
        {
            Debug.LogError("TextMeshProUGUI component not found on the EssenceText object.");
        }
    }
}
