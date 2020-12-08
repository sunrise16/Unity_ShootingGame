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
                text.text = string.Format("{0} ({1} / 8)", GameData.currentPlayerLife, GameData.currentPlayerLifeFragment);
                break;
            case "CurrentSpell":
                text.text = string.Format("{0} ({1} / 8)", GameData.currentPlayerSpell, GameData.currentPlayerSpellFragment);
                break;
            case "CurrentPower":
                if (GameData.currentPower.Equals(0.0f) || GameData.currentPower.Equals(1.0f) || GameData.currentPower.Equals(2.0f) ||
                    GameData.currentPower.Equals(3.0f) || GameData.currentPower.Equals(4.0f))
                {
                    text.text = string.Format("{0}.00 / 4.00", Mathf.Round(GameData.currentPower * 100) * 0.01f);
                }
                else
                {
                    text.text = string.Format("{0} / 4.00", Mathf.Round(GameData.currentPower * 100) * 0.01f);
                }
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
