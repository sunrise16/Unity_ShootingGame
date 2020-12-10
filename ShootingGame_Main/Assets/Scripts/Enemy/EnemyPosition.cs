using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyPosition : MonoBehaviour
{
    private Vector2 margin;                     // 인게임 화면 테두리에서의 마진값 (화면 바깥으로 벗어나지 못하게)

    private void Start()
    {
        margin = new Vector2(0.03f, 0.03f);
    }

    private void Update()
    {
        // 적 화면 내 고정
        MoveInScreen();
    }

    // 적을 화면 내에 고정시키는 함수
    private void MoveInScreen()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        position.x = Mathf.Clamp(position.x, 0.0f + margin.x, 1.0f - margin.x);
        position.y = Mathf.Clamp(position.y, 0.0f + margin.y, 1.0f - margin.y);

        transform.position = Camera.main.ViewportToWorldPoint(position);
    }
}
