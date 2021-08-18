using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private Camera camera;
    private Vector3 position;
    private EnemyType enemyType;

    private int[] enemyItem = new int[11];              // 적이 드랍하는 아이템 배열
    private int enemyNumber;                            // 적 분류 번호 (일반)
    private int enemyBossNumber;                        // 적 분류 번호 (보스)

    private float enemyCurrentHP;                       // 적의 현재 체력
    private float enemyMaxHP;                           // 적의 최대 체력
    private float[] enemyMaxHPArray;                    // 적의 최대 체력 배열

    private bool isCounter;                             // 사망 시 반격탄 발사 체크
    private bool isScreenOut;                           // 적이 화면 밖으로 벗어났는지 체크
    private bool isInvincible;                          // 적이 무적 상태인지 체크

    private void Start()
    {
        camera = GameObject.Find("CAMERA").transform.Find("GAMECAMERA").GetComponent<Camera>();
    }

    private void Update()
    {
        CheckScreenOut();
    }

    private void CheckScreenOut()
    {
        Vector3 screenPoint = camera.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0.0f && screenPoint.x < 1.0f && screenPoint.y > 0.0f && screenPoint.y < 1.0f;

        if (onScreen.Equals(false))
        {
            isScreenOut = true;
        }
        else
        {
            isScreenOut = false;
        }
    }

    #region GET, SET

    public float GetEnemyCurrentHPRate()
    {
        return enemyCurrentHP / enemyMaxHP;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public int GetEnemyItem(int index)
    {
        return enemyItem[index];
    }

    public int GetEnemyNumber()
    {
        return enemyNumber;
    }

    public int GetEnemyBossNumber()
    {
        return enemyBossNumber;
    }

    public float GetEnemyCurrentHP()
    {
        return enemyCurrentHP;
    }

    public float GetEnemyMaxHP()
    {
        return enemyMaxHP;
    }

    public float[] GetEnemyMaxHPArray()
    {
        return enemyMaxHPArray;
    }

    public bool GetCounter()
    {
        return isCounter;
    }

    public bool GetScreenOut()
    {
        return isScreenOut;
    }

    public bool GetInvincible()
    {
        return isInvincible;
    }

    public void SetEnemyType(EnemyType type)
    {
        enemyType = type;
    }

    public void SetEnemyNumber(int number)
    {
        enemyNumber = number;
    }

    public void SetEnemyBossNumber(int number)
    {
        enemyBossNumber = number;
    }

    public void SetEnemyItem(int[] itemArray)
    {
        for (int i = 0; i < 11; i++)
        {
            enemyItem[i] = itemArray[i];
        }
    }

    public void SetEnemyCurrentHP(float targetHP)
    {
        enemyCurrentHP = targetHP;
    }

    public void SetEnemyMaxHP(float targetHP)
    {
        enemyMaxHP = targetHP;
    }

    public void SetEnemyMaxHPArray(float[] targetHP)
    {
        enemyMaxHPArray = targetHP;
    }

    public void SetCounter(bool counter)
    {
        isCounter = counter;
    }

    public void SetScreenOut(bool screenOut)
    {
        isScreenOut = screenOut;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    #endregion
}