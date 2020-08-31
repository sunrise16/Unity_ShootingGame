using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlayerHitCount : MonoBehaviour
{
    private PlayerDatabase playerDatabase;
    private Text hitCountText;
    
	void Start()
    {
        playerDatabase = GameObject.Find("PLAYER").GetComponent<PlayerDatabase>();
        hitCountText = GetComponent<Text>();
        StartCoroutine(HitCountText());
	}
	
	public IEnumerator HitCountText()
    {
        while (true)
        {
            hitCountText.text = playerDatabase.hitCount.ToString();

            yield return new WaitForEndOfFrame();
        }
	}
}
