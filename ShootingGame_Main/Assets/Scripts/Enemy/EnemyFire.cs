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
                StartCoroutine(EnemyMinionAttack(enemyPatternNumber, enemyAttackWaitTime, enemyAttackDelayTime, isPatternRepeat, enemyFireCount, enemyAttackRepeatTime));
                break;
            case EnemyType.ENEMYTYPE_BOSS:
                StartCoroutine(EnemyBossAttack());
                break;
            default:
                break;
        }
    }

    #region 패턴 관련

    #region 미니언 패턴

    public IEnumerator EnemyMinionAttack(int enemyPatternNumber, float waitTime, float delayTime, bool isPatternRepeat, int fireCount, float repeatTime)
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
                    StartCoroutine(string.Format("Minion_Pattern{0}_Lunatic", enemyPatternNumber));
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

    // 패턴 1
    public IEnumerator Minion_Pattern1_Lunatic()
    {
        Vector2 bulletFirePosition = transform.position;

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
            3, player.transform.position, 0.0f);
    }

    // 패턴 2
    public IEnumerator Minion_Pattern2_Lunatic()
    {
        Vector2 bulletFirePosition = transform.position;

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
                3, player.transform.position, (6.0f * i) - 24.0f);
        }
    }

    // 패턴 3
    public IEnumerator Minion_Pattern3_Lunatic()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        GameObject effect = effectPool.GetChild(0).gameObject;
        bulletEffectManager.CreateBulletFireEffect(effect, 6, 0.5f, 0.18f, 0.5f, bulletFirePosition);

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사
        for (int i = 0; i < 36; i++)
        {
            GameObject bullet = bulletPool[1].GetChild(i).gameObject;
            bulletEffectManager.CapsuleBulletFire
                (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent[1], 0.04f, 0.06f, 0.0f, 0.0f, 1.0f, 110,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL, (i % 6 < 3) ? 7.0f - (1.0f * (i % 3)) : 4.0f + (1.0f * (i % 3)),
                0.0f, 0.0f, 0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, 10.0f * i);
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

    #endregion
}
