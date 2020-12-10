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
                StartCoroutine(EnemyBossAttack());
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

        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            yield return null;

            switch (GameData.gameDifficulty)
            {
                case GameDifficulty.DIFFICULTY_EASY:
                    StartCoroutine(string.Format("Minion_Pattern{0}_Easy", enemyPatternNumber));
                    break;
                case GameDifficulty.DIFFICULTY_NORMAL:
                    StartCoroutine(string.Format("Minion_Pattern{0}_Normal", enemyPatternNumber));
                    break;
                case GameDifficulty.DIFFICULTY_HARD:
                    StartCoroutine(string.Format("Minion_Pattern{0}_Hard", enemyPatternNumber));
                    break;
                case GameDifficulty.DIFFICULTY_LUNATIC:
                    // StartCoroutine(string.Format("Minion_Pattern{0}_Lunatic", enemyPatternNumber));
                    MinionPattern_Lunatic(enemyPatternNumber, customPatternNumber);
                    break;
                case GameDifficulty.DIFFICULTY_EXTRA:
                    StartCoroutine(string.Format("Minion_Pattern{0}_Extra", enemyPatternNumber));
                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(delayTime);

            if (isPatternRepeat.Equals(true))
            {
                count++;

                if (count >= fireCount)
                {
                    count = 0;

                    if (isPatternOnce.Equals(true))
                    {
                        break;
                    }

                    yield return new WaitForSeconds(repeatTime);
                }
            }
        }
    }

    #region Lunatic

    public void MinionPattern_Lunatic(int enemyPatternNumber, int customPatternNumber = 0)
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
                StartCoroutine(MinionPattern_Lunatic3(customPatternNumber));
                break;
            case 4:
                StartCoroutine(MinionPattern_Lunatic4(customPatternNumber));
                break;
            default:
                break;
        }
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
            bulletParent[0], 0.04f, 1.0f, 20,
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
                bulletParent[0], 0.03f, 1.0f, 35,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f,
                0.0f, 0.0f, 1.0f, 3.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, (6.0f * i) - 24.0f, customPatternNumber);
        }
    }

    // 패턴 3
    public IEnumerator MinionPattern_Lunatic3(int customPatternNumber = 0)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(23);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 6, 0.5f, 0.2f, 0.5f, bulletFirePosition);

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사
        for (int i = 0; i < 108; i++)
        {
            GameObject bullet = bulletPool[1].GetChild(i).gameObject;
            bulletEffectManager.CapsuleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[1], 0.04f, 0.06f, 0.0f, 0.0f, 1.0f, 110,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL,
                (i % 6 < 3) ? 7.0f - (1.0f * (i % 3)) - ((0.8f * (i / 36)) - (0.2f * (i / 36))) : 4.0f + (1.0f * (i % 3)) - ((0.8f * (i / 36)) - (0.2f * (i / 36))),
                0.0f, 0.0f, 0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, 10.0f * i, customPatternNumber);
        }
    }

    // 패턴 4
    public IEnumerator MinionPattern_Lunatic4(int customPatternNumber = 0)
    {
        Vector2 bulletFirePosition = transform.position;

        // 효과음 재생
        SoundManager.instance.PlaySE(23);

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 4, 0.5f, 0.25f, 0.5f, bulletFirePosition);

        yield return new WaitForSeconds(0.25f);

        // 탄막 1 발사
        for (int i = 0; i < 16; i++)
        {
            GameObject bullet = bulletPool[1].GetChild(i).gameObject;
            bulletEffectManager.CapsuleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[1], 0.04f, 0.06f, 0.0f, 0.0f, 1.0f, 110,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL,
                (i % 6 < 3) ? 7.0f - (1.0f * (i % 3)) - ((0.8f * (i / 36)) - (0.2f * (i / 36))) : 4.0f + (1.0f * (i % 3)) - ((0.8f * (i / 36)) - (0.2f * (i / 36))),
                0.0f, 0.0f, 0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, 10.0f * i, customPatternNumber);
        }
    }

    #endregion

    #endregion

    #region 보스 패턴

    public IEnumerator EnemyBossAttack()
    {
        // 임시
        return null;
    }

    #endregion

    #endregion

    #region GET, SET

    public int GetEnemyPatternNumber()
    {
        return enemyPatternNumber;
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
