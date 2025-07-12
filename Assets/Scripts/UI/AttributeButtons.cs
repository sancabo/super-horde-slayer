using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeButtons : MonoBehaviour
{
    public int points = 0;

    // Update is called once per frame
    void Update()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        if (points > 0)
        {

            Array.ForEach(buttons, button =>
            {
                button.interactable = true;
            });
        }
        else
        {
            Array.ForEach(buttons, button =>
            {
                button.interactable = false;
            });
        }

        transform.Find("PointsButton").GetComponentInChildren<TextMeshProUGUI>().text = $"{points}";
    }
}
