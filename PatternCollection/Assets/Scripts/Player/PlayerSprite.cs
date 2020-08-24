using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    private SpriteRenderer playerSprite;
    [SerializeField] private Sprite[] playerNormalSpriteArray;
    [SerializeField] private Sprite[] playerLeftMoveSpriteArray;
    [SerializeField] private Sprite[] playerRightMoveSpriteArray;

    private float spriteChangeDelay;
    private int spriteIndexNumber;
    private bool isLeftMove;
    private bool isRightMove;

	void Start ()
    {
        playerSprite = GetComponent<SpriteRenderer>();

        spriteChangeDelay = 0.0f;
        spriteIndexNumber = 0;
        isLeftMove = false;
        isRightMove = false;
	}
	
	void Update ()
    {
        spriteChangeDelay += Time.deltaTime;
        if (spriteChangeDelay >= 0.08f)
        {
            spriteChangeDelay = 0.0f;
            if (spriteIndexNumber == 7)
            {
                if (isLeftMove == false && isRightMove == false)
                {
                    spriteIndexNumber = 0;
                }
            }
            else
            {
                spriteIndexNumber++;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isLeftMove = true;
            isRightMove = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            isLeftMove = false;
            isRightMove = true;
        }
        else
        {
            isLeftMove = false;
            isRightMove = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            spriteIndexNumber = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            spriteIndexNumber = 0;
        }

        if (isLeftMove == true)
        {
            playerSprite.sprite = playerLeftMoveSpriteArray[spriteIndexNumber];
        }
        else if (isRightMove == true)
        {
            playerSprite.sprite = playerRightMoveSpriteArray[spriteIndexNumber];
        }
        else
        {
            playerSprite.sprite = playerNormalSpriteArray[spriteIndexNumber];
        }
	}
}