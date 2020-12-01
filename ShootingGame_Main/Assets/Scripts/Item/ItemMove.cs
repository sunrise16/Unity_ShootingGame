using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemMove : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private ItemStatus itemStatus;
    private GameObject player;

    private float itemSpeed;
    
	private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        itemStatus = GetComponent<ItemStatus>();
        player = GameObject.Find("PLAYER");

        itemSpeed = 2.0f;

        rigidbody2D.AddForce(Vector2.up * 2.0f);
	}
	
    private void Update()
    {
        if (itemStatus.GetPlayerFind().Equals(true))
        {
            if (itemSpeed < 5.0f)
            {
                itemSpeed += 0.1f;
            }
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, itemSpeed * Time.deltaTime);
        }
        else
        {
            itemSpeed = 2.0f;
        }
    }
}
