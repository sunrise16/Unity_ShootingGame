using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    public float rotateSpeed;

	void Start()
	{
        StartCoroutine(Rotate());
	}

    public IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }
    }
}
