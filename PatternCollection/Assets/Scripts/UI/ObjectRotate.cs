﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    public float rotateSpeed;

	void Update()
	{
		transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
	}
}
