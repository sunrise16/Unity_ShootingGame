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
    public Sprite[] characterSprite;
    public RuntimeAnimatorController[] animatorController;

    private void Start()
    {
        // 테스트용
        GameData.gameMode = GameMode.GAMEMODE_MAINGAME;
        GameData.gameDifficulty = GameDifficulty.DIFFICULTY_LUNATIC;
        // 테스트용

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
            GameData.currentPlayerBomb = 3;
        }
        else if (GameData.gameMode.Equals(GameMode.GAMEMODE_PRACTICE))
        {
            GameData.currentStage = (int)GameData.gamePracticeStage;
            GameData.currentPlayerLife = 8;
            GameData.currentPlayerBomb = 8;
        }
        GameData.currentChapter = 1;

        StageStart();
    }

    private void StageStart()
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
        yield return new WaitForSeconds(1.0f);

        int wave1Count = 0;
        while (wave1Count <= 8)
        {
            GameObject stage1_MinionLarge1 = CreateMinion(new Vector2(-5.0f, 2.0f), "ENEMY", LayerMask.NameToLayer("ENEMY_BODY"),
            new Vector3(1.5f, 1.5f, 1.0f), 0.3f, 0, EnemyType.ENEMYTYPE_LMINION, 500.0f, 1, true, 10.0f);
            EnemyMoveOnce(stage1_MinionLarge1, new Vector3(5.0f, 1.5f, 1.0f), iTween.EaseType.easeInOutQuad, 10.0f);

            yield return new WaitForSeconds(1.2f);

            wave1Count++;
        }

        yield return new WaitForSeconds(1.0f);
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

    #region 스테이지 클리어

    public void StageClear()
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

    #region 적 미니언 생성

    public GameObject CreateMinion(Vector2 spawnPosition, string enemyTag, int enemyLayer, Vector3 enemyScale, float colliderRadius,
        int animationNumber, EnemyType enemyType, float enemyHP, int enemyPatternNumber, bool isAutoDestroy = false, float waitTime = 0.0f)
    {
        GameObject enemy = enemyPool.GetChild(0).gameObject;
        enemy.SetActive(true);
        enemy.transform.position = spawnPosition;
        enemy.gameObject.tag = enemyTag;
        enemy.gameObject.layer = enemyLayer;
        enemy.transform.SetParent(enemyParent);
        enemy.transform.localScale = enemyScale;
        enemy.GetComponent<CircleCollider2D>().radius = colliderRadius;
        enemy.GetComponent<Animator>().runtimeAnimatorController = animatorController[animationNumber];

        EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
        enemyStatus.SetEnemyType(enemyType);
        enemyStatus.SetEnemyMaxHP(enemyHP);
        enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHP());

        if (!enemy.GetComponent<Minion_Pattern1>())
        {
            enemy.AddComponent<Minion_Pattern1>();
        }

        EnemyDestroy enemyDestroy = enemy.GetComponent<EnemyDestroy>();
        if (isAutoDestroy.Equals(true))
        {
            StartCoroutine(enemyDestroy.EnemyAutoDestroy(waitTime));
        }

        return enemy;
    }

    // 적 이동
    public void EnemyMoveOnce(GameObject gameObject, Vector3 targetPosition, iTween.EaseType easeType, float moveTime)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        // 스프라이트 조절
        if (gameObject.transform.position.x > targetPosition.x)
        {
            animator.SetTrigger("isLeftMove");
        }
        else if (gameObject.transform.position.x < targetPosition.x)
        {
            animator.SetTrigger("isRightMove");
        }
        else
        {
            animator.SetTrigger("isIdle");
        }

        // 최종 이동 처리
        iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", easeType, "time", moveTime));
    }

    #endregion
}
