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

        gameObject.SetActive(false);
    }

    public IEnumerator EnemyAutoDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy();
    }
}
