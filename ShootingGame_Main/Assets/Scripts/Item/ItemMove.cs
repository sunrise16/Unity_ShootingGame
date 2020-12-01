using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemMove : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private ItemStatus itemStatus;
    private GameObject player;
    private PlayerStatus playerStatus;

    private float itemSpeed;
    
	private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        itemStatus = GetComponent<ItemStatus>();
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;
        playerStatus = player.GetComponent<PlayerStatus>();

        itemSpeed = 2.0f;

        rigidbody2D.AddForce(Vector2.up * 60.0f);
	}
	
    private void Update()
    {
        if (itemStatus.GetPlayerFind().Equals(true) || playerStatus.GetAutoCollect().Equals(true))
        {
            if (itemSpeed < 15.0f)
            {
                itemSpeed += 0.25f;
            }
            rigidbody2D.gravityScale = 0.0f;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, itemSpeed * Time.deltaTime);
        }
        else
        {
            rigidbody2D.gravityScale = 0.1f;
            itemSpeed = 2.0f;
        }
    }
}
