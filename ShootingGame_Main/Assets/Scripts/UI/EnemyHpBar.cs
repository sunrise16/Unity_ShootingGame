using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    private EnemyStatus enemyDatabase;
    private Image hpBarImage;

	private void Start()
    {
        enemyDatabase = GameObject.Find("ENEMY").GetComponent<EnemyStatus>();

        hpBarImage = GetComponent<Image>();
        StartCoroutine(HpBarUpdate());
	}
	
	public IEnumerator HpBarUpdate()
    {
        while (true)
        {
            hpBarImage.fillAmount = enemyDatabase.GetEnemyCurrentHP() / enemyDatabase.GetEnemyMaxHP();

            yield return new WaitForEndOfFrame();
        }
	}
}
