using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    private EnemyStatus enemyStatus;
    private Transform enemyParent;

    private void Start()
    {
        enemyStatus = GetComponent<EnemyStatus>();
        enemyParent = GameObject.Find("CHARACTER").transform.Find("Enemy");
    }

    private void Update()
    {
        if (enemyStatus.GetEnemyCurrentHP() <= 0.0f)
        {
            StopAllCoroutines();
            
            Destroy();
        }
    }

    public void Destroy()
    {
        transform.position = new Vector2(0.0f, 0.0f);
        gameObject.layer = LayerMask.NameToLayer("Default");
        transform.SetParent(enemyParent);
        transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        GetComponent<CircleCollider2D>().radius = 0.5f;
        GetComponent<Animator>().runtimeAnimatorController = null;

        EnemyStatus enemyStatus = GetComponent<EnemyStatus>();
        enemyStatus.SetEnemyType(EnemyType.ENEMYTYPE_NONE);
        enemyStatus.SetEnemyMaxHP(1.0f);
        enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHP());

        // 개별 추가한 스크립트 전부 삭제
        DestroyScript();

        gameObject.SetActive(false);
    }

    public IEnumerator EnemyAutoDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy();
    }

    public void DestroyScript()
    {
        // 개별 패턴 작성하는 대로 여기에 전부 추가하기 !!!
        Destroy(GetComponent<Minion_Pattern1>());
    }
}
