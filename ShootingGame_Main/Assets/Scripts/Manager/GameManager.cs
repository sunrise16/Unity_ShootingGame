using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        if (GlobalData.gameMode.Equals(GameMode.GAMEMODE_MAINGAME))
        {
            switch (GlobalData.gameDifficulty)
            {
                case GameDifficulty.DIFFICULTY_EASY:
                case GameDifficulty.DIFFICULTY_NORMAL:
                case GameDifficulty.DIFFICULTY_HARD:
                case GameDifficulty.DIFFICULTY_LUNATIC:
                    GlobalData.currentStage = 1;
                    break;
                case GameDifficulty.DIFFICULTY_EXTRA:
                    GlobalData.currentStage = 7;
                    break;
            }
            GlobalData.currentPlayerLife = 2;
            GlobalData.currentPlayerBomb = 3;
        }
        else if (GlobalData.gameMode.Equals(GameMode.GAMEMODE_PRACTICE))
        {
            GlobalData.currentStage = (int)GlobalData.gamePracticeStage;
            GlobalData.currentPlayerLife = 8;
            GlobalData.currentPlayerBomb = 8;
        }
        GlobalData.currentChapter = 1;

        StageStart();
    }

    private void StageStart()
    {
        switch (GlobalData.currentStage)
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

    private IEnumerator Stage1()
    {
        return null;
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

    private void StageClear()
    {
        if (GlobalData.gameMode.Equals(GameMode.GAMEMODE_MAINGAME))
        {
            if (GlobalData.currentStage < 6)
            {
                GlobalData.currentStage++;
                GlobalData.currentChapter = 1;
                StageStart();
            }
            else
            {
                GlobalData.gameMode = GameMode.GAMEMODE_ENDING;

                // 조건에 따른 최종 등급 설정
                // 여기에 작성

                UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
            }
        }
        else if (GlobalData.gameMode.Equals(GameMode.GAMEMODE_PRACTICE))
        {
            GlobalData.gameMode = GameMode.GAMEMODE_TITLE;
            GlobalData.InitCurrentData();
        }
    }

    #endregion
}
