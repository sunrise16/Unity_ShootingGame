using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private GameObject grazeCircle;
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private PlayerStatus playerStatus;

    private Vector2 margin;                     // 인게임 화면 테두리에서의 마진값 (화면 바깥으로 벗어나지 못하게)
    private Vector2 moveSpeedVector;            // 이동 속도 벡터 (Rigidbody2D.velocity를 사용하기 때문에 필요)

    private void Start()
    {
        grazeCircle = GameObject.Find("GrazeCircle").gameObject;
        animator = GameObject.Find("Body").GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();

        margin = new Vector2(0.03f, 0.03f);
        moveSpeedVector = Vector2.zero;
    }
    
    private void Update()
    {
        // 그레이즈 인식 범위를 플레이어 본체에 고정
        grazeCircle.transform.position = transform.position;

        // 피탄 시 이동 관련 변수값 초기화
        if (playerStatus.GetSpriteOff().Equals(true))
        {
            playerStatus.SetSlowMode(false);
            playerStatus.SetPlayerMoveSpeed(0.0f);
            moveSpeedVector = Vector2.zero;
            rigidbody2D.velocity = moveSpeedVector;
        }

        if (playerStatus.GetSpriteOff().Equals(false) && playerStatus.GetRespawn().Equals(false))
        {
            // 저속 이동 시 1.5f 속도, 고속 이동 시 4.0f 속도
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerStatus.SetSlowMode(true);
                playerStatus.SetPlayerMoveSpeed(1.5f);
            }
            else
            {
                playerStatus.SetSlowMode(false);
                playerStatus.SetPlayerMoveSpeed(4.0f);
            }

            // 상하좌우 입력 시 이동 처리
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveSpeedVector.x = -playerStatus.GetPlayerMoveSpeed();
                animator.SetTrigger("isLeftMove");
                animator.ResetTrigger("isIdle");
                animator.ResetTrigger("isRightMove");
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveSpeedVector.x = playerStatus.GetPlayerMoveSpeed();
                animator.SetTrigger("isRightMove");
                animator.ResetTrigger("isIdle");
                animator.ResetTrigger("isLeftMove");
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveSpeedVector.y = playerStatus.GetPlayerMoveSpeed();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveSpeedVector.y = -playerStatus.GetPlayerMoveSpeed();
            }
            rigidbody2D.velocity = moveSpeedVector;

            // 상하좌우키 뗐을 때 속도 및 애니메이션 초기화
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                moveSpeedVector = Vector2.zero;
                animator.SetTrigger("isIdle");
                animator.ResetTrigger("isLeftMove");
                animator.ResetTrigger("isRightMove");
            }
        }

        // 캐릭터 화면 내 고정
        MoveInScreen();
    }
    
    // 캐릭터를 화면 내에 고정시키는 함수
    private void MoveInScreen()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        position.x = Mathf.Clamp(position.x, 0.0f + margin.x, 1.0f - margin.x);
        position.y = Mathf.Clamp(position.y, 0.0f + margin.y, 1.0f - margin.y);

        transform.position = Camera.main.ViewportToWorldPoint(position);
    }
}
