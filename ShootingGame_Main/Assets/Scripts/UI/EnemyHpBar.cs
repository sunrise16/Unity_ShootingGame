using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    private EnemyDatabase enemyDatabase;
    private Image hpBarImage;

	void Start()
    {
        enemyDatabase = GameObject.Find("ENEMY").GetComponent<EnemyDatabase>();

        hpBarImage = GetComponent<Image>();
        StartCoroutine(HpBarUpdate());
	}
	
	public IEnumerator HpBarUpdate()
    {
        while (true)
        {
            hpBarImage.fillAmount = enemyDatabase.enemyCurrentHp / enemyDatabase.enemyMaxHp;

            yield return new WaitForEndOfFrame();
        }
	}
}
