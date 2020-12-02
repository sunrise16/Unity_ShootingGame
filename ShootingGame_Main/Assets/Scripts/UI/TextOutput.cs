using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TextOutput : MonoBehaviour
{
    private Text text;
    private float power;

	private void Start()
    {
        text = GetComponent<Text>();
	}

    private void Update()
    {
        switch (gameObject.name)
        {
            case "Difficulty":
                if (GameData.gameDifficulty.Equals(GameDifficulty.DIFFICULTY_EASY))
                {
                    text.text = "EASY";
                }
                else if (GameData.gameDifficulty.Equals(GameDifficulty.DIFFICULTY_NORMAL))
                {
                    text.text = "NORMAL";
                }
                else if (GameData.gameDifficulty.Equals(GameDifficulty.DIFFICULTY_HARD))
                {
                    text.text = "HARD";
                }
                else if (GameData.gameDifficulty.Equals(GameDifficulty.DIFFICULTY_LUNATIC))
                {
                    text.text = "LUNATIC";
                }
                else if (GameData.gameDifficulty.Equals(GameDifficulty.DIFFICULTY_EXTRA))
                {
                    text.text = "EXTRA";
                }
                break;
            case "CurrentGameScore":
                text.text = GameData.currentScore.ToString();
                break;
            case "CurrentLife":
                text.text = GameData.currentPlayerLife.ToString();
                break;
            case "CurrentLifeFragment":
                text.text = GameData.currentPlayerLifeFragment.ToString();
                break;
            case "CurrentSpell":
                text.text = GameData.currentPlayerSpell.ToString();
                break;
            case "CurrentSpellFragment":
                text.text = GameData.currentPlayerSpellFragment.ToString();
                break;
            case "CurrentPower":
                power = Mathf.Round(GameData.currentPower * 100) * 0.01f;
                text.text = power.ToString();
                break;
            case "CurrentScoreItem":
                text.text = GameData.currentScoreItem.ToString();
                break;
            case "CurrentGraze":
                text.text = GameData.currentGraze.ToString();
                break;
            default:
                break;
        }
    }
}
