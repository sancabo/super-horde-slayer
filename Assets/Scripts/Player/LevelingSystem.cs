using System;
using UnityEngine;

public class LevelingSystem : MonoBehaviour
{
    [SerializeField] private int _toNextLvl;

    [SerializeField] private float _lvlCurveExp;

    [SerializeField] private GameDirector _gameDirector;

    [SerializeField] private FillableOrb _expBar;


    private int _experience = 0;

    private int _lvl = 1;


    void Start()
    {
        _expBar.SetAmountAndDeplete(_toNextLvl);
    }

    public int GetLevel()
    {
        return _lvl;
    }

    //Increase experience and adjust level if nextLevel threshold is reached. If needed, adjust the new threshold
    //Also increase the difficulty of the game and lastly, updates the UI.
    public void AwardExperience(int increment)
    {
        _experience += increment;
        while (_experience > _toNextLvl)
        {
            //Lvl Up logic
            _experience -= _toNextLvl;
            _toNextLvl = CalculateLevelingCurve(_lvl);
            _lvl++;
            _gameDirector.LvlUp();
            GetComponent<Player>().LvlUp();
            Debug.Log($"PLAYER: level up!. Exp Needed for next level: {_toNextLvl}");
        }
        _expBar.SetAmountAndDeplete(_toNextLvl);
        _expBar.Award(_experience);

        //TODO: Reverse the relation. Make the Display update itself.
        GameObject.Find("LvlDisplay").GetComponent<LvlDisplay>().SetLvl(_lvl);
    }

    //Returns the amount of experience needed to reach the next level.  
    private int CalculateLevelingCurve(int lvl)
    {
        //Total Exp Needed is a Polynomial function: f(Lvl) = Y * Lvl ^ 2. 
        //Y =_lvlCurveExp. Every ten levels Y Increases, by half of Y (40, 60, 80, 100, 120, ...)
        //The increment at each Lvl is the derivative of f. f' =  2 * Y * Lvl
        float quadraticExponent = _lvlCurveExp * (1f + 0.5f * (lvl / 10));
        return (int)Math.Floor(2 * lvl * quadraticExponent);
    }
}

