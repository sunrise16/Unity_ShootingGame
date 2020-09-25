using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum GameDifficulty
{
    DIFFICULTY_NONE,
    DIFFICULTY_EASY,
    DIFFICULTY_NORMAL,
    DIFFICULTY_HARD,
    DIFFICULTY_LUNATIC,
    DIFFICULTY_EXTRA,
}

public class GameManager : MonoBehaviour
{
    private GameObject enemy;
    private EnemyFire enemyFire;
    private EnemySprite enemySprite;
    private EnemyDatabase enemyDatabase;
    private Transform effectParent;
    private GameObject destroyzoneAll;

    public bool isCleared;
    public int patternNumber;
    public GameDifficulty gameDifficulty;

    void Start()
    {
        enemy = GameObject.Find("ENEMY");
        enemyFire = enemy.GetComponent<EnemyFire>();
        enemySprite = enemy.transform.GetChild(0).GetComponent<EnemySprite>();
        enemyDatabase = enemy.GetComponent<EnemyDatabase>();
        effectParent = GameObject.Find("EFFECT").transform;
        destroyzoneAll = GameObject.Find("DESTROYZONE").transform.GetChild(0).gameObject;

        StartCoroutine(GameStart());
    }

    private void SetEnemyHp()
    {
        switch (patternNumber)
        {
            case 1:     // 루미아 중간보스 통상
                enemyDatabase.enemyCurrentHp = 150.0f;
                enemyDatabase.enemyMaxHp = 150.0f;
                break;
            case 2:     // 루미아 중간보스 스펠
            case 7:     // 대요정 중간보스 통상
                enemyDatabase.enemyCurrentHp = 350.0f;
                enemyDatabase.enemyMaxHp = 350.0f;
                break;
            case 13:    // 홍 메이링 중간보스 통상
                enemyDatabase.enemyCurrentHp = 450.0f;
                enemyDatabase.enemyMaxHp = 450.0f;
                break;
            case 3:     // 루미아 1통상
            case 5:     // 루미아 2통상
                enemyDatabase.enemyCurrentHp = 500.0f;
                enemyDatabase.enemyMaxHp = 500.0f;
                break;
            case 8:     // 치르노 1통상
            case 14:    // 홍 메이링 중간보스 스펠
                enemyDatabase.enemyCurrentHp = 550.0f;
                enemyDatabase.enemyMaxHp = 550.0f;
                break;
            case 4:     // 루미아 1스펠
            case 6:     // 루미아 2스펠
            case 10:    // 치르노 2통상
            case 15:    // 홍 메이링 1통상
            case 17:    // 홍 메이링 2통상
                enemyDatabase.enemyCurrentHp = 600.0f;
                enemyDatabase.enemyMaxHp = 600.0f;
                break;
            case 9:     // 치르노 1스펠
            case 19:    // 홍 메이링 3통상
                enemyDatabase.enemyCurrentHp = 650.0f;
                enemyDatabase.enemyMaxHp = 650.0f;
                break;
            case 11:    // 치르노 2스펠
            case 12:    // 치르노 3스펠
            case 16:    // 홍 메이링 1스펠
            case 18:    // 홍 메이링 2스펠
            case 20:    // 홍 메이링 3스펠
                enemyDatabase.enemyCurrentHp = 700.0f;
                enemyDatabase.enemyMaxHp = 700.0f;
                break;
            case 21:    // 홍 메이링 4스펠
                enemyDatabase.enemyCurrentHp = 750.0f;
                enemyDatabase.enemyMaxHp = 750.0f;
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

        enemyFire.Fire(patternNumber);
        destroyzoneAll.SetActive(false);
        isCleared = false;
    }

    public IEnumerator StageClear()
    {
        isCleared = true;
        Vector3 originPosition = new Vector3(0.0f, 3.5f, 0.0f);
        float moveTime = 1.5f;

        patternNumber++;
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
