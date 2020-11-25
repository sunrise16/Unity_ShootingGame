using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    private SpriteRenderer enemySprite;
    [SerializeField] private Sprite[] enemyNormalSpriteArray;
    [SerializeField] private Sprite[] enemyMoveSpriteArray;

    private float spriteChangeDelay;
    public int spriteIndexNumber;
    public bool isLeftMove;
    public bool isRightMove;
    public bool isSpriteReturn;

    void Start()
    {
        enemySprite = GetComponent<SpriteRenderer>();

        spriteChangeDelay = 0.0f;
        spriteIndexNumber = 0;
        isLeftMove = false;
        isRightMove = false;
        isSpriteReturn = false;
    }

    void Update()
    {
        spriteChangeDelay += Time.deltaTime;
        if (spriteChangeDelay >= 0.08f)
        {
            spriteChangeDelay = 0.0f;
            if (spriteIndexNumber >= 3)
            {
                if (isLeftMove.Equals(false) && isRightMove.Equals(false))
                {
                    spriteIndexNumber = 0;
                }
                else
                {
                    if (isSpriteReturn.Equals(true))
                    {
                        spriteIndexNumber--;
                    }
                    else
                    {
                        spriteIndexNumber = 3;
                    }
                }
            }
            else
            {
                if (isSpriteReturn.Equals(true))
                {
                    spriteIndexNumber--;
                    if (spriteIndexNumber <= 0)
                    {
                        spriteIndexNumber = 0;
                        isSpriteReturn = false;
                        isLeftMove = false;
                        isRightMove = false;
                        if (GetComponent<SpriteRenderer>().flipX.Equals(true))
                        {
                            GetComponent<SpriteRenderer>().flipX = false;
                        }
                    }
                }
                else
                {
                    spriteIndexNumber++;
                }
            }
        }
        
        if (isLeftMove.Equals(true) || isRightMove.Equals(true))
        {
            enemySprite.sprite = enemyMoveSpriteArray[spriteIndexNumber];
        }
        else
        {
            enemySprite.sprite = enemyNormalSpriteArray[spriteIndexNumber];
        }
    }
}