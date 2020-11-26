using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private float enemyCurrentHP;
    private float enemyMaxHP;

    private void Awake()
    {
        enemyCurrentHP = 1000.0f;
        enemyMaxHP = 1000.0f;
	}

    public float GetEnemyCurrentHPRate()
    {
        return enemyCurrentHP / enemyMaxHP;
    }

    public float GetEnemyCurrentHP()
    {
        return enemyCurrentHP;
    }

    public float GetEnemyMaxHP()
    {
        return enemyMaxHP;
    }

    public void SetEnemyCurrentHP(float damage)
    {
        enemyCurrentHP -= damage;
    }

    public void SetEnemyMaxHP(float targetHP)
    {
        enemyMaxHP = targetHP;
    }
}
