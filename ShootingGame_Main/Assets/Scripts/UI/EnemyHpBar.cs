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
	}
	
	private void Update()
    {
        // HP바 이미지 = 적의 현재 체력 / 적의 최대 체력
        hpBarImage.fillAmount = enemyDatabase.GetEnemyCurrentHP() / enemyDatabase.GetEnemyMaxHP();
    }
}
