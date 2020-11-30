using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private EnemyType enemyType;

    private int enemyPatternNumber;

    private float enemyCurrentHP;
    private float enemyMaxHP;

    public float GetEnemyCurrentHPRate()
    {
        return enemyCurrentHP / enemyMaxHP;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public int GetEnemyPatternNumber()
    {
        return enemyPatternNumber;
    }

    public float GetEnemyCurrentHP()
    {
        return enemyCurrentHP;
    }

    public float GetEnemyMaxHP()
    {
        return enemyMaxHP;
    }

    public void SetEnemyType(EnemyType type)
    {
        enemyType = type;
    }

    public void SetEnemyPatternNumber(int number)
    {
        enemyPatternNumber = number;
    }

    public void SetEnemyCurrentHP(float targetHP)
    {
        enemyCurrentHP = targetHP;
    }

    public void SetEnemyMaxHP(float targetHP)
    {
        enemyMaxHP = targetHP;
    }
}