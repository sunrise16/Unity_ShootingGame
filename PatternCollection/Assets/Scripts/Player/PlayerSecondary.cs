using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSecondary : MonoBehaviour
{
    public GameObject fastMoveModePoint;
    public GameObject slowMoveModePoint;
    private PlayerMove playerMove;

    void Start()
    {
        playerMove = GameObject.Find("PLAYER").GetComponent<PlayerMove>();
        StartCoroutine(SecondaryPosition());
    }

    public IEnumerator SecondaryPosition()
    {
        Vector3 vector = Vector3.zero;

        while (true)
        {
            if (playerMove.isSlowMoveMode.Equals(true))
            {
                transform.position = Vector3.SmoothDamp(transform.position, slowMoveModePoint.transform.position, ref vector, 0.05f);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, fastMoveModePoint.transform.position, ref vector, 0.05f);
            }

            yield return new WaitForEndOfFrame();
        }
	}
}
