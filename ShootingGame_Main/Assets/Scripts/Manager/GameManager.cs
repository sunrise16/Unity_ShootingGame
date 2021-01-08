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

    private void Start()
    {
        /// 테스트용 (메인 화면 구현하면 지울 것 !!!)
        GameData.gameMode = GameMode.GAMEMODE_MAINGAME;
        GameData.gameDifficulty = GameDifficulty.DIFFICULTY_LUNATIC;
        /// 테스트용 (메인 화면 구현하면 지울 것 !!!)
        
        enemyPool = GameObject.Find("CHARACTER").transform.Find("Enemy");
        enemyParent = GameObject.Find("CHARACTER").transform.Find("Enemy_Temp");

        if (GameData.gameMode.Equals(GameMode.GAMEMODE_MAINGAME))
        {
            switch (GameData.gameDifficulty)
            {
                case GameDifficulty.DIFFICULTY_EASY:
                case GameDifficulty.DIFFICULTY_NORMAL:
                case GameDifficulty.DIFFICULTY_HARD:
                case GameDifficulty.DIFFICULTY_LUNATIC:
                    GameData.currentStage = 1;
                    break;
                case GameDifficulty.DIFFICULTY_EXTRA:
                    GameData.currentStage = 7;
                    break;
            }
            // 최초 잔기 수와 폭탄은 옵션에서 설정한 값으로 넣어야 함 (옵션 기능 추가 시 고쳐야 된다는 뜻)
            GameData.currentPlayerLife = 2;
            GameData.currentPlayerSpell = 3;
        }
        else if (GameData.gameMode.Equals(GameMode.GAMEMODE_PRACTICE))
        {
            GameData.currentStage = (int)GameData.gamePracticeStage;
            GameData.currentPlayerLife = 8;
            GameData.currentPlayerSpell = 8;
        }
        GameData.currentChapter = 1;

        StageStart();
    }

    private void StageStart()
    {
        switch (GameData.currentStage)
        {
            case 1:
                StartCoroutine(Stage1_FirstHalf());
                break;
            case 2:
                StartCoroutine(Stage2_FirstHalf());
                break;
            case 3:
                StartCoroutine(Stage3_FirstHalf());
                break;
            case 4:
                StartCoroutine(Stage4_FirstHalf());
                break;
            case 5:
                StartCoroutine(Stage5_FirstHalf());
                break;
            case 6:
                StartCoroutine(Stage6_FirstHalf());
                break;
            case 7:
                StartCoroutine(StageExtra_FirstHalf());
                break;
            default:
                break;
        }
    }

    #region 스테이지 진행

    #region 스테이지 1

    public IEnumerator Stage1_FirstHalf()
    {
        // 스테이지 1 BGM 재생
        SoundManager.instance.PlayBGM(1);

        yield return new WaitForSeconds(1.0f);

        #region Wave 1
        
        int[] wave1Item = new int[11] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Vector3 wave1SpawnPosition1;
        Vector3 wave1SpawnPosition2;
        for (int i = 0; i < 8; i++)
        {
            wave1SpawnPosition1 = new Vector3(-2.0f + (0.25f * i), 5.0f, 0.0f);
            wave1SpawnPosition2 = new Vector3(2.0f - (0.25f * i), 5.0f, 0.0f);
            Vector3[] paths1 = new Vector3[3];
            Vector3[] paths2 = new Vector3[3];

            GameObject minionSmall1 = CreateEnemy(wave1SpawnPosition1, "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"), new Vector3(1.5f, 1.5f, 1.0f),
                0.2f, 3, false, EnemyType.ENEMYTYPE_SMINION, wave1Item, 1, 10.0f, 1, 0, 2.0f, 0.125f, false, true, 8, 0.5f, 0, true, 9.0f);
            GameObject minionSmall2 = CreateEnemy(wave1SpawnPosition2, "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"), new Vector3(1.5f, 1.5f, 1.0f),
                0.2f, 3, false, EnemyType.ENEMYTYPE_SMINION, wave1Item, 1, 10.0f, 1, 0, 2.0f, 0.125f, false, true, 8, 0.5f, 0, true, 9.0f);
            EnemyMove enemyMove1 = minionSmall1.GetComponent<EnemyMove>();
            EnemyMove enemyMove2 = minionSmall2.GetComponent<EnemyMove>();

            paths1[0] = new Vector3(minionSmall1.transform.position.x, minionSmall1.transform.position.y, 0.0f);
            paths1[1] = new Vector3(minionSmall1.transform.position.x - 1.0f, minionSmall1.transform.position.y - 2.0f, 0.0f);
            paths1[2] = new Vector3(minionSmall1.transform.position.x - 5.0f, minionSmall1.transform.position.y - 3.0f, 0.0f);
            StartCoroutine(enemyMove1.EnemyMovePathOnce(paths1, iTween.EaseType.easeInOutQuad, 9.0f));

            paths2[0] = new Vector3(minionSmall2.transform.position.x, minionSmall2.transform.position.y, 0.0f);
            paths2[1] = new Vector3(minionSmall2.transform.position.x + 1.0f, minionSmall2.transform.position.y - 2.0f, 0.0f);
            paths2[2] = new Vector3(minionSmall2.transform.position.x + 5.0f, minionSmall2.transform.position.y - 3.0f, 0.0f);
            StartCoroutine(enemyMove2.EnemyMovePathOnce(paths2, iTween.EaseType.easeInOutQuad, 9.0f));

            yield return new WaitForSeconds(0.4f);
        }

        #endregion

        yield return new WaitForSeconds(7.0f);
        // 스테이지 1 타이틀
        StageTitleOutput(1);

        yield return new WaitForSeconds(5.0f);
        GameData.currentChapter++;

        #region Wave 2

        int[] wave2Item1 = new int[11]{ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] wave2Item2 = new int[11] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] wave2Item3 = new int[11] { 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
        Vector3[] wave2SpawnPosition = new Vector3[10]
            { new Vector3(-0.25f, 5.0f), new Vector3(0.25f, 5.0f), new Vector3(-0.75f, 5.0f), new Vector3(0.75f, 5.0f), new Vector3(-1.25f, 5.0f),
                new Vector3(1.25f, 5.0f), new Vector3(-1.75f, 5.0f), new Vector3(1.75f, 5.0f), new Vector3(-2.25f, 5.0f), new Vector3(2.25f, 5.0f) };
        
        StartCoroutine(Stage1_Wave2Pattern1(wave2Item3));
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(Stage1_Wave2Pattern2(wave2Item1, wave2Item2, wave2SpawnPosition));

        #endregion

        yield return new WaitForSeconds(14.5f);
        GameData.currentChapter++;

        #region Wave 3

        int[] wave3Item = new int[11] { 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Vector3 wave3SpawnPosition1 = new Vector3(-4.0f, 1.5f, 0.0f);
        Vector3 wave3SpawnPosition2 = new Vector3(4.0f, 1.5f, 0.0f);
        Vector3 wave3TargetPosition1 = new Vector3(4.0f, 3.0f, 0.0f);
        Vector3 wave3TargetPosition2 = new Vector3(-4.0f, 3.0f, 0.0f);
        for (int i = 0; i < 16; i++)
        {
            GameObject minionSmall = CreateEnemy(i < 8 ? wave3SpawnPosition1 : wave3SpawnPosition2, "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"),
                new Vector3(1.5f, 1.5f, 1.0f), 0.2f, 2, true, EnemyType.ENEMYTYPE_SMINION, wave3Item, 1, 12.0f, 4, 1, 1.0f, 0.5f, false, false, 0, 0.0f, 1, true, 6.5f);
            EnemyMove enemyMove = minionSmall.GetComponent<EnemyMove>();
            
            StartCoroutine(enemyMove.EnemyMoveOnce(i < 8 ? wave3TargetPosition1 : wave3TargetPosition2, iTween.EaseType.linear, 6.5f));

            yield return new WaitForSeconds(i.Equals(7) ? 2.0f : 0.85f);
        }

        #endregion

        yield return new WaitForSeconds(9.4f);
        GameData.currentChapter++;

        // 스테이지 1 중간보스
        Debug.Log("TEST");
        StartCoroutine(Stage1_MidBoss());
    }

    private IEnumerator Stage1_MidBoss()
    {
        #region Spawn

        int[] midBossItem = new int[11] { 0, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0 };
        Vector3 midBossSpawnPosition = new Vector3(0.0f, 5.5f, 0.0f);
        Vector3 midBossTargetPosition = new Vector3(0.0f, 2.0f, 0.0f);
        GameObject minionLarge = CreateEnemy(midBossSpawnPosition, "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"),
                new Vector3(1.5f, 1.5f, 1.0f), 0.5f, 24, true, EnemyType.ENEMYTYPE_BOSS, midBossItem, 1, 300.0f, 1, 0, 3.0f, 0.05f, false, true, -1, 0.0f);
        EnemyMove enemyMove = minionLarge.GetComponent<EnemyMove>();

        StartCoroutine(enemyMove.EnemyMoveOnce(midBossTargetPosition, iTween.EaseType.easeOutQuad, 1.5f));

        yield return new WaitForSeconds(3.0f);

        #endregion

        yield return new WaitForSeconds(50.0f);
        GameData.currentChapter++;

        // 스테이지 1 후반
        Debug.Log("TEST2");
        // StartCoroutine(Stage1_SecondHalf());
    }

    #endregion

    #region 스테이지 2

    private IEnumerator Stage2_FirstHalf()
    {
        return null;
    }

    #endregion

    #region 스테이지 3

    private IEnumerator Stage3_FirstHalf()
    {
        return null;
    }

    #endregion

    #region 스테이지 4

    private IEnumerator Stage4_FirstHalf()
    {
        return null;
    }

    #endregion

    #region 스테이지 5

    private IEnumerator Stage5_FirstHalf()
    {
        return null;
    }

    #endregion

    #region 스테이지 6

    private IEnumerator Stage6_FirstHalf()
    {
        return null;
    }

    #endregion

    #region 스테이지 엑스트라

    private IEnumerator StageExtra_FirstHalf()
    {
        return null;
    }

    #endregion

    #endregion

    #region 스테이지 타이틀

    public void StageTitleOutput(int stageNumber)
    {
        // 임시
        Debug.Log(string.Format("Stage {0}", stageNumber));
    }

    #endregion

    #region 스테이지 클리어

    private void StageClear()
    {
        if (GameData.gameMode.Equals(GameMode.GAMEMODE_MAINGAME))
        {
            if (GameData.currentStage < 6)
            {
                GameData.currentStage++;
                GameData.currentChapter = 1;
                StageStart();
            }
            else
            {
                GameData.gameMode = GameMode.GAMEMODE_ENDING;

                // 조건에 따른 최종 등급 설정
                // 여기에 작성

                UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
            }
        }
        else if (GameData.gameMode.Equals(GameMode.GAMEMODE_PRACTICE))
        {
            GameData.gameMode = GameMode.GAMEMODE_TITLE;
            GameData.InitGameData();
        }
    }

    #endregion

    #region 스테이지 세부 적 생성 패턴

    #region 스테이지 1

    public IEnumerator Stage1_Wave2Pattern1(int[] item)
    {
        Vector3 wave2TargetPosition;
        Vector3[] wave2SpawnPosition = new Vector3[2] { new Vector3(-2.125f, 5.0f, 0.0f), new Vector3(2.125f, 5.0f, 0.0f) };

        for (int i = 0; i < 2; i++)
        {
            wave2TargetPosition = new Vector3(wave2SpawnPosition[i].x, wave2SpawnPosition[i].y - 2.0f, 0.0f);

            GameObject minionLarge = CreateEnemy(wave2SpawnPosition[i], "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"), new Vector3(1.5f, 1.5f, 1.0f),
                0.5f, 24, false, EnemyType.ENEMYTYPE_LMINION, item, i, 75.0f, 3, 0, 2.0f, 1.25f, false, false, 0, 0.0f, 0, true, 14.0f);
            EnemyMove enemyMove = minionLarge.GetComponent<EnemyMove>();

            StartCoroutine(enemyMove.EnemyMoveTwice(wave2TargetPosition, wave2SpawnPosition[i], iTween.EaseType.easeOutQuart, iTween.EaseType.easeInQuad, 2.0f, 4.0f, 8.0f));
        }

        yield return null;
    }

    public IEnumerator Stage1_Wave2Pattern2(int[] item1, int[] item2, Vector3[] spawnPosition)
    {
        Vector3 wave2TargetPosition;

        for (int i = 0; i < 10; i++)
        {
            wave2TargetPosition = new Vector3(spawnPosition[i].x, spawnPosition[i].y - 3.0f, 0.0f);

            GameObject minionSmall = CreateEnemy(spawnPosition[i], "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"), new Vector3(1.5f, 1.5f, 1.0f),
                0.2f, 5, false, EnemyType.ENEMYTYPE_SMINION, (i % 4 == 0) ? item2 : item1, 1, 10.0f, 2, 0, 1.5f, 1.3f, true, true, 5, 0.0f, 0, true, 9.0f);
            EnemyMove enemyMove = minionSmall.GetComponent<EnemyMove>();

            StartCoroutine(enemyMove.EnemyMoveTwice(wave2TargetPosition, spawnPosition[i], iTween.EaseType.easeOutQuart, iTween.EaseType.easeInQuad, 1.5f, 3.0f, 4.5f));

            if ((i % 2).Equals(1))
            {
                yield return new WaitForSeconds(1.2f);
            }
        }
    }

    #endregion

    #endregion

    #region 적 오브젝트 생성

    private GameObject CreateEnemy(Vector3 spawnPosition, string enemyTag, int enemyLayer, Vector3 enemyScale, float colliderRadius, int animationNumber,
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

    #endregion
}
