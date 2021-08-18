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
        // 이 스크립트를 할당받은 게임 오브젝트의 이름 체크
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
                // 게임 데이터에 저장된 현재 점수 따라잡기 (점수가 11111씩 오르는 연출)
                if (score < GameData.currentScore)
                {
                    score += 11111;
                    // UI에 표시되는 점수가 게임 데이터에 저장된 현재 점수를 넘지 않도록 해주기
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
