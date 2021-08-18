using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    public float rotateSpeed;

    private void Update()
    {
        // 1초에 rotateSpeed 수치만큼 회전
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}
