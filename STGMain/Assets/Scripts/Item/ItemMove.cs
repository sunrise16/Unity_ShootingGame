using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemMove : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private ItemStatus itemStatus;
    private GameObject player;
    private PlayerStatus playerStatus;

    private float itemSpeed;                    // 아이템 떨어지는 속도
    
	private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        itemStatus = GetComponent<ItemStatus>();
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;
        playerStatus = player.GetComponent<PlayerStatus>();

        itemSpeed = 2.0f;

        // 최초 아이템 드랍 시 화면 위쪽으로 힘을 가해줌
        rigidbody2D.AddForce(Vector2.up * 100.0f);
	}
	
    private void Update()
    {
        // 자동 회수 처리 시 플레이어에게 향하게 함
        if (itemStatus.GetPlayerFind().Equals(true))
        {
            if (itemSpeed < 25.0f)
            {
                itemSpeed += 0.75f;
            }
            rigidbody2D.gravityScale = 0.0f;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, itemSpeed * Time.deltaTime);
        }
        // 자동 회수 처리중이 아닐 경우 중력값 및 아이템 속도 기본값으로 설정
        else
        {
            rigidbody2D.gravityScale = 0.1f;
            itemSpeed = 2.0f;
        }
    }
}
