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
    }

    // Update is called once per frame
    void Update()
    {
        grazeCountText.text = playerDatabase.grazeCount.ToString();
    }
}
