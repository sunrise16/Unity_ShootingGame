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

    public bool isCleared;
    public int stageNumber;

    void Start()
    {
        enemy = GameObject.Find("ENEMY");
        enemyDatabase = GameObject.Find("ENEMY").GetComponent<EnemyDatabase>();
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();
        enemySprite = GameObject.Find("ENEMY").transform.GetChild(0).GetComponent<EnemySprite>();
        destroyzoneAll = GameObject.Find("DESTROYZONE").transform.GetChild(0).gameObject;

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
        isCleared = false;
    }

    public IEnumerator StageClear()
    {
        isCleared = true;
        Vector3 originPosition = new Vector3(0.0f, 3.5f, 0.0f);
        float moveTime = 1.5f;

        stageNumber++;
        SetEnemyHp();
        enemyFire.StopAllCoroutines();

        iTween.MoveTo(enemy.gameObject, iTween.Hash("position", originPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(enemy.GetComponent<EnemyFire>().EnemySpriteSet(originPosition.x, enemy.transform.position.x, moveTime));

        enemySprite.isLeftMove = false;
        enemySprite.isRightMove = false;
        destroyzoneAll.SetActive(true);
        StartCoroutine(GameStart());

        System.GC.Collect();

        yield return null;
    }
}
