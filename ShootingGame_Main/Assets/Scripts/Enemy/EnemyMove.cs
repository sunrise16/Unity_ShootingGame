using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    #region 적 이동

    // 지정 장소로 1회 이동
    public IEnumerator EnemyMoveOnce(Vector3 targetPosition, iTween.EaseType easeType, float moveTime)
    {
        yield return null;

        // 스프라이트 조절
        SetAnimatorTrigger(gameObject, targetPosition);

        // 이동 처리
        iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", easeType, "time", moveTime));
    }

    // 지정 장소로 곡선을 그리며 1회 이동
    public IEnumerator EnemyMovePathOnce(Vector3[] paths, iTween.EaseType easeType, float moveTime)
    {
        yield return null;

        // 스프라이트 조절
        SetAnimatorTrigger(gameObject, paths[paths.Length - 1]);

        // 이동 처리
        iTween.MoveTo(gameObject, iTween.Hash("path", paths, "easetype", easeType, "time", moveTime));
    }

    // 지정 장소 이동 후 일정 시간 지난 뒤 다시 이동 (총 2회)
    public IEnumerator EnemyMoveTwice(Vector3 targetPositionFirst, Vector3 targetPositionSecond, iTween.EaseType easeType1, iTween.EaseType easeType2, float moveTime1, float moveTime2, float waitTime)
    {
        yield return null;

        // 스프라이트 조절
        SetAnimatorTrigger(gameObject, targetPositionFirst);

        // 이동 1 처리
        iTween.MoveTo(gameObject, iTween.Hash("position", targetPositionFirst, "easetype", easeType1, "time", moveTime1));

        yield return new WaitForSeconds(waitTime);

        if (gameObject.activeSelf.Equals(true))
        {
            // 스프라이트 조절
            SetAnimatorTrigger(gameObject, targetPositionSecond);

            // 이동 2 처리
            iTween.MoveTo(gameObject, iTween.Hash("position", targetPositionSecond, "easetype", easeType2, "time", moveTime2));
        }
    }

    #endregion

    #region 애니메이터 트리거 설정

    public void SetAnimatorTrigger(GameObject gameObject, Vector3 targetPosition)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        // 스프라이트 조절
        if (gameObject.transform.position.x > targetPosition.x)
        {
            animator.SetTrigger("isLeftMove");
        }
        else if (gameObject.transform.position.x < targetPosition.x)
        {
            animator.SetTrigger("isRightMove");
        }
        else
        {
            animator.SetTrigger("isIdle");
        }
    }

    #endregion
}
