using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject enemy;
    private EnemyDatabase enemyDatabase;
    private EnemyFire enemyFire;
    private EnemySprite enemySprite;
    private GameObject destroyzoneAll;

    public int stageNumber;

    void Start()
    {
        enemy = GameObject.Find("ENEMY");
        enemyDatabase = GameObject.Find("ENEMY").GetComponent<EnemyDatabase>();
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();
        enemySprite = GameObject.Find("ENEMY").transform.Find("Body").GetComponent<EnemySprite>();
        destroyzoneAll = GameObject.Find("DESTROYZONE").transform.Find("DESTROYZONE_ALL").gameObject;

        stageNumber = 1;
        StartCoroutine(GameStart());
    }

    private void SetEnemyHp()
    {
        switch (stageNumber)
        {
            case 1: case 2: case 3: case 4: case 5: case 6:
                enemyDatabase.enemyCurrentHp = 1000.0f;
                enemyDatabase.enemyMaxHp = 1000.0f;
                break;
            case 7: case 8: case 9: case 10:
                enemyDatabase.enemyCurrentHp = 1200.0f;
                enemyDatabase.enemyMaxHp = 1200.0f;
                break;
            default:
                enemyDatabase.enemyCurrentHp = 0.0f;
                enemyDatabase.enemyMaxHp = 0.0f;
                break;
        }
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(2.0f);

        enemyFire.Fire(stageNumber);
        destroyzoneAll.SetActive(false);
    }

    public IEnumerator StageClear()
    {
        stageNumber++;
        SetEnemyHp();
        enemyFire.StopAllCoroutines();

        enemy.transform.position = new Vector2(0.0f, 3.5f);
        enemySprite.isLeftMove = false;
        enemySprite.isRightMove = false;
        destroyzoneAll.SetActive(true);
        StartCoroutine(GameStart());

        yield return null;
    }
}
