using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public sealed class GameData : MonoBehaviour
{
    public static GameDifficulty gameDifficulty = GameDifficulty.DIFFICULTY_NONE;                   // 현재 게임 난이도
    public static GameMode gameMode = GameMode.GAMEMODE_NONE;                                       // 현재 게임 모드
    public static GamePracticeStage gamePracticeStage = GamePracticeStage.GAMEPRACTICESTAGE_NONE;   // 현재 연습 모드 스테이지
    public static GameGrade gameGrade = GameGrade.GAMEGRADE_NONE;                                   // 플레이한 게임의 최종 평가 등급

    public static int currentStage = 0;                 // 현재 진행중인 스테이지
    public static int currentChapter = 0;               // 현재 진행중인 챕터
    public static long currentScore = 0;                // 현재 게임 득점

    public static int currentPlayerLife = 0;            // 현재 플레이어 남은 목숨
    public static int currentPlayerBomb = 0;            // 현재 플레이어 남은 스펠
    public static int currentPlayerLifeFragment = 0;    // 현재 플레이어 남은 목숨 조각
    public static int currentPlayerBombFragment = 0;    // 현재 플레이어 남은 스펠 조각
    public static float currentPower = 0.0f;            // 현재 기체 파워

    public static int currentScoreItem = 0;             // 현재 점수 아이템 획득 수
    public static int currentGraze = 0;                 // 현재 플레이어 그레이즈 횟수
    public static int currentMissCount = 0;             // 현재 미스 횟수
    public static int currentBombUseCount = 0;          // 현재 스펠 사용 횟수
    public static int currentContinueCount = 0;         // 현재 컨티뉴 횟수

    public static long highScore_MainGameA = 0;         // 최고 득점 (메인 게임, 캐릭터 A)
    public static long highScore_MainGameB = 0;         // 최고 득점 (메인 게임, 캐릭터 B)
    public static long highScore_MainGameC = 0;         // 최고 득점 (메인 게임, 캐릭터 C)
    public static long highScore_MainGameD = 0;         // 최고 득점 (메인 게임, 캐릭터 B)

    public static long highScore_PracticeStage1A = 0;   // 최고 득점 (연습 모드 스테이지 1, 캐릭터 A)
    public static long highScore_PracticeStage1B = 0;   // 최고 득점 (연습 모드 스테이지 1, 캐릭터 B)
    public static long highScore_PracticeStage1C = 0;   // 최고 득점 (연습 모드 스테이지 1, 캐릭터 C)
    public static long highScore_PracticeStage1D = 0;   // 최고 득점 (연습 모드 스테이지 1, 캐릭터 D)

    public static long highScore_PracticeStage2A = 0;   // 최고 득점 (연습 모드 스테이지 2, 캐릭터 A)
    public static long highScore_PracticeStage2B = 0;   // 최고 득점 (연습 모드 스테이지 2, 캐릭터 B)
    public static long highScore_PracticeStage2C = 0;   // 최고 득점 (연습 모드 스테이지 2, 캐릭터 C)
    public static long highScore_PracticeStage2D = 0;   // 최고 득점 (연습 모드 스테이지 2, 캐릭터 D)

    public static long highScore_PracticeStage3A = 0;   // 최고 득점 (연습 모드 스테이지 3, 캐릭터 A)
    public static long highScore_PracticeStage3B = 0;   // 최고 득점 (연습 모드 스테이지 3, 캐릭터 B)
    public static long highScore_PracticeStage3C = 0;   // 최고 득점 (연습 모드 스테이지 3, 캐릭터 C)
    public static long highScore_PracticeStage3D = 0;   // 최고 득점 (연습 모드 스테이지 3, 캐릭터 D)

    public static long highScore_PracticeStage4A = 0;   // 최고 득점 (연습 모드 스테이지 4, 캐릭터 A)
    public static long highScore_PracticeStage4B = 0;   // 최고 득점 (연습 모드 스테이지 4, 캐릭터 B)
    public static long highScore_PracticeStage4C = 0;   // 최고 득점 (연습 모드 스테이지 4, 캐릭터 C)
    public static long highScore_PracticeStage4D = 0;   // 최고 득점 (연습 모드 스테이지 4, 캐릭터 D)

    public static long highScore_PracticeStage5A = 0;   // 최고 득점 (연습 모드 스테이지 5, 캐릭터 A)
    public static long highScore_PracticeStage5B = 0;   // 최고 득점 (연습 모드 스테이지 5, 캐릭터 B)
    public static long highScore_PracticeStage5C = 0;   // 최고 득점 (연습 모드 스테이지 5, 캐릭터 C)
    public static long highScore_PracticeStage5D = 0;   // 최고 득점 (연습 모드 스테이지 5, 캐릭터 D)

    public static long highScore_PracticeStage6A = 0;   // 최고 득점 (연습 모드 스테이지 6, 캐릭터 A)
    public static long highScore_PracticeStage6B = 0;   // 최고 득점 (연습 모드 스테이지 6, 캐릭터 B)
    public static long highScore_PracticeStage6C = 0;   // 최고 득점 (연습 모드 스테이지 6, 캐릭터 C)
    public static long highScore_PracticeStage6D = 0;   // 최고 득점 (연습 모드 스테이지 6, 캐릭터 D)

    public static void InitGameData()
    {
        gameDifficulty = GameDifficulty.DIFFICULTY_NONE;
        gameMode = GameMode.GAMEMODE_NONE;
        gamePracticeStage = GamePracticeStage.GAMEPRACTICESTAGE_NONE;
        gameGrade = GameGrade.GAMEGRADE_NONE;

        currentStage = 0;
        currentChapter = 0;
        currentScore = 0;
        currentPlayerLife = 0;
        currentPlayerLifeFragment = 0;
        currentPlayerBomb = 0;
        currentPlayerBombFragment = 0;
        currentPower = 0.0f;
        currentScoreItem = 0;
        currentGraze = 0;
        currentMissCount = 0;
        currentBombUseCount = 0;
        currentContinueCount = 0;
    }
}
