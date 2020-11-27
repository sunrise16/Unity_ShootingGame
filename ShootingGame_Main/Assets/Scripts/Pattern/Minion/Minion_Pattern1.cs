using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Minion_Pattern1 : MonoBehaviour
{
    private PatternManager patternManager;
    private Transform bulletParent;
    private Transform bullet;
    private GameObject effect;
    private GameObject player;

    private void Start()
    {
        patternManager = GameObject.Find("MANAGER").transform.Find("PatternManager").GetComponent<PatternManager>();
        bulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet_Temp").transform.Find("EnemyBullet_Temp1");
        bullet = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle");
        effect = GameObject.Find("EFFECT").transform.Find("Effect").GetChild(0).gameObject;
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;

        StartCoroutine(Pattern());
    }

    public IEnumerator Pattern()
    {
        while (true)
        {
            switch (GameData.gameDifficulty)
            {
                case GameDifficulty.DIFFICULTY_EASY:
                    break;
                case GameDifficulty.DIFFICULTY_NORMAL:
                    break;
                case GameDifficulty.DIFFICULTY_HARD:
                    break;
                case GameDifficulty.DIFFICULTY_LUNATIC:
                    StartCoroutine(Minion_Pattern1_LunaticAttack1());
                    yield return new WaitForSeconds(1.0f);
                    break;
                default:
                    break;
            }
        }
    }

    #region Lunatic

    public IEnumerator Minion_Pattern1_LunaticAttack1()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        patternManager.CreateBulletFireEffect(effect, 3, 0.5f, 0.25f, 0.4f, bulletFirePosition);

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사
        for (int i = 0; i < 32; i++)
        {
            patternManager.CircleBulletFire
                (bullet.GetChild(i).gameObject, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
                bulletParent, 0.04f, 1.0f, 20,
                BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.5f,
                0.0f, 0.0f,
                0.0f, 0.0f, false, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, player.transform.position, (11.25f * i));
        }
    }

    #endregion
}
