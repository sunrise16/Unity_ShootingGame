using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private BulletEffectManager bulletEffectManager;
    private EnemyStatus enemyStatus;
    private GameObject player;
    private Transform[] bulletPool;
    private Transform[] bulletParent;
    private Transform effectPool;
    private Transform effectParent;

    private int enemyPatternNumber;
    private int enemyCounterPatternNumber;
    private float enemyAttackWaitTime;
    private float enemyAttackDelayTime;
    private bool isPatternOnce;
    private bool isPatternRepeat;
    private int enemyFireCount;
    private float enemyAttackRepeatTime;
    private int enemyCustomPatternNumber;

    private void Start()
    {
        bulletEffectManager = GameObject.Find("MANAGER").transform.Find("BulletEffectManager").GetComponent<BulletEffectManager>();
        enemyStatus = GetComponent<EnemyStatus>();
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;
        bulletPool = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            bulletPool[i] = GameObject.Find("BULLET").transform.Find("EnemyBullet").GetChild(i);
        }
        bulletParent = new Transform[10];
        for (int i = 0; i < 10; i++)
        {
            bulletParent[i] = GameObject.Find("BULLET").transform.Find("EnemyBullet_Temp").transform.Find(string.Format("EnemyBullet_Temp{0}", i + 1));
        }
        effectPool = GameObject.Find("EFFECT").transform.Find("Effect");
        effectParent = GameObject.Find("EFFECT").transform.Find("Effect_Temp");
        
        switch (enemyStatus.GetEnemyType())
        {
            case EnemyType.ENEMYTYPE_SMINION:
            case EnemyType.ENEMYTYPE_MMINION:
            case EnemyType.ENEMYTYPE_LMINION:
                StartCoroutine(EnemyMinionAttack(enemyPatternNumber, enemyAttackWaitTime, enemyAttackDelayTime, isPatternRepeat, enemyFireCount, enemyAttackRepeatTime, enemyCustomPatternNumber));
                break;
            case EnemyType.ENEMYTYPE_BOSS:
                // 임시
                StartCoroutine(EnemyBossAttack(enemyPatternNumber, enemyAttackWaitTime, enemyAttackDelayTime, isPatternRepeat, enemyFireCount, enemyAttackRepeatTime, enemyCustomPatternNumber));
                break;
            default:
                break;
        }
    }

    #region 패턴 관련

    #region 미니언 패턴

    public IEnumerator EnemyMinionAttack(int enemyPatternNumber, float waitTime, float delayTime, bool isPatternRepeat, int fireCount, float repeatTime, int customPatternNumber = 0)
    {
        int count = 0;

        // 패턴 시작 전 최초 대기 시간
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            yield return null;

            // 적이 카메라 영역 안에 있을 경우 (화면 바깥으로 벗어나지 않았을 때)
            if (enemyStatus.GetScreenOut().Equals(false))
            {
                // 난이도에 따라 패턴 코루틴 시작
                switch (GameData.gameDifficulty)
                {
                    case GameDifficulty.DIFFICULTY_EASY:
                        // StartCoroutine(string.Format("Minion_Pattern{0}_Easy", enemyPatternNumber));
                        break;
                    case GameDifficulty.DIFFICULTY_NORMAL:
                        // StartCoroutine(string.Format("Minion_Pattern{0}_Normal", enemyPatternNumber));
                        break;
                    case GameDifficulty.DIFFICULTY_HARD:
                        // StartCoroutine(string.Format("Minion_Pattern{0}_Hard", enemyPatternNumber));
                        break;
                    case GameDifficulty.DIFFICULTY_LUNATIC:
                        StartCoroutine(MinionPattern_Lunatic(enemyPatternNumber, count, customPatternNumber));
                        break;
                    case GameDifficulty.DIFFICULTY_EXTRA:
                        // StartCoroutine(string.Format("Minion_Pattern{0}_Extra", enemyPatternNumber));
                        break;
                    default:
                        break;
                }
            }

            // 다음 탄막 발사 간 대기 시간
            yield return new WaitForSeconds(delayTime);

            // 일정 시간 대기 후 패턴 반복 체크되어 있을 경우
            if (isPatternRepeat.Equals(true))
            {
                // 탄막 발사 횟수 증가
                count++;

                // 탄막 발사 횟수가 특정 횟수에 도달했을 경우
                if (count >= fireCount)
                {
                    count = 0;

                    // 패턴 1회만 반복 체크되어 있을 경우 패턴 종료
                    if (isPatternOnce.Equals(true))
                    {
                        break;
                    }

                    // 다음 패턴 반복까지의 대기 시간
                    yield return new WaitForSeconds(repeatTime);
                }
            }
        }
    }

    #region Lunatic

    public IEnumerator MinionPattern_Lunatic(int enemyPatternNumber, int fireCount = 0, int customPatternNumber = 0)
    {
        switch (enemyPatternNumber)
        {
            case 1:
                StartCoroutine(MinionPattern_Lunatic1(customPatternNumber));
                break;
            case 2:
                StartCoroutine(MinionPattern_Lunatic2(customPatternNumber));
                break;
            case 3:
                Vector3 playerPosition = player.transform.position;
                for (int i = 0; i < 30; i++)
                {
                    StartCoroutine(MinionPattern_Lunatic3(i, playerPosition, customPatternNumber));
                    yield return new WaitForSeconds(0.004f);
                }
                break;
            case 4:
                StartCoroutine(MinionPattern_Lunatic4(customPatternNumber));
                break;
            case 5:
                // StartCoroutine(MinionPattern_Lunatic5(customPatternNumber));
                break;
            default:
                break;
        }

        yield return null;
    }

    // 패턴 1
    public IEnumerator MinionPattern_Lunatic1(int customPatternNumber = 0)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(23);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 3, 0.6f, 0.1f, 0.6f, bulletFirePosition, 4.0f, 4.0f);

        yield return new WaitForSeconds(0.1f);

        // 탄막 1 발사
        GameObject bullet = bulletPool[0].GetChild(0).gameObject;
        bulletEffectManager.CircleBulletFire
            (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
            bulletParent[0], 0.04f, 0.0f, 0.0f, 1.0f, 20, false, 0,
            BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING, 4.0f,
            0.1f, 7.0f, 0.0f, 0.0f, false, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            3, player.transform.position, 0.0f, customPatternNumber);
    }

    // 패턴 2
    public IEnumerator MinionPattern_Lunatic2(int customPatternNumber = 0)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(42);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 1, 0.5f, 0.15f, 0.5f, bulletFirePosition, 4.0f, 4.0f);

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사
        for (int i = 0; i < 9; i++)
        {
            GameObject bullet = bulletPool[0].GetChild(i).gameObject;
            bulletEffectManager.CircleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[0], 0.03f, 0.0f, 0.0f, 1.0f, 35, false, 0,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f,
                0.0f, 0.0f, 1.0f, 3.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, (6.0f * i) - 24.0f, customPatternNumber);
        }
    }

    // 패턴 3
    public IEnumerator MinionPattern_Lunatic3(int fireCount, Vector3 playerPosition, int customPatternNumber = 0)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(23);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 6, 0.5f, 0.2f, 0.5f, bulletFirePosition);

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사
        for (int i = 0; i < 4; i++)
        {
            GameObject bullet = bulletPool[1].GetChild(i).gameObject;
            bulletEffectManager.CapsuleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[1], 0.04f, 0.06f, 0.0f, 0.0f, 1.0f, 110, false, 0,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL,
                7.0f - (1.5f * (i % 2)),
                0.0f, 0.0f, 0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                1, playerPosition, (enemyStatus.GetEnemyNumber().Equals(0) ? (6.0f * fireCount) : (-6.0f * fireCount)) + (i < 2 ? 0.0f : 180.0f),
                customPatternNumber);
        }
    }
    
    // 패턴 4 (커스텀 스크립트 有)
    public IEnumerator MinionPattern_Lunatic4(int customPatternNumber)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(42);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 4, 0.6f, 0.18f, 0.6f, bulletFirePosition);

        yield return new WaitForSeconds(0.18f);

        // 탄막 1 발사
        for (int i = 0; i < 8; i++)
        {
            GameObject bullet = bulletPool[1].GetChild(i).gameObject;
            bulletEffectManager.CapsuleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[0], 0.02f, 0.055f, 0.0f, 0.0f, 1.0f, 88, false, 0,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 6.0f,
                0.0f, 0.0f, 0.2f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, 45.0f * i, customPatternNumber);

            BulletReaimingSetting(bullet, 0, 23, 0.0f, BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 3.0f, BulletRotateState.BULLETROTATESTATE_NONE,
                0.0f, 0.0f, true, false, true, false);
        }
    }

    #endregion

    #endregion

    #region 보스 패턴

    public IEnumerator EnemyBossAttack(int enemyPatternNumber, float waitTime, float delayTime, bool isPatternRepeat, int fireCount, float repeatTime, int customPatternNumber)
    {
        int count = 0;

        // 패턴 시작 전 최초 대기 시간
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            yield return null;

            // 적이 카메라 영역 안에 있을 경우 (화면 바깥으로 벗어나지 않았을 때)
            if (enemyStatus.GetScreenOut().Equals(false))
            {
                // 난이도에 따라 패턴 코루틴 시작
                switch (GameData.gameDifficulty)
                {
                    case GameDifficulty.DIFFICULTY_EASY:
                        break;
                    case GameDifficulty.DIFFICULTY_NORMAL:
                        break;
                    case GameDifficulty.DIFFICULTY_HARD:
                        break;
                    case GameDifficulty.DIFFICULTY_LUNATIC:
                        StartCoroutine(BossPattern_Lunatic(enemyPatternNumber, count, customPatternNumber));
                        break;
                    case GameDifficulty.DIFFICULTY_EXTRA:
                        break;
                    default:
                        break;
                }
            }

            // 다음 탄막 발사 간 대기 시간
            yield return new WaitForSeconds(delayTime);

            // 일정 시간 대기 후 패턴 반복 체크되어 있을 경우
            if (isPatternRepeat.Equals(true))
            {
                // 탄막 발사 횟수 증가
                count++;

                // 탄막 발사 횟수 제한이 -1로 설정되어 있을 경우 count 증가 무한 반복
                if (!fireCount.Equals(-1))
                {
                    // 탄막 발사 횟수가 특정 횟수에 도달했을 경우
                    if (count >= fireCount)
                    {
                        count = 0;

                        // 패턴 1회만 반복 체크되어 있을 경우 패턴 종료
                        if (isPatternOnce.Equals(true))
                        {
                            break;
                        }

                        // 다음 패턴 반복까지의 대기 시간
                        yield return new WaitForSeconds(repeatTime);
                    }
                }
            }
        }
    }

    #region Lunatic

    public IEnumerator BossPattern_Lunatic(int enemyPatternNumber, int fireCount = 0, int customPatternNumber = 0)
    {
        switch (enemyPatternNumber)
        {
            case 1:
                StartCoroutine(BossPattern_Lunatic1(fireCount, customPatternNumber));
                break;
            default:
                break;
        }

        yield return null;
    }

    // 패턴 1
    public IEnumerator BossPattern_Lunatic1(int fireCount, int customPatternNumber = 0)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(23);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 1, 0.45f, 0.15f, 0.45f, bulletFirePosition, 4.0f, 4.0f);

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사
        for (int i = 0; i < 8; i++)
        {
            GameObject bullet = bulletPool[0].GetChild(0).gameObject;
            bulletEffectManager.CircleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER2"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[0], 0.04f, 0.0f, 0.03f, 1.0f, 462, true, 0,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 3.0f + (1.0f * (i % 4)),
                0.0f, 0.0f, 0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                1, new Vector2(transform.position.x, transform.position.y - 5.0f), i < 4 ? 2.0f + (7.0f * fireCount) : -2.0f + (-7.0f * fireCount), customPatternNumber);
        }

        if ((fireCount % 6).Equals(0))
        {
            // 탄막 2 발사
            for (int i = 0; i < 32; i++)
            {
                GameObject bullet = bulletPool[0].GetChild(0).gameObject;
                bulletEffectManager.CircleBulletFire
                    (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                    bulletParent[1], 0.02f, 0.0f, 0.0f, 1.0f, 267, false, 0,
                    BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL, i < 16 ? 6.5f : 5.0f,
                    0.0f, 0.0f, 0.0f, 0.0f, false, false,
                    BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                    3, player.transform.position, 22.5f * (i % 16) + (11.25f * (i / 16)), customPatternNumber);
            }
        }
    }

    #endregion

    #endregion

    #region 반격탄 패턴

    public void EnemyCounter(int counterPatternNumber)
    {
        switch (counterPatternNumber)
        {
            case 1:
                switch (GameData.gameDifficulty)
                {
                    case GameDifficulty.DIFFICULTY_EASY:
                        break;
                    case GameDifficulty.DIFFICULTY_NORMAL:
                        break;
                    case GameDifficulty.DIFFICULTY_HARD:
                        break;
                    case GameDifficulty.DIFFICULTY_LUNATIC:
                        StartCoroutine(EnemyCounter_Pattern1_Lunatic());
                        break;
                    case GameDifficulty.DIFFICULTY_EXTRA:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    #region Lunatic

    private IEnumerator EnemyCounter_Pattern1_Lunatic()
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(42);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 5, 0.65f, 0.15f, 0.65f, bulletFirePosition);

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사
        for (int i = 0; i < 28; i++)
        {
            GameObject bullet = bulletPool[0].GetChild(i).gameObject;
            bulletEffectManager.CircleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[1], 0.02f, 0.0f, 0.0f, 1.0f, (i % 4) + 6, false, 0,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL, Random.Range(4.0f, 8.0f),
                0.0f, 0.0f, 0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                2, player.transform.position, 0.0f);
        }
    }

    #endregion

    #endregion

    #region 커스텀 스크립트 변수 설정

    private void BulletReaimingSetting(GameObject bullet, int repeatCount, int soundNumber, float waitTime,
        BulletSpeedState bulletSpeedState, float bulletMoveSpeed, BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        bool isPlayerAimed, bool isRandomAimed, bool isSpeedDown, bool isTimer)
    {
        BulletReaiming bulletReaiming = bullet.GetComponent<BulletReaiming>();
        bulletReaiming.repeatCount = repeatCount;
        bulletReaiming.soundNumber = soundNumber;
        bulletReaiming.waitTime = waitTime;
        bulletReaiming.bulletSpeedState = bulletSpeedState;
        bulletReaiming.bulletMoveSpeed = bulletMoveSpeed;
        bulletReaiming.bulletRotateState = bulletRotateState;
        bulletReaiming.bulletRotateSpeed = bulletRotateSpeed;
        bulletReaiming.bulletRotateLimit = bulletRotateLimit;
        bulletReaiming.isPlayerAimed = isPlayerAimed;
        bulletReaiming.isRandomAimed = isRandomAimed;
        bulletReaiming.isSpeedDown = isSpeedDown;
        bulletReaiming.isTimer = isTimer;
    }

    #endregion

    #endregion

    #region GET, SET

    public int GetEnemyPatternNumber()
    {
        return enemyPatternNumber;
    }

    public int GetEnemyCounterPatternNumber()
    {
        return enemyCounterPatternNumber;
    }

    public float GetEnemyAttackWaitTime()
    {
        return enemyAttackWaitTime;
    }

    public float GetEnemyAttackDelayTime()
    {
        return enemyAttackDelayTime;
    }

    public bool GetEnemyPatternOnce()
    {
        return isPatternOnce;
    }

    public bool GetEnemyPatternRepeat()
    {
        return isPatternRepeat;
    }

    public int GetEnemyFireCount()
    {
        return enemyFireCount;
    }

    public float GetEnemyAttackRepeatTime()
    {
        return enemyAttackRepeatTime;
    }

    public int GetEnemyCustomPatternNumber()
    {
        return enemyCustomPatternNumber;
    }
    
    public void SetEnemyPatternNumber(int number)
    {
        enemyPatternNumber = number;
    }

    public void SetEnemyCounterPatternNumber(int number)
    {
        enemyCounterPatternNumber = number;
    }

    public void SetEnemyAttackWaitTime(float time)
    {
        enemyAttackWaitTime = time;
    }

    public void SetEnemyAttackDelayTime(float time)
    {
        enemyAttackDelayTime = time;
    }

    public void SetEnemyPatternOnce(bool once)
    {
        isPatternOnce = once;
    }

    public void SetEnemyPatternRepeat(bool repeat)
    {
        isPatternRepeat = repeat;
    }

    public void SetEnemyFireCount(int count)
    {
        enemyFireCount = count;
    }

    public void SetEnemyAttackRepeatTime(float time)
    {
        enemyAttackRepeatTime = time;
    }

    public void SetEnemyCustomPatternNumber(int number)
    {
        enemyCustomPatternNumber = number;
    }

    #endregion
}
