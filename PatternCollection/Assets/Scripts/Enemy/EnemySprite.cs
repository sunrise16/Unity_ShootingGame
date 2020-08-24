using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    private SpriteRenderer enemySprite;
    [SerializeField] private Sprite[] enemyNormalSpriteArray;
    [SerializeField] private Sprite[] enemyLeftMoveSpriteArray;
    [SerializeField] private Sprite[] enemyRightMoveSpriteArray;

    private float spriteChangeDelay;
    private int spriteIndexNumber;
    public bool isLeftMove;
    public bool isRightMove;

    void Start()
    {
        enemySprite = GetComponent<SpriteRenderer>();

        spriteChangeDelay = 0.0f;
        spriteIndexNumber = 0;
        isLeftMove = false;
        isRightMove = false;
    }

    void Update()
    {
        spriteChangeDelay += Time.deltaTime;
        if (spriteChangeDelay >= 0.08f)
        {
            spriteChangeDelay = 0.0f;
            if (spriteIndexNumber == 3)
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
        
        if (isLeftMove == true)
        {
            enemySprite.sprite = enemyLeftMoveSpriteArray[spriteIndexNumber];
        }
        else if (isRightMove == true)
        {
            enemySprite.sprite = enemyRightMoveSpriteArray[spriteIndexNumber];
        }
        else
        {
            enemySprite.sprite = enemyNormalSpriteArray[spriteIndexNumber];
        }
    }
}