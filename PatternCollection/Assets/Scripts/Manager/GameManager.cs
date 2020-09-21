using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject enemy;
    private EnemyFire enemyFire;
    private EnemySprite enemySprite;
    private EnemyDatabase enemyDatabase;
    private Transform effectParent;
    private GameObject destroyzoneAll;

    public bool isCleared;
    public int stageNumber;

    void Start()
    {
        enemy = GameObject.Find("ENEMY");
        enemyFire = enemy.GetComponent<EnemyFire>();
        enemySprite = enemy.transform.GetChild(0).GetComponent<EnemySprite>();
        enemyDatabase = enemy.GetComponent<EnemyDatabase>();
        effectParent = GameObject.Find("EFFECT").transform;
        destroyzoneAll = GameObject.Find("DESTROYZONE").transform.GetChild(0).gameObject;

        stageNumber = 7;
        StartCoroutine(GameStart());
    }

    private void SetEnemyHp()
    {
        switch (stageNumber)
        {
            case 1: case 2:
                enemyDatabase.enemyCurrentHp = 400.0f;
                enemyDatabase.enemyMaxHp = 400.0f;
                break;
            case 7:
                enemyDatabase.enemyCurrentHp = 450.0f;
                enemyDatabase.enemyMaxHp = 450.0f;
                break;
            case 3: case 5:
                enemyDatabase.enemyCurrentHp = 500.0f;
                enemyDatabase.enemyMaxHp = 500.0f;
                break;
            case 8:
                enemyDatabase.enemyCurrentHp = 550.0f;
                enemyDatabase.enemyMaxHp = 550.0f;
                break;
            case 4: case 6:
                enemyDatabase.enemyCurrentHp = 600.0f;
                enemyDatabase.enemyMaxHp = 600.0f;
                break;
            default:
                enemyDatabase.enemyCurrentHp = 1000.0f;
                enemyDatabase.enemyMaxHp = 1000.0f;
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
        destroyzoneAll.SetActive(true);

        for (int i = 0; i < effectParent.childCount; i++)
        {
            if (effectParent.GetChild(i).gameObject.activeSelf.Equals(true))
            {
                effectParent.GetChild(i).GetComponent<EraseEffect>().ClearEffect();
            }
        }

        enemyFire.StopAllCoroutines();
        StartCoroutine(GameStart());

        iTween.MoveTo(enemy.gameObject, iTween.Hash("position", originPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        enemySprite.spriteIndexNumber = 0;
        StartCoroutine(enemy.GetComponent<EnemyFire>().EnemySpriteSet(originPosition.x, enemy.transform.position.x, moveTime));
        
        System.GC.Collect();

        yield return null;
    }
}
