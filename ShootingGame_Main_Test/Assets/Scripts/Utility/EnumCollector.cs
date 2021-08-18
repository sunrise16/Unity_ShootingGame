﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// 게임 종류 분류
public enum GameType
{
    GAMETYPE_NONE,
    GAMETYPE_TH6,       // 원본게임 탄막
    GAMETYPE_TH7,
    GAMETYPE_TH8,
    GAMETYPE_TH10,
    GAMETYPE_TH11,
    GAMETYPE_TH12,
    GAMETYPE_TH13,
    GAMETYPE_TH14,
    GAMETYPE_TH15,
    GAMETYPE_TH16,
    GAMETYPE_TH17,
    GAMETYPE_ORI1,      // 오리지널 탄막
    GAMETYPE_ORI2,
}

// 게임 난이도 분류
public enum GameDifficulty
{
    DIFFICULTY_NONE,
    DIFFICULTY_EASY,
    DIFFICULTY_NORMAL,
    DIFFICULTY_HARD,
    DIFFICULTY_LUNATIC,
    DIFFICULTY_EXTRA,
}

// 게임 내 현재 씬 분류
public enum GameMode
{
    GAMEMODE_NONE,
    GAMEMODE_TITLE,
    GAMEMODE_MAINGAME,
    GAMEMODE_ENDING,
    GAMEMODE_STAFFROLL,
    GAMEMODE_RANKING,
    GAMEMODE_PRACTICE,
    GAMEMODE_REPLAY,
}

// 연습 모드에서의 현재 스테이지 분류
public enum GamePracticeStage
{
    GAMEPRACTICESTAGE_NONE,
    GAMEPRACTICESTAGE_STAGE1,
    GAMEPRACTICESTAGE_STAGE2,
    GAMEPRACTICESTAGE_STAGE3,
    GAMEPRACTICESTAGE_STAGE4,
    GAMEPRACTICESTAGE_STAGE5,
    GAMEPRACTICESTAGE_STAGE6,
}

// 게임 결과 등급 분류 (사용할 일이 있을지는 모르겠음)
public enum GameGrade
{
    GAMEGRADE_NONE,
    GAMEGRADE_F,
    GAMEGRADE_D,
    GAMEGRADE_C,
    GAMEGRADE_B,
    GAMEGRADE_A,
    GAMEGRADE_S,
}

// 적 종류 분류
public enum EnemyType
{
    ENEMYTYPE_NONE,
    ENEMYTYPE_SMINION,
    ENEMYTYPE_MMINION,
    ENEMYTYPE_LMINION,
    ENEMYTYPE_BOSS,
}

// 아이템 사이즈 분류
public enum ItemSize
{
    ITEMSIZE_NONE,
    ITEMSIZE_SMALL,
    ITEMSIZE_MEDIUM,
    ITEMSIZE_LARGE,
}

// 아이템 종류 분류
public enum ItemType
{
    ITEMTYPE_NONE,
    ITEMTYPE_POWER,
    ITEMTYPE_SCORE,
    ITEMTYPE_LIFE,
    ITEMTYPE_LIFEFRAGMENT,
    ITEMTYPE_SPELL,
    ITEMTYPE_SPELLFRAGMENT,
    ITEMTYPE_FULLPOWER,
}

// 탄막 종류 분류
public enum BulletType
{
    BULLETTYPE_NONE,
    BULLETTYPE_EMPTY,
    BULLETTYPE_NORMAL,
    BULLETTYPE_LASER_HOLD,
    BULLETTYPE_LASER_MOVE,
}

// 탄막 반사 성질 분류
public enum BulletReflectState
{
    BULLETREFLECTSTATE_NONE,
    BULLETREFLECTSTATE_NORMAL,
    BULLETREFLECTSTATE_CONTAINBOTTOM,
}

// 탄막 속도 성질 분류
public enum BulletSpeedState
{
    BULLETSPEEDSTATE_NONE,
    BULLETSPEEDSTATE_NORMAL,
    BULLETSPEEDSTATE_ACCELERATING,
    BULLETSPEEDSTATE_DECELERATING,
    BULLETSPEEDSTATE_LOOP,
    BULLETSPEEDSTATE_LOOPONCE,
}

// 탄막 회전 성질 분류
public enum BulletRotateState
{
    BULLETROTATESTATE_NONE,
    BULLETROTATESTATE_NORMAL,
    BULLETROTATESTATE_LIMIT,
    BULLETROTATESTATE_ROTATEAROUND,
    BULLETROTATESTATE_LOOKAT,
    BULLETROTATESTATE_LOOPSIN,
    BULLETROTATESTATE_LOOPCOS,
}