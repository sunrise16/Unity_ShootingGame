using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    private EnemyDatabase enemyDatabase;
    private EnemyFire enemyFire;

    public int stageNumber;
    public bool isStageClear;
    private float waitTime;
    private float waitTimeDelay;

    void Start()
    {
        enemyDatabase = GameObject.Find("ENEMY").GetComponent<EnemyDatabase>();
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();

        stageNumber = 1;
        isStageClear = false;
        waitTime = 2.0f;
        waitTimeDelay = 0.0f;

        StartCoroutine(GameStart());
    }
	
	void Update()
    {
		if (enemyDatabase.enemyCurrentHp <= 0)
        {
            stageNumber++;
            SetEnemyHp();
            isStageClear = true;
            enemyFire.StopAllCoroutines();

            GameObject.Find("ENEMY").transform.position = new Vector2(0.0f, 3.5f);
            GameObject.Find("ENEMY").transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = false;
            GameObject.Find("ENEMY").transform.Find("Body").GetComponent<EnemySprite>().isRightMove = false;
            GameObject.Find("DESTROYZONE").transform.Find("ALLCLEARZONE1").gameObject.SetActive(true);
            GameObject.Find("DESTROYZONE").transform.Find("ALLCLEARZONE2").gameObject.SetActive(true);
        }

        if (isStageClear == true)
        {
            waitTimeDelay += Time.deltaTime;
            if (waitTimeDelay >= waitTime)
            {
                waitTimeDelay = 0.0f;
                isStageClear = false;
                enemyFire.Fire(stageNumber);

                GameObject.Find("DESTROYZONE").transform.Find("ALLCLEARZONE1").gameObject.SetActive(false);
                GameObject.Find("DESTROYZONE").transform.Find("ALLCLEARZONE2").gameObject.SetActive(false);
            }
        }
	}

    private void SetEnemyHp()
    {
        switch (stageNumber)
        {
            case 1: case 2: case 3: case 4: case 5: case 6:
                enemyDatabase.enemyCurrentHp = 1000.0f;
                enemyDatabase.enemyMaxHp = 1000.0f;
                break;
            case 7: case 8: case 9: case 10:
                enemyDatabase.enemyCurrentHp = 1200.0f;
                enemyDatabase.enemyMaxHp = 1200.0f;
                break;
            default:
                enemyDatabase.enemyCurrentHp = 0.0f;
                enemyDatabase.enemyMaxHp = 0.0f;
                break;
        }
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(2.0f);

        enemyFire.Fire(stageNumber);
        StopAllCoroutines();
    }
}
