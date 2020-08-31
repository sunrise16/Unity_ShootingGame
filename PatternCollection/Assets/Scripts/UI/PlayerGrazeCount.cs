using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlayerGrazeCount : MonoBehaviour
{
    private PlayerDatabase playerDatabase;
    private Text grazeCountText;

    void Start()
    {
        playerDatabase = GameObject.Find("PLAYER").GetComponent<PlayerDatabase>();
        grazeCountText = GetComponent<Text>();
        StartCoroutine(GrazeCount());
    }

    // Update is called once per frame
    public IEnumerator GrazeCount()
    {
        while (true)
        {
            grazeCountText.text = playerDatabase.grazeCount.ToString();

            yield return new WaitForEndOfFrame();
        }
    }
}
