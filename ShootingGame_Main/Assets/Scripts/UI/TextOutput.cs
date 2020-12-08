using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TextOutput : MonoBehaviour
{
    private Text text;
    private long score;

	private void Start()
    {
        text = GetComponent<Text>();
        score = 0;
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
                if (score < GameData.currentScore)
                {
                    score += 11111;
                    if (score >= GameData.currentScore)
                    {
                        score = GameData.currentScore;
                    }
                }
                text.text = score.ToString();
                break;
            case "CurrentLife":
                text.text = string.Format("{0} ({1} / 8)", GameData.currentPlayerLife, GameData.currentPlayerLifeFragment);
                break;
            case "CurrentSpell":
                text.text = string.Format("{0} ({1} / 8)", GameData.currentPlayerSpell, GameData.currentPlayerSpellFragment);
                break;
            case "CurrentPower":
                text.text = string.Format("{0} / 4.00", GameData.currentPower.ToString("N2"));
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
