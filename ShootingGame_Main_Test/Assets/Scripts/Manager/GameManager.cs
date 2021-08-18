using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Transform enemyPool;
    private Transform enemyParent;

    public Sprite[] bulletSprite;
    public Sprite[] effectSprite;
    public Sprite[] itemSprite;
    public Sprite[] characterSprite;
    public RuntimeAnimatorController[] characterAnimatorController;
    public RuntimeAnimatorController[] bulletAnimatorController;

    public GameMode gameMode;
    public GameDifficulty gameDifficulty;
    public int gameStage;
    public int gameChapter;

    private void Start()
    {
        GameData.gameMode = gameMode;
        GameData.gameDifficulty = gameDifficulty;
        GameData.currentStage = gameStage;
        GameData.currentChapter = gameChapter;

        enemyPool = GameObject.Find("CHARACTER").transform.Find("Enemy");
        enemyParent = GameObject.Find("CHARACTER").transform.Find("Enemy_Temp");

        if (GameData.gameMode.Equals(GameMode.GAMEMODE_MAINGAME))
        {
            // 최초 잔기 수와 폭탄은 옵션에서 설정한 값으로 넣어야 함 (옵션 기능 추가 시 고쳐야 된다는 뜻)
            GameData.currentPlayerLife = 2;
            GameData.currentPlayerSpell = 3;
        }
        else if (GameData.gameMode.Equals(GameMode.GAMEMODE_PRACTICE))
        {
            GameData.currentPlayerLife = 8;
            GameData.currentPlayerSpell = 8;
        }
        StageStart();
    }

    public void StageStart()
    {
        switch (GameData.currentStage)
        {
            case 1:
                StartCoroutine(Stage1());
                break;
            case 2:
                StartCoroutine(Stage2());
                break;
            case 3:
                StartCoroutine(Stage3());
                break;
            case 4:
                StartCoroutine(Stage4());
                break;
            case 5:
                StartCoroutine(Stage5());
                break;
            case 6:
                StartCoroutine(Stage6());
                break;
            case 7:
                StartCoroutine(StageExtra());
                break;
            default:
                break;
        }
    }

    #region 스테이지 진행

    #region 스테이지 1

    public IEnumerator Stage1()
    {
        // 스테이지 1 BGM 재생
        SoundManager.instance.PlayBGM(1);

        yield return new WaitForSeconds(2.0f);

        #region Enemy Appear
        
        int[] enemyItem = new int[11] { 3, 5, 2, 2, 0, 0, 0, 1, 0, 0, 0 };
        float[] enemyHP = new float[6] { 200.0f, 160.0f, 240.0f, 200.0f, 240.0f, 220.0f };
        Vector3 spawnPosition = new Vector3(0.0f, 5.0f, 0.0f);
        Vector3 targetPosition = new Vector3(0.0f, 3.0f, 0.0f);

        GameObject boss = CreateBoss(spawnPosition, "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"), new Vector3(1.5f, 1.5f, 1.0f),
                0.2f, 3, false, EnemyType.ENEMYTYPE_BOSS, enemyItem, 1, enemyHP);
        EnemyMove enemyMove = boss.GetComponent<EnemyMove>();
        EnemyFire enemyFire = boss.GetComponent<EnemyFire>();

        StartCoroutine(enemyMove.EnemyMoveOnce(targetPosition, iTween.EaseType.easeOutQuad, 1.0f));

        yield return new WaitForSeconds(1.5f);

        enemyFire.EnemyBossAttack(GameData.currentStage, GameData.currentChapter);

        #endregion
    }

    #endregion

    #region 스테이지 2

    private IEnumerator Stage2()
    {
        return null;
    }

    #endregion

    #region 스테이지 3

    private IEnumerator Stage3()
    {
        return null;
    }

    #endregion

    #region 스테이지 4

    private IEnumerator Stage4()
    {
        return null;
    }

    #endregion

    #region 스테이지 5

    private IEnumerator Stage5()
    {
        return null;
    }

    #endregion

    #region 스테이지 6

    private IEnumerator Stage6()
    {
        return null;
    }

    #endregion

    #region 스테이지 엑스트라

    private IEnumerator StageExtra()
    {
        return null;
    }

    #endregion

    #endregion

    #region 적 오브젝트 생성

    public GameObject CreateMinion(Vector3 spawnPosition, string enemyTag, int enemyLayer, Vector3 enemyScale, float colliderRadius, int animationNumber,
        bool enemyCounter, EnemyType enemyType, int[] enemyItem, int enemyNumber, float enemyHP, int enemyPatternNumber, int enemyCounterPatternNumber,
        float enemyAttackWaitTime, float enemyAttackDelayTime, bool isPatternOnce, bool isPatternRepeat, int enemyFireCount, float enemyAttackRepeatTime,
        int enemyCustomPatternNumber = 0, bool isAutoDestroy = false, float waitTime = 0.0f)
    {
        GameObject enemy = enemyPool.GetChild(0).gameObject;
        enemy.SetActive(true);
        enemy.transform.position = spawnPosition;
        enemy.gameObject.tag = enemyTag;
        enemy.gameObject.layer = enemyLayer;
        enemy.transform.SetParent(enemyParent);
        enemy.transform.localScale = enemyScale;

        CircleCollider2D circleCollider2D = enemy.GetComponent<CircleCollider2D>();
        circleCollider2D.enabled = true;
        circleCollider2D.radius = colliderRadius;

        Animator animator = enemy.GetComponent<Animator>();
        animator.runtimeAnimatorController = characterAnimatorController[animationNumber];

        EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
        enemyStatus.SetCounter(enemyCounter);
        enemyStatus.SetEnemyType(enemyType);
        enemyStatus.SetEnemyItem(enemyItem);
        if (enemyType.Equals(EnemyType.ENEMYTYPE_BOSS))
        {
            enemyStatus.SetEnemyNumber(0);
            enemyStatus.SetEnemyBossNumber(enemyNumber);
        }
        else if (enemyType.Equals(EnemyType.ENEMYTYPE_NONE))
        {
            enemyStatus.SetEnemyNumber(0);
            enemyStatus.SetEnemyBossNumber(0);
        }
        else
        {
            enemyStatus.SetEnemyNumber(enemyNumber);
            enemyStatus.SetEnemyBossNumber(0);
        }
        enemyStatus.SetEnemyMaxHP(enemyHP);
        enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHP());

        EnemyFire enemyFire = enemy.GetComponent<EnemyFire>();
        enemyFire.SetEnemyPatternNumber(enemyPatternNumber);
        enemyFire.SetEnemyCounterPatternNumber(enemyCounterPatternNumber);
        enemyFire.SetEnemyAttackWaitTime(enemyAttackWaitTime);
        enemyFire.SetEnemyAttackDelayTime(enemyAttackDelayTime);
        enemyFire.SetEnemyPatternOnce(isPatternOnce);
        enemyFire.SetEnemyPatternRepeat(isPatternRepeat);
        enemyFire.SetEnemyFireCount(enemyFireCount);
        enemyFire.SetEnemyAttackRepeatTime(enemyAttackRepeatTime);
        enemyFire.SetEnemyCustomPatternNumber(enemyCustomPatternNumber);

        EnemyDestroy enemyDestroy = enemy.GetComponent<EnemyDestroy>();
        if (isAutoDestroy.Equals(true))
        {
            StartCoroutine(enemyDestroy.EnemyAutoDestroy(waitTime));
        }

        return enemy;
    }
    public GameObject CreateBoss(Vector3 spawnPosition, string enemyTag, int enemyLayer, Vector3 enemyScale, float colliderRadius, int animationNumber,
        bool enemyCounter, EnemyType enemyType, int[] enemyItem, int enemyNumber, float[] enemyHP)
    {
        GameObject enemy = enemyPool.GetChild(0).gameObject;
        enemy.SetActive(true);
        enemy.transform.position = spawnPosition;
        enemy.gameObject.tag = enemyTag;
        enemy.gameObject.layer = enemyLayer;
        enemy.transform.SetParent(enemyParent);
        enemy.transform.localScale = enemyScale;

        CircleCollider2D circleCollider2D = enemy.GetComponent<CircleCollider2D>();
        circleCollider2D.enabled = true;
        circleCollider2D.radius = colliderRadius;

        Animator animator = enemy.GetComponent<Animator>();
        animator.runtimeAnimatorController = characterAnimatorController[animationNumber];

        EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
        enemyStatus.SetCounter(enemyCounter);
        enemyStatus.SetEnemyType(enemyType);
        enemyStatus.SetEnemyItem(enemyItem);
        if (enemyType.Equals(EnemyType.ENEMYTYPE_BOSS))
        {
            enemyStatus.SetEnemyNumber(0);
            enemyStatus.SetEnemyBossNumber(enemyNumber);
        }
        else if (enemyType.Equals(EnemyType.ENEMYTYPE_NONE))
        {
            enemyStatus.SetEnemyNumber(0);
            enemyStatus.SetEnemyBossNumber(0);
        }
        else
        {
            enemyStatus.SetEnemyNumber(enemyNumber);
            enemyStatus.SetEnemyBossNumber(0);
        }
        enemyStatus.SetEnemyMaxHPArray(enemyHP);
        enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHPArray()[0]);

        return enemy;
    }

    #endregion
}
