using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{

    [SerializeField] private Sprite _fireballSprite;
    [SerializeField] private Sprite _axeSprite;
    [SerializeField] private Sprite _spearSprite;
    [SerializeField] private Sprite _lightingSprite;

    [SerializeField] private Sprite _miasmaSprite;
    public enum Skill
    {
        Fireball,
        Axe,
        Spear,
        Dash,
        Lighting,

        Miasma,
        None
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssignToLeftClick(Skill skill)
    {
        transform.Find("SlotLeftClick").GetComponent<Image>().sprite = GetSkillSprite(skill);
        // Implement the logic to assign the skill to the left click
        Debug.Log($"Assigned {skill} to left click.");
    }

    private Sprite GetSkillSprite(Skill skill)
    {
        switch (skill)
        {
            case Skill.Fireball:
                return _fireballSprite;
            case Skill.Axe:
                return _axeSprite;
            case Skill.Spear:
                return _spearSprite;
            case Skill.Lighting:
                return _lightingSprite;
            case Skill.Miasma:
                return _miasmaSprite;
            default:
                throw new ArgumentOutOfRangeException(nameof(skill), skill, null);
        }
    }


    public void AssignToPosition(int position, Skill skill)
    {
        switch (position)
        {
            case 1:
                AssignTo(skill, "SlotOne");
                break;
            case 2:
                AssignTo(skill, "SlotTwo");
                break;
            case 3:
                AssignTo(skill, "SlotThree");
                break;
            case 0:
                AssignTo(skill, "SlotRightClick");
                break;
            default:
                Debug.LogWarning("Invalid position for skill assignment.");
                break;
        }
    }

    public void RemoveFromSlot(int position)
    {
        switch (position)
        {
            case 1:
                RemoveFromSlot("SlotOne");
                break;
            case 2:
                RemoveFromSlot("SlotTwo");
                break;
            case 3:
                RemoveFromSlot("SlotThree");
                break;
            case 0:
                RemoveFromSlot("SlotRightClick");
                break;
            default:
                Debug.LogWarning("Invalid position for skill assignment.");
                break;
        }
    }



    public void AssignTo(Skill skill, string slotName)
    {
        //transform.Find("SlotRightClick").GetComponent<Image>().sprite = GetSkillSprite(skill);
        transform.Find(slotName).GetComponent<Image>().sprite = GetSkillSprite(skill);
        transform.Find(slotName).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        Debug.Log($"Assigned {skill} to {slotName}");
    }

    public void AssignToSpace(Skill skill)
    {
        // Implement the logic to assign the skill to the space key
        Debug.Log($"Assigned {skill} to space key.");
    }


    public void RemoveFromSlot(string slotName)
    {
        transform.Find(slotName).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Debug.Log($"Removed {slotName} key.");
    }
}
