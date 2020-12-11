using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private Animator animator;
    private Vector3 targetPosition;                 // 적의 이동 목적지

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    #region 적 이동

    // 지정 장소로 1회 이동
    public IEnumerator EnemyMoveOnce(Vector3 position, iTween.EaseType easeType, float moveTime)
    {
        yield return null;

        // 스프라이트 조절
        targetPosition = position;
        SetAnimatorTrigger(targetPosition);

        // 이동 처리
        iTween.MoveTo(gameObject, iTween.Hash("position", position, "easetype", easeType, "time", moveTime));
    }

    // 지정 장소로 곡선을 그리며 1회 이동
    public IEnumerator EnemyMovePathOnce(Vector3[] paths, iTween.EaseType easeType, float moveTime)
    {
        yield return null;

        // 스프라이트 조절
        StartCoroutine(SetAnimatorTrigger(paths));

        // 이동 처리
        iTween.MoveTo(gameObject, iTween.Hash("path", paths, "easetype", easeType, "time", moveTime));
    }

    // 지정 장소 이동 후 일정 시간 지난 뒤 다시 이동 (총 2회)
    public IEnumerator EnemyMoveTwice(Vector3 positionFirst, Vector3 positionSecond, iTween.EaseType easeType1, iTween.EaseType easeType2, float moveTime1, float moveTime2, float waitTime)
    {
        yield return null;
        
        // 스프라이트 조절
        targetPosition = positionFirst;
        SetAnimatorTrigger(targetPosition);

        // 이동 1 처리
        iTween.MoveTo(gameObject, iTween.Hash("position", positionFirst, "easetype", easeType1, "time", moveTime1));

        yield return new WaitForSeconds(waitTime);

        if (gameObject.activeSelf.Equals(true))
        {
            // 스프라이트 조절
            targetPosition = positionSecond;
            SetAnimatorTrigger(targetPosition);

            // 이동 2 처리
            iTween.MoveTo(gameObject, iTween.Hash("position", positionSecond, "easetype", easeType2, "time", moveTime2));
        }
    }

    #endregion

    #region 애니메이터 트리거 설정

    // 스프라이트 애니메이션 조절 함수 (패스 이동)
    public IEnumerator SetAnimatorTrigger(Vector3[] paths)
    {
        int pathNumber = 1;
        SetAnimatorTrigger(paths[pathNumber]);

        while (transform.position.x == paths[paths.Length - 1].x && transform.position.y == paths[paths.Length - 1].y)
        {
            if (transform.position.x == paths[pathNumber].x && transform.position.y == paths[pathNumber].y)
            {
                SetAnimatorTrigger(paths[pathNumber]);
                pathNumber++;
            }
            yield return null;
        }
    }
    // 스프라이트 애니메이션 조절 함수 (단일 좌표 이동)
    public void SetAnimatorTrigger(Vector3 targetPosition)
    {
        // 적의 직전 위치와 현재 위치간의 좌표값 차이에 따라 스프라이트 조절
        if (transform.position.x > targetPosition.x)
        {
            if (!animator.GetBool("isLeftMove").Equals(true))
            {
                animator.SetTrigger("isLeftMove");
            }
        }
        else if (transform.position.x < targetPosition.x)
        {
            if (!animator.GetBool("isRightMove").Equals(true))
            {
                animator.SetTrigger("isRightMove");
            }
        }
        else
        {
            if (!animator.GetBool("isIdle").Equals(true))
            {
                animator.SetTrigger("isIdle");
            }
        }
    }

    #endregion
}
