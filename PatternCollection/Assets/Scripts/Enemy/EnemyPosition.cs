using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyPosition : MonoBehaviour
{
    private Vector2 margin;
    
    void Start()
    {
        margin = new Vector2(0.03f, 0.03f);
    }
	
	void Update()
    {
        MoveInScreen();
    }

    private void MoveInScreen()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        position.x = Mathf.Clamp(position.x, 0.0f + margin.x, 1.0f - margin.x);
        position.y = Mathf.Clamp(position.y, 0.0f + margin.y, 1.0f - margin.y);

        transform.position = Camera.main.ViewportToWorldPoint(position);
    }
}
