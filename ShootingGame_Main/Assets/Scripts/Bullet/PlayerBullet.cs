using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private GameManager gameManager;
    private Transform bulletTransform;
    private Transform bulletPoolTransform;

    private float playerBulletSpeed;

	private void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        bulletTransform = GameObject.Find("BULLET").transform;
        bulletPoolTransform = GameObject.Find("BulletPool").transform;

        playerBulletSpeed = 24.0f;
	}

    private void Update()
    {
        transform.Translate(Vector2.right * playerBulletSpeed * Time.deltaTime);
    }
}
