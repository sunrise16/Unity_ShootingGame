using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    public float enemyCurrentHp { get; set; }
    public float enemyMaxHp { get; set; }

    void Start()
    {
        enemyCurrentHp = 1000.0f;
        enemyMaxHp = 1000.0f;
	}

    public float GetEnemyCurrentHPRate()
    {
        return enemyCurrentHp / enemyMaxHp;
    }
}
