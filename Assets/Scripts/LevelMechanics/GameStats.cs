using System;
using System.Collections;
using UnityEngine;

public class GameStats : MonoBehaviour
{

    public int enemiesKilled = 0;

    public int PlayTimeHour { get; private set; }
    public int PlayTimeMinute { get; private set; }
    public int PlayTimeSecond { get; private set; }

    public int maxDamageDone = 0;
    private int playTime = 0;


    void Start()
    {
        StartCoroutine(RecordTimeRoutine());
    }

    public void EnemyDied()
    {
        enemiesKilled++;
    }

    public void DamageDone(int damage)
    {
         Debug.Log("PRE Updating Max Damage Done " + maxDamageDone + "-->" + damage);
        if (damage > maxDamageDone)
        {
            Debug.Log("POST Updating Max Damage Done " + maxDamageDone + "-->" + damage);
            maxDamageDone = damage;
        }
    }

    public IEnumerator RecordTimeRoutine()
    {
        TimeSpan ts;
        while (true)
        {
            yield return new WaitForSeconds(1);
            playTime += 1;

            ts = TimeSpan.FromSeconds(playTime);

            PlayTimeHour = (int)ts.TotalHours;
            PlayTimeMinute = ts.Minutes;
            PlayTimeSecond = ts.Seconds;
        }
    }
}
